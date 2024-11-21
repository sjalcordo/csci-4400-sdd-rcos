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
        private List<int> _currentSums;

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

        public void Graph(Dictionary<string, List<int>> votes)
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

            for (int i = 0; i < _currentSums.Count; i++)
            {
                SetupAndPlaceVoteIcon(_averageColor, i, _currentSums[i], _currentSums.Count);
            }
        }

        private void SetupAndPlaceVoteIcon(Color color, int column, int value, int columnCount)
        {
            GameObject voteIconObj = Instantiate(_voteIconPrefab);
            voteIconObj.transform.parent = _container.transform;
            VoteIcon voteIcon = voteIconObj.GetComponent<VoteIcon>();
            voteIcon.image.color = color;
            voteIcon.rectTransform.anchoredPosition = new Vector2(column * (_container.sizeDelta.x / (columnCount - 1)), _container.sizeDelta.y / 2f + value * (_container.sizeDelta.y / 10f));
        }
    }
}

