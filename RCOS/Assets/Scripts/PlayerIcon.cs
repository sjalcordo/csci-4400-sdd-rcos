/*
 *  AUTHOR: Sean (alcors@rpi.edu)
 *  DESC: Helper class used to easily set the text and picture of a player icon.
 */

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerIcon : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameObj;
    [SerializeField] private RawImage _pfp;

    public void SetName(string name)
    {
        _nameObj.text = name;
    }

    public void SetPfp(Texture pfp)
    {
        _pfp.texture = pfp;
    }
}
