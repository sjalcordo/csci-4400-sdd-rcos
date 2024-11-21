/*
 *  AUTHOR: Michael (zhangy90@rpi.edu)
 *  DESC: Class used to manage the progress bar of each player.
 */

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressBarHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text _name;
    [SerializeField] private RawImage _pfp;
    [Space(5)]
    [SerializeField] private Slider _slider;
    [SerializeField] private GameObject _particleChild;

    [Header("Parameters")]
    [SerializeField] private float targetProgress = 0;
    [SerializeField] float FillSpeed = 0.5f;

    private ParticleSystem particles;

    private void Awake() {
        particles = _particleChild.GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (_slider.value < targetProgress) {
           _slider.value += FillSpeed * Time.deltaTime;  
           if (!particles.isPlaying)
            particles.Play();
        }
        else {
            particles.Stop();
        }
 
    }

    //Add incremental progress to the bar until targetProgress
    public void AddProgress(float delta) {
        targetProgress = _slider.value + delta;
    }

    public void SetProgress(float newProgress)
    {
        targetProgress = newProgress;
    }

    public void SetName(string name)
    {
        _name.text = name;
    }

    public void SetPfp(Texture texture)
    {
        _pfp.texture = texture;
    }
}
