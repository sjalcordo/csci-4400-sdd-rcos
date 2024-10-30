using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Helpers
{
    public class TextHelper : MonoBehaviour
    {
        private TMP_Text _textObj;

        private void Awake()
        {
            _textObj = GetComponent<TMP_Text>();
        }

        public void ChangeText(string text)
        {
            _textObj.text = text;
        }
    }
}

