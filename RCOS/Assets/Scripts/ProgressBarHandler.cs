/*
 *  AUTHOR: Michael (zhangy90@rpi.edu)
 *  DESC: Class used to manage the progress bar of each player.
 */

using UnityEngine;
using UnityEngine.UI;

public class ProgressBarHandler : MonoBehaviour
{
    private Slider slider;
    private ParticleSystem particles;
    public GameObject particleChild;
    public float FillSpeed = 0.5f;
    public float fillTarget = 0.0f;
    private float targetProgress = 0;

    private void Awake() {
        slider = gameObject.GetComponent<Slider>();
        particles = particleChild.GetComponent<ParticleSystem>();
    }
    // Start is called before the first frame update
    void Start()
    {
        setProgress(fillTarget);
    }

    // Update is called once per frame
    void Update()
    {
        if (slider.value < targetProgress) {
           slider.value += FillSpeed * Time.deltaTime;  
           if (!particles.isPlaying)
            particles.Play();
        }
        else {
            particles.Stop();
        }
 
    }

    //Add incremental progress to the bar until targetProgress
    public void setProgress(float newProgress) {
        targetProgress = slider.value + newProgress;
    }
}
