using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VoteIcon : MonoBehaviour
{
    [SerializeField] private Image _image;
    public Image image => _image;

    [SerializeField] private RectTransform _rectTransform;
    public RectTransform rectTransform => _rectTransform;
}
