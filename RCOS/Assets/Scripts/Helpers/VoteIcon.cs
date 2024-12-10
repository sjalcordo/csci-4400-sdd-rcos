/*
 *  AUTHOR: Sean (alcors@rpi.edu)
 *  DESC: Helper class used to store the icon for voting.
 */

using UnityEngine;
using UnityEngine.UI;

public class VoteIcon : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image _image;
    public Image image => _image;

    [SerializeField] private RectTransform _rectTransform;
    public RectTransform rectTransform => _rectTransform;
}
