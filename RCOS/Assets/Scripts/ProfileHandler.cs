/*
 *  AUTHOR: Sean (alcors@rpi.edu)
 *  DESC: Helper class used to keep track of each player's profiles.
 */

using System.Collections.Generic;
using UnityEngine;
using SocketIOClient;
using UnityEngine.Events;

namespace Gameplay
{
    public class ProfileHandler : MonoBehaviour
    {
        [SerializeField] private LobbyHandler _lobbyHandler;
        [SerializeField] private PromptHandler _promptHandler;
        [SerializeField] private ProgressHandler _progressHandler;
        [SerializeField] private int _maxAnswers = 5;
        [Space(5)]
        [SerializeField] private float _defaultTimers = 30f;
        [SerializeField] private int _tickPerUpdate = 5;
        private int _currentTick = 0;
        private float _timeForUpdate = 0f;

        public int maxAnswers => _maxAnswers;

        private Dictionary<string, List<Prompt>> _playerResponses = new Dictionary<string, List<Prompt>>();
        public Dictionary<string, List<Prompt>> playerResponses => _playerResponses;

        public UnityEvent OnProfileCompletion;

        private Dictionary<string, bool> _activePlayers = new Dictionary<string, bool>();
        private Dictionary<string, float> _timers = new Dictionary<string, float>();

        private bool _creationActive = false;

        private void Start()
        {
            // Adds a listener to when we receive a message from the server.
            Sockets.ServerUtil.manager.OnSocketEvent.AddListener(OnSocket);
        }

        private void Update()
        {
            if (!_creationActive)
            {
                return;
            }

            _timeForUpdate += Time.deltaTime;

            if (_currentTick < _tickPerUpdate)
            {
                _currentTick++;
                return;
            }

            foreach (string hashedIP in _lobbyHandler.hashedIPs)
            {
                if (!_timers.ContainsKey(hashedIP) || !_activePlayers.ContainsKey(hashedIP) || !_activePlayers[hashedIP]) { continue; }

                Sockets.ServerUtil.manager.SendEvent("timer-update", hashedIP, _timers[hashedIP]);

                if (_timers[hashedIP] > 0)
                {
                    _timers[hashedIP] = _timers[hashedIP] - _timeForUpdate;
                    if (_timers[hashedIP] <= 0)
                    {
                        _timers[hashedIP] = _defaultTimers;
                        OnPromptResponse(hashedIP, _promptHandler.GetRandomAnswer(hashedIP));
                        Sockets.ServerUtil.manager.SendEvent("time-out", hashedIP);
                    }
                }
            }
            _timeForUpdate = 0f;
        }

        public void StartProfileCreation()
        {
            Invoke(nameof(DelayedStartProfiles), 0.5f);
        }

        private void DelayedStartProfiles()
        {
            _creationActive = true;
            foreach (string hashedIP in _lobbyHandler.hashedIPs)
            {
                _timers[hashedIP] = _defaultTimers;
                _activePlayers[hashedIP] = false;
            }
        }

        private void OnSocket(string name, SocketIOResponse response)
        {
            // Only listen for verify lobby commands
            if (name != "on-prompt-response" && name != "on-request-prompt") return;

            switch (name)
            {
                case "on-prompt-response":
                    OnPromptResponse(response.GetValue<string>(0), response.GetValue<string>(1));
                    break;
                case "on-request-prompt":
                    Debug.Log("connected player: " + response.GetValue<string>(0));
                    _activePlayers[response.GetValue<string>(0)] = true;
                    break;
            }
        }
        private void OnPromptResponse(string hashedIP, string promptResponse)
        {
            // If there is no active prompt for the current key.
            if (!_promptHandler.currentPrompts.ContainsKey(hashedIP))
                return;

            if (!_playerResponses.ContainsKey(hashedIP))
            {
                _playerResponses[hashedIP] = new List<Prompt>();
            }

            _playerResponses[hashedIP].Add(new Prompt(_promptHandler.currentPrompts[hashedIP], promptResponse));
            _progressHandler.UpdateProgress(hashedIP, (float) _playerResponses[hashedIP].Count / _maxAnswers);

            if (_playerResponses[hashedIP].Count < _maxAnswers)
            {
                _promptHandler.RemoveAnswer(hashedIP, promptResponse);
                _promptHandler.SendNextPrompt(hashedIP);
                _timers[hashedIP] = _defaultTimers;
                return;
            }

            _activePlayers[hashedIP] = false;

            if(CheckCompletion())
            {
                _creationActive = false;
                OnProfileCompletion.Invoke();
                Sockets.ServerUtil.manager.SendEvent("questions-finished", hashedIP);
            }
            else
            {
                Sockets.ServerUtil.manager.SendEvent("move-to-waiting", hashedIP);
            }
        }

        private bool CheckCompletion()
        {
            foreach (List<Prompt> prompts in _playerResponses.Values)
            {
                if (prompts.Count < _maxAnswers)
                {
                    return false;
                }
            }
            return true;
        }

        public int GetResponseCount(string hashedIP)
        {
            if (!_playerResponses.ContainsKey(hashedIP))
                return 0;

            return _playerResponses[hashedIP].Count;
        }
    }
}


