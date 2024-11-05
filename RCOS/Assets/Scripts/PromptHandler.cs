using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using SocketIOClient;
namespace Gameplay
{
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

        // Copy constructor
        public Prompt(Prompt prompt)
        {
            this.prompt = prompt.prompt;
            this.response = prompt.response;
        }
    }

    public class PromptHandler : MonoBehaviour
    {
        [SerializeField] private LobbyHandler _lobbyHandler;
        [Space(5)]

        private Dictionary<string, Prompt> _currentPrompts = new Dictionary<string, Prompt>();
        [SerializeField] private string _promptFilename = "prompts";
        [SerializeField] private string _answerFilename = "answers";

        [SerializeField] private List<Prompt> _prompts = new List<Prompt>();
        [SerializeField] private List<string> _answers = new List<string>();

        private void Start()
        {
            // Adds a listener to when we receive a message from the server.
            Sockets.ServerUtil.manager.OnSocketEvent.AddListener(OnSocket);

            ParsePrompts();
        }

        public void SetPrompt(string ID, string prompt)
        {
            _currentPrompts[ID] = new Prompt(prompt);
        }

        private void OnSocket(string name, SocketIOResponse response)
        {
            // Only listen for verify lobby commands
            if (name != "on-prompt-response" && name != "on-request-prompt" && name != "on-request-answers") return;

            switch (name)
            {
                case "on-prompt-response":
                    string hashedIP = response.GetValue<string>(0);
                    string promptResponse = response.GetValue<string>(1);

                    // If there is no active prompt for the current key.
                    if (!_currentPrompts.ContainsKey(hashedIP))
                        return;

                    Debug.Log("Received Prompt Response: " + response.GetValue<string>() + " " + response.GetValue<string>(1));
                    _currentPrompts[hashedIP].response = promptResponse;
                    break;
                case "on-request-prompt":
                    hashedIP = response.GetValue<string>(0);
                    
                    Sockets.ServerUtil.manager.SendEvent("send-prompt", hashedIP, _currentPrompts[hashedIP].prompt);
                    break;
                case "on-request-answers":

                    hashedIP = response.GetValue<string>(0);
                    List<string> answers = new List<string>();
                    for (int i = 0; i < 5; i++)
                    {
                        answers.Add(_answers[Random.Range(0, _answers.Count)]);
                    }
                    Sockets.ServerUtil.manager.SendEvent("send-answers", hashedIP, answers);
                    break;
            }
        }

        public void StartPrompts()
        {
            Sockets.ServerUtil.manager.SendEvent("game-start");
            foreach (KeyValuePair<string, Player> playerPair in _lobbyHandler.players)
            {
                string prompt = GetRandomPrompt();
                _currentPrompts[playerPair.Key] = new Prompt(prompt);
            }
        }

        private string GetRandomPrompt()
        {
            return _prompts[Random.Range(0, _prompts.Count)].prompt;
        }

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
        }
    }
}

