/*
 *  AUTHOR: Sean (alcors@rpi.edu)
 *  DESC: Helper class used to keep track of each player's profiles.
 */

using System.Collections.Generic;
using UnityEngine;
using SocketIOClient;
namespace Gameplay
{
    public class ProfileHandler : MonoBehaviour
    {
        [SerializeField] private PromptHandler _promptHandler;
        [SerializeField] private ProgressHandler _progressHandler;
        [SerializeField] private int _maxAnswers = 5;

        private Dictionary<string, List<Prompt>> _playerResponses = new Dictionary<string, List<Prompt>>();

        private void Start()
        {
            // Adds a listener to when we receive a message from the server.
            Sockets.ServerUtil.manager.OnSocketEvent.AddListener(OnSocket);
        }

        private void OnSocket(string name, SocketIOResponse response)
        {
            // Only listen for verify lobby commands
            if (name != "on-prompt-response") return;

            OnPromptResponse(response.GetValue<string>(0), response.GetValue<string>(1));
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
            }
            else
            {
                Sockets.ServerUtil.manager.SendEvent("questions-finished", hashedIP);
            }

            if(CheckCompletion())
            {

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


