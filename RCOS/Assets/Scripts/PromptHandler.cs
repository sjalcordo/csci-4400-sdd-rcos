/*
 *  AUTHOR: Sean (alcors@rpi.edu)
 *  DESC: Helper class used to store and send out the different prompts used during the game.
 */

using System.Collections.Generic;
using UnityEngine;
using SocketIOClient;
namespace Gameplay
{
    /// <summary>
    /// Helper class used to store the prompt and response as a string.
    /// </summary>
    [System.Serializable]
    public class Prompt
    {
        public string prompt;
        public string response;

        // Constructor
        public Prompt(string prompt)
        {
            this.prompt = prompt;
        }

        public Prompt(string prompt, string response)
        {
            this.prompt = prompt;
            this.response = response;
        }

        // Copy constructor
        public Prompt(Prompt prompt)
        {
            this.prompt = prompt.prompt;
            response = prompt.response;
        }
    }

    public class PromptHandler : MonoBehaviour
    {
        // Reference to the lobby handler to get the list of players.
        [SerializeField] private LobbyHandler _lobbyHandler;
        [SerializeField] private ProfileHandler _profileHandler;
        [Space(5)]

        // Inspector-Exposed Private Variables.
        [SerializeField] private string _promptFilename = "prompts";
        [SerializeField] private string _answerFilename = "answers";
        [Space(5)]
        [SerializeField] private List<Prompt> _prompts = new List<Prompt>();
        [SerializeField] private List<string> _answers = new List<string>();

        // Member Variables
        private List<Prompt> _initialPrompts = new List<Prompt>();
        private List<string> _initialAnswers = new List<string>();

        private Dictionary<string, string> _currentPrompts = new Dictionary<string, string>();
        private Dictionary<string, List<Prompt>> _availablePrompts = new Dictionary<string, List<Prompt>>();
        public Dictionary<string, string> currentPrompts => _currentPrompts;
        private Dictionary<string, List<string>> _answerPools = new Dictionary<string, List<string>>();
        private Dictionary<string, List<string>> _availableAnswers = new Dictionary<string, List<string>>();
        private Dictionary<string, List<Prompt>> _playerResponses = new Dictionary<string, List<Prompt>>();
        private Dictionary<string, bool> _fillInAvailable = new Dictionary<string, bool>();

        private void Start()
        {
            // Adds a listener to when we receive a message from the server.
            Sockets.ServerUtil.manager.OnSocketEvent.AddListener(OnSocket);

            ParsePrompts();

        }

        /// <summary>
        /// Adds the given list to the prompts to be distributed.
        /// </summary>
        public void AddCustomPrompts(List<Prompt> additionalPrompts)
        {
            _prompts = new List<Prompt>(_initialPrompts);
            foreach (Prompt prompt in additionalPrompts)
            {
                _prompts.Add(prompt);
            }
        }

        /// <summary>
        /// Adds given list of answers to the answers to be distributed.
        /// </summary>
        /// <param name="additionalAnswers"></param>
        public void AddCustomAnswers(List<string> additionalAnswers)
        {
            _answers = new List<string>(_initialAnswers);
            foreach(string answer in additionalAnswers)
            {
                _answers.Add(answer);
            }
        }

        /// <summary>
        /// Refreshes and populates the answer pool by calling Refill on each hashedIP.
        /// </summary>
        public void PopulateAnswerPool()
        {
            foreach (string hashedIP in _lobbyHandler.hashedIPs)
            {
                _answerPools[hashedIP] = new List<string>();
                _fillInAvailable[hashedIP] = true;

                RefillAnswers(hashedIP);
            }
        }

        /// <summary>
        /// Refills the answers for a given hashedIP.
        /// </summary>
        public void RefillAnswers(string hashedIP)
        {
            // If the key does not exist
            if (!_answerPools.ContainsKey(hashedIP))
                return;

            _answerPools[hashedIP].Remove("");

            while (_answerPools[hashedIP].Count < 5)
            {
                // If there are no more available answers, refill the list.
                if (!_availableAnswers.ContainsKey(hashedIP) || _availableAnswers[hashedIP].Count <= 0)
                {
                    _availableAnswers[hashedIP] = _answers;
                }

                // Add Answer
                int index = Random.Range(0, _availableAnswers[hashedIP].Count);
                Debug.Log("Available answers length " + _availableAnswers[hashedIP].Count + " " + index);
                _answerPools[hashedIP].Add(_availableAnswers[hashedIP][index]);
                _availableAnswers[hashedIP].RemoveAt(index);
            }

            if (_fillInAvailable[hashedIP])
            {
                if (!_answerPools[hashedIP].Contains(""))
                {
                    _answerPools[hashedIP].Add("");
                }
            }
        }

