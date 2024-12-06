/*
 *  AUTHOR: Sean (alcors@rpi.edu)
 *  DESC: Used on an object with text to easily change the text.
 */

using UnityEngine;
using TMPro;

namespace Helpers
{
    public class TextHelper : MonoBehaviour
    {
        // Member Variables
        private TMP_Text _textObj;

        private void Awake()
        {
            _textObj = GetComponent<TMP_Text>();
        }

        /// <summary>
        /// Changes the text to match the given string.
        /// </summary>
        public void ChangeText(string text)
        {
            _textObj.text = text;
        }
    }
}

