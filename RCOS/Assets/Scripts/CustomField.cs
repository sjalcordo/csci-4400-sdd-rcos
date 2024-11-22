using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Gameplay
{
    public class CustomField : MonoBehaviour
    {
        [SerializeField] private TMP_Text _textObj;
        public TMP_Text textObj => _textObj;
        [SerializeField] private Button _button;
        public Button button => _button;

        private bool _isAnswer;
        public bool isAnswer => _isAnswer;

        private void Start()
        {
            CustomizationHandler handler = GameObject.Find("CustomizationHandler").GetComponent<CustomizationHandler>();

            if (_isAnswer)
            {
                _button.onClick.AddListener(delegate { handler.RemoveAnswer(_textObj.text); });
            }
            else
            {
                _button.onClick.AddListener(delegate { handler.RemovePrompt(_textObj.text); });
            }
        }

        public void Setup(bool answer)
        {
            _isAnswer = answer;
        }

        public void DestroySelf()
        {
            Destroy(gameObject);
        }
    }
}