        /// <summary>
        /// Sets the current prompt for a given hashedIP.
        /// </summary>
        public void SetPrompt(string ID, string prompt)
        {
            _currentPrompts[ID] = prompt;
        }

        /// <summary>
        /// Goes through each possible socket option for this script.
        /// </summary>
        private void OnSocket(string name, SocketIOResponse response)
        {
            // Only listen for verify lobby commands
            if (name != "on-request-prompt" && name != "on-request-answers" && name != "on-used-fill-in") return;

            switch (name)
            {
                case "on-request-prompt":
                    string hashedIP = response.GetValue<string>(0);
                    
                    Sockets.ServerUtil.manager.SendEvent("send-prompt", hashedIP, _currentPrompts[hashedIP], _profileHandler.GetResponseCount(hashedIP) + 1);
                    break;
                case "on-request-answers":
                    hashedIP = response.GetValue<string>(0);
                    Sockets.ServerUtil.manager.SendEvent("send-answers", hashedIP, _answerPools[hashedIP]);
                    break;
                case "on-used-fill-in":
                    hashedIP = response.GetValue<string>(0);
                    _answerPools[hashedIP].Remove("");
                    _fillInAvailable[hashedIP] = false;
                    RefillAnswers(hashedIP);
                    break;
            }
        }

        /// <summary>
        /// Starts the game by distributing the prompts.
        /// </summary>
        public void StartPrompts()
        {
            Sockets.ServerUtil.manager.SendEvent("game-start");

            PopulateAnswerPool();

            foreach (string hashedIP in _lobbyHandler.hashedIPs)
            {
                _availablePrompts[hashedIP] = _prompts;

                string prompt = GetRandomPrompt(hashedIP);
                _currentPrompts[hashedIP] = prompt;

            }
        }

        /// <summary>
        /// Gets a random answer from the given pool.
        /// </summary>
        public string GetRandomAnswer(string hashedIP)
        {
            if (!_answerPools.ContainsKey(hashedIP))
            {
                return "";
            }

            return _answerPools[hashedIP][Random.Range(0, _answerPools[hashedIP].Count)];
        }

        /// <summary>
        /// Removes an answer from the pool associated with the given hashedIP.
        /// </summary>
        public void RemoveAnswer(string hashedIP, string response)
        {
            if (!_answerPools.ContainsKey(hashedIP))
            {
                return;
            }

            _answerPools[hashedIP].Remove(response);
            RefillAnswers(hashedIP);
        }

        /// <summary>
        /// Sends the next prompt for the hashedIP.
        /// </summary>
        public void SendNextPrompt(string hashedIP)
        {
            RefillAnswers(hashedIP);
            _currentPrompts[hashedIP] = GetRandomPrompt(hashedIP);
            Sockets.ServerUtil.manager.SendEvent("send-answers", hashedIP, _answerPools[hashedIP]);
            Sockets.ServerUtil.manager.SendEvent("send-prompt", hashedIP, _currentPrompts[hashedIP], _profileHandler.GetResponseCount(hashedIP) + 1);
        }

        /// <summary>
        /// Gets a random prompt from the prompt pool for the hashedIP.
        /// </summary>
        private string GetRandomPrompt(string hashedIP)
        {
            if (!_availablePrompts.ContainsKey(hashedIP))
            {
                return "";
            }

            int index = Random.Range(0, _prompts.Count);
            string prompt = _availablePrompts[hashedIP][index].prompt;
            _availablePrompts[hashedIP].RemoveAt(index);
            return prompt;
        }

        /// <summary>
        /// Goes through the prompts and answers in the CSV file.
        /// </summary>
        private void ParsePrompts()
        {
            List<Dictionary<string, object>> sheetLines = Helpers.CSVReader.Read(_promptFilename);
            foreach (Dictionary<string, object> line in sheetLines)
            {
                _prompts.Add(new Prompt(line["prompt"].ToString()));
            }

            sheetLines = Helpers.CSVReader.Read(_answerFilename);
            foreach (Dictionary<string, object> line in sheetLines)
            {
                _answers.Add(line["answers"].ToString());
            }
            _initialPrompts = new List<Prompt>(_prompts);
            _initialAnswers = new List<string>(_answers);
        }
    }
}

