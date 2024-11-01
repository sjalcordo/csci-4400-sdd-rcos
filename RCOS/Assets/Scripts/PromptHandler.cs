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
            if (name != "on-prompt-response" && name != "request-prompt") return;

            switch (name)
            {
                case "on-prompt-response":
                    string hashedIP = response.GetValue<string>(0);
                    string promptResponse = response.GetValue<string>(1);

                    // If there is no active prompt for the current key.
                    if (!_currentPrompts.ContainsKey(hashedIP))
                        return;

                    Debug.Log("Received Prompt Response: " + response.GetValue<string>() + " " + response.GetValue<string>(1));
                    break;
                case "request-prompt":

                    break;
            }
        }

        private void ParsePrompts()
        {
            List<Dictionary<string, object>> sheetLines = Helpers.CSVReader.Read(_promptFilename);
            foreach (Dictionary<string, object> line in sheetLines)
            {
                Debug.Log(line.Keys);
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

