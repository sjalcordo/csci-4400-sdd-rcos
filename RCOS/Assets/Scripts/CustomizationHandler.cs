/*
 *  AUTHOR: Sean (alcors@rpi.edu)
 *  DESC: Handles the customization options, saving them, and loading them.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Gameplay
{
    [System.Serializable]
    public class CustomProperties {
        public List<Prompt> prompts = new List<Prompt>();
        public List<string> answers = new List<string>();
        public float customTimer = 30f;
    }

    public class CustomizationHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject _fieldPrefab;
        [SerializeField] private PromptHandler _promptHandler;
        [SerializeField] private ProfileHandler _profileHandler;
        [Space(5)]
        [SerializeField] private TMP_InputField _answerField;
        [SerializeField] private TMP_InputField _promptField;
        [SerializeField] private TMP_InputField _timerField;
        [Space(5)]
        [SerializeField] private Transform _answerContainer;
        [SerializeField] private Transform _promptContainer;

        [Header("Parameters")]
        [SerializeField] private string _customSaveKey = "Customization";

        private CustomProperties _currentProperties;

        private void Start()
        {
            if (!PlayerPrefs.HasKey(_customSaveKey)) {
                _currentProperties = new CustomProperties();    
                return; 
            }

            _currentProperties = JsonUtility.FromJson<CustomProperties>(PlayerPrefs.GetString(_customSaveKey));

            foreach (string answer in _currentProperties.answers)
            {
                AddAnswerToContainer(answer);
            }

            foreach(Prompt prompt in _currentProperties.prompts)
            {
                AddPromptToContainer(prompt);
            }
            SendCustomProperties();
        }

        /// <summary>
        /// Sends the custom properties to the prompt handler and profile handler.
        /// </summary>
        public void SendCustomProperties()
        {
            _promptHandler.AddCustomPrompts(_currentProperties.prompts);
            _promptHandler.AddCustomAnswers(_currentProperties.answers);
            _profileHandler.timer = _currentProperties.customTimer;
        }

        /// <summary>
        /// Adds the answer to the container so that it can be seen and removed.
        /// </summary>
        private void AddAnswerToContainer(string answer)
        {
            GameObject fieldObj = Instantiate(_fieldPrefab);
            fieldObj.transform.SetParent(_answerContainer);
            fieldObj.GetComponent<CustomField>().textObj.text = answer;
            fieldObj.GetComponent<CustomField>().Setup(true);
        }

        /// <summary>
        /// Adds the prompt to the container so that it can be seen and removed.
        /// </summary>
        private void AddPromptToContainer(Prompt prompt)
        {
            GameObject fieldObj = Instantiate(_fieldPrefab);
            fieldObj.transform.SetParent(_promptContainer);
            fieldObj.GetComponent<CustomField>().textObj.text = prompt.prompt;
        }

        /// <summary>
        /// Adds an answer to the saved properties and the container.
        /// </summary>
        public void SubmitAnswer()
        {
            if (_answerField.text == "") { return;  }
            _currentProperties.answers.Add(_answerField.text);
            AddAnswerToContainer(_answerField.text);
            _answerField.text = "";
        }

        /// <summary>
        /// Removes an answer from the saved properties.
        /// </summary>
        /// <param name="answer"></param>
        public void RemoveAnswer(string answer)
        {
            _currentProperties.answers.Remove(answer);
        }

        /// <summary>
        /// Adds an answer to the saved properties and the container.
        /// </summary>
        public void SubmitPrompt()
        {
            if (_promptField.text == "") { return; }
            Prompt temp = new Prompt(_promptField.text);
            _currentProperties.prompts.Add(temp);
            AddPromptToContainer(temp);
            _promptField.text = "";
        }

        /// <summary>
        /// Changes the custom timer.
        /// </summary>
        public void SubmitTimer()
        {
            if (_timerField.text == "") { return; }

            _currentProperties.customTimer = float.Parse(_timerField.text);
        }

        /// <summary>
        /// Removes the prompt from the current properties.
        /// </summary>
        /// <param name="promptText"></param>
        public void RemovePrompt(string promptText)
        {
            foreach (Prompt prompt in _currentProperties.prompts)
            {
                if (prompt.prompt == promptText)
                {
                    _currentProperties.prompts.Remove(prompt);
                    break;
                }
            }
        }

        /// <summary>
        /// Saves the answers and prompts to player prefs.
        /// </summary>
        public void SaveAnswersAndPrompts()
        {
            PlayerPrefs.SetString(_customSaveKey, JsonUtility.ToJson(_currentProperties));
        }
    }
}

