using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerIcon : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameObj;

    public void SetName(string name)
    {
        _nameObj.text = name;
    }
}
