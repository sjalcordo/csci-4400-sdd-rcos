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
        [Header("References")]
        [SerializeField] private LobbyHandler _lobbyHandler;
        [SerializeField] private PromptHandler _promptHandler;
        [SerializeField] private ProgressHandler _progressHandler;
        [Header("Parameters")]
        [SerializeField] private int _maxAnswers = 5;
        [Space(5)]
        [SerializeField] private float _defaultTimers = 30f;
        public float timer;
        [SerializeField] private int _tickPerUpdate = 5;

        // Member Variables
        private int _currentTick = 0;
        private float _timeForUpdate = 0f;

        public int maxAnswers => _maxAnswers;

        private Dictionary<string, List<Prompt>> _playerResponses = new Dictionary<string, List<Prompt>>();
        public Dictionary<string, List<Prompt>> playerResponses => _playerResponses;

        public UnityEvent OnProfileCompletion;

        private Dictionary<string, bool> _activePlayers = new Dictionary<string, bool>();
        private Dictionary<string, float> _timers = new Dictionary<string, float>();

        private bool _creationActive = false;

        private void Awake()
        {
            timer = _defaultTimers;
        }

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

            // Prevents overloading the server with too many signals
            if (_currentTick < _tickPerUpdate)
            {
                _currentTick++;
                return;
            }

            // Iterates and sends out the time to each player.
            foreach (string hashedIP in _lobbyHandler.hashedIPs)
            {
                if (!_timers.ContainsKey(hashedIP) || !_activePlayers.ContainsKey(hashedIP) || !_activePlayers[hashedIP]) { continue; }

                Sockets.ServerUtil.manager.SendEvent("timer-update", hashedIP, _timers[hashedIP]);

                if (_timers[hashedIP] > 0)
                {
                    _timers[hashedIP] = _timers[hashedIP] - _timeForUpdate;
                    if (_timers[hashedIP] <= 0)
                    {
                        _timers[hashedIP] = timer;
                        OnPromptResponse(hashedIP, _promptHandler.GetRandomAnswer(hashedIP));
                        Sockets.ServerUtil.manager.SendEvent("time-out", hashedIP);
                    }
                }
            }
            _timeForUpdate = 0f;
        } // Update

        /// <summary>
        /// Starts the profile creation after a delay.
        /// </summary>
        public void StartProfileCreation()
        {
            Invoke(nameof(DelayedStartProfiles), 0.5f);
        }

        /// <summary>
        /// Used to delay start profiles, this will reset all of the timers.
        /// </summary>
        private void DelayedStartProfiles()
        {
            _creationActive = true;
            foreach (string hashedIP in _lobbyHandler.hashedIPs)
            {
                _timers[hashedIP] = timer;
                _activePlayers[hashedIP] = false;
                _playerResponses[hashedIP] = new List<Prompt>();
            }
        }

        private void OnSocket(string name, SocketIOResponse response)
        {
            // Only listen for verify lobby commands
            if (name != "on-prompt-response" && name != "on-request-prompt" && name != "on-get-timer-duration") return;

            switch (name)
            {
                case "on-prompt-response":
                    OnPromptResponse(response.GetValue<string>(0), response.GetValue<string>(1));
                    break;
                case "on-request-prompt":
                    Debug.Log("connected player: " + response.GetValue<string>(0));
                    _activePlayers[response.GetValue<string>(0)] = true;
                    break;
                case "on-get-timer-duration":
                    Sockets.ServerUtil.manager.SendEvent("send-timer-duration", response.GetValue<string>(0), timer);
                    break;
            }
        }

        /// <summary>
        /// On a response to the prompt, as well as continuation of the game flow.
        /// </summary>
        private void OnPromptResponse(string hashedIP, string promptResponse)
        {
            // If there is no active prompt for the current key.
            if (!_promptHandler.currentPrompts.ContainsKey(hashedIP))
                return;

            _playerResponses[hashedIP].Add(new Prompt(_promptHandler.currentPrompts[hashedIP], promptResponse));
            _progressHandler.UpdateProgress(hashedIP, (float) _playerResponses[hashedIP].Count / _maxAnswers);

            // Check if we have already answered all of the prompts.
            if (_playerResponses[hashedIP].Count < _maxAnswers)
            {
                _promptHandler.RemoveAnswer(hashedIP, promptResponse);
                _promptHandler.SendNextPrompt(hashedIP);
                _timers[hashedIP] = timer;
                return;
            }

            _activePlayers[hashedIP] = false;

            // Checks if all players have finished their prompts.
            if(CheckCompletion())
            {
                _creationActive = false;
                OnProfileCompletion.Invoke();
                Sockets.ServerUtil.manager.SendEvent("questions-finished", hashedIP);
                Sockets.ServerUtil.manager.SendEvent("between-presentations");
            }
            else
            {
                Sockets.ServerUtil.manager.SendEvent("move-to-waiting", hashedIP);
            }
        } // OnPromptResponse

        /// <summary>
        /// Checks if all players have completed their prompts.
        /// </summary>
        /// <returns></returns>
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


