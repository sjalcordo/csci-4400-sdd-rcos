using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Gameplay
{
    public class GraphHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private LobbyHandler _lobbyHandler;
        [Space(5)]
        [SerializeField] private TMP_Text _name;
        [Space(5)]
        [SerializeField] private RectTransform _container;
        [SerializeField] private GameObject _voteIconPrefab;
        [Header("Parameters")]
        [SerializeField] private Color[] _possibleColors;
        [SerializeField] private Color _averageColor;

        private Dictionary<string, Color> _colors = new Dictionary<string, Color>();
        private Dictionary<string, float> _playerAverages = new Dictionary<string, float>();
        public Dictionary<string, float> playerAverages => _playerAverages;
        private List<int> _currentSums = new List<int>();

        public void GetPlayerColors()
        {
            List<Color> availableColors = new List<Color>(_possibleColors);

            int index;
            foreach (string user in _lobbyHandler.hashedIPs)
            {
                index = Random.Range(0, availableColors.Count);
                _colors[user] = availableColors[index];
                availableColors.RemoveAt(index);
            }
        }

        public void Graph(string hashedIP, Dictionary<string, List<int>> votes)
        {
            for (int i = _container.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(_container.transform.GetChild(i).gameObject);
            }

            string firstKey = "";
            foreach (string key in votes.Keys)
            {
                firstKey = key;
                break;
            }

            if (firstKey == "")
            {
                return;
            }

            _name.text = _lobbyHandler.names[hashedIP];

            for (int i = 0; i < votes[firstKey].Count; i++)
            {
                _currentSums.Add(0);
            }

            foreach(KeyValuePair<string, List<int>> pair in votes)
            {
                Color playerColor = _colors[pair.Key];
                for (int i = 0; i < pair.Value.Count; i++)
                {
                    SetupAndPlaceVoteIcon(playerColor, i, pair.Value[i], pair.Value.Count);
                    _currentSums[i] += pair.Value[i];
                }
            }

            float average = 0;
            for (int i = 0; i < _currentSums.Count; i++)
            {
                average += _currentSums[i];
                SetupAndPlaceVoteIcon(_averageColor, i, _currentSums[i] / votes.Count, _currentSums.Count);
            }
            average /= (float) _currentSums.Count;
            _playerAverages[hashedIP] = average;
        }

        private void SetupAndPlaceVoteIcon(Color color, int column, float value, int columnCount)
        {
            GameObject voteIconObj = Instantiate(_voteIconPrefab);
            voteIconObj.transform.SetParent(_container.transform);
            VoteIcon voteIcon = voteIconObj.GetComponent<VoteIcon>();
            voteIcon.image.color = color;
            voteIcon.rectTransform.anchoredPosition = new Vector2(column * (_container.sizeDelta.x / (columnCount - 1)), _container.sizeDelta.y / 2f + value * (_container.sizeDelta.y / 10f));
        }
    }
}

