using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InputPlaceholder : MonoBehaviour
{
    [SerializeField] private string inputText;
    [SerializeField] private GameObject reactionGroup;
    [SerializeField] private TMP_Text reactionTextBox;

    public void GrabFromInputField(string input) {
        inputText = input;
        DisplayReactionToInput();
    }
    public void DisplayReactionToInput() {
        reactionTextBox.text = inputText;
        reactionGroup.SetActive(true);
    }
}
