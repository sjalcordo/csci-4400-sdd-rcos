/*
 *  AUTHOR: Sean (alcors@rpi.edu)
 *  DESC: Helper class used to organize the progress bars for the different players.
 */

using System.Collections.Generic;
using UnityEngine;
namespace Gameplay
{
    public class ProgressHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject _progressBarPrefab;
        [SerializeField] private Transform _container;

        // Private Members
        private Dictionary<string, ProgressBarHandler> _progressBars = new Dictionary<string, ProgressBarHandler>();

        /// <summary>
        /// This will add a progress bar to the container.
        /// </summary>
        public void AddProgressBar(string hashedIP)
        {
            if (_progressBars.ContainsKey(hashedIP))
            {
                return;
            }

            GameObject progressBarObj = Instantiate(_progressBarPrefab);
            progressBarObj.transform.parent = _container;
            _progressBars[hashedIP] = progressBarObj.GetComponent<ProgressBarHandler>();
        }

        /// <summary>
        /// This will update the progress of the bar that is tied to the given hashedIP.
        /// </summary>
        public void UpdateProgress(string hashedIP, float progress)
        {
            if (!_progressBars.ContainsKey(hashedIP))
            {
                return;
            }

            _progressBars[hashedIP].SetProgress(progress);
        }

        /// <summary>
        /// This will update the name of the progress bar tied to the given hashedIP.
        /// </summary>
        public void SetProgressBarName(string hashedIP, string name)
        {
            if (!_progressBars.ContainsKey(hashedIP))
            {
                return;
            }

            _progressBars[hashedIP].SetName(name);
        }

        /// <summary>
        /// This will set the profile picture's texture of the progress bar tied to the given hashedIP.
        /// </summary>
        public void SetProgressBarPfp(string hashedIP, Texture texture)
        {
            if (!_progressBars.ContainsKey(hashedIP))
            {
                return;
            }

            _progressBars[hashedIP].SetPfp(texture);
        }
    }
}

