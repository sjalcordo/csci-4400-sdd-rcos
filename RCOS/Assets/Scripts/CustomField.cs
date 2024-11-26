/*
 *  AUTHOR: Sean (alcors@rpi.edu)
 *  DESC: Class that is used to easily access the text and button of a custom field for the customization options.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Gameplay
{
    public class CustomField : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_Text _textObj;
        public TMP_Text textObj => _textObj;
        [SerializeField] private Button _button;
        public Button button => _button;

        // Member Variables & Read-only Accessors
        private bool _isAnswer;
        public bool isAnswer => _isAnswer;

        private void Start()
        {
            // Get a reference to the customization handler for adding event listeners.
            CustomizationHandler handler = GameObject.Find("CustomizationHandler").GetComponent<CustomizationHandler>();

            // Add the corresponding listener
            if (_isAnswer)
            {
                _button.onClick.AddListener(delegate { handler.RemoveAnswer(_textObj.text); });
            }
            else
            {
                _button.onClick.AddListener(delegate { handler.RemovePrompt(_textObj.text); });
            }
        }

        // Sets answer or prompt as needed
        public void Setup(bool answer)
        {
            _isAnswer = answer;
        }

        // Used to destroy self.
        public void DestroySelf()
        {
            Destroy(gameObject);
        }
    }
}

