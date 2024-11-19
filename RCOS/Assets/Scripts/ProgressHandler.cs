/*
 *  AUTHOR: Sean (alcors@rpi.edu)
 *  DESC: Helper class used to organize the progress bars for the different players.
 */

using System.Collections.Generic;
using UnityEngine;
using SocketIOClient;
namespace Gameplay
{
    public class ProgressHandler : MonoBehaviour
    {
        [SerializeField] private GameObject _progressBarPrefab;
        [SerializeField] private Transform _container;

        private Dictionary<string, ProgressBarHandler> _progressBars = new Dictionary<string, ProgressBarHandler>();

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

        public void UpdateProgress(string hashedIP, float progress)
        {
            if (!_progressBars.ContainsKey(hashedIP))
            {
                return;
            }

            _progressBars[hashedIP].SetProgress(progress);
        }

        public void SetProgressBarName(string hashedIP, string name)
        {
            if (!_progressBars.ContainsKey(hashedIP))
            {
                return;
            }

            _progressBars[hashedIP].SetName(name);
        }

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

