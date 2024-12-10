/*
 *  AUTHOR: Sean (alcors@rpi.edu)
 *  DESC: Handles the creation of the graph, tracking the player averages, and placing the graph points.
 */
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
        [SerializeField] private VotingHandler _votingHandler;
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

        /// <summary>
        /// Gets the player colors from the list and ensures there are no duplicate colors.
        /// Stores this into a dictionary for each user.
        /// </summary>
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

        /// <summary>
        /// Graphs the votes given and ties it to the given user.
        /// </summary>
        /// <param name="presenterID"></param>
        /// <param name="votes"></param>
        public void Graph(string presenterID, Dictionary<string, List<int>> votes)
        {
            // Clears the graph
            for (int i = _container.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(_container.transform.GetChild(i).gameObject);
            }

            // Used to get the size of the votes
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

            // Sets the graph name
            _name.text = _lobbyHandler.names[presenterID];

            // Sets up the sums
            List<int> currentSums = new List<int>();
            for (int i = 0; i < votes[firstKey].Count; i++)
            {
                currentSums.Add(0);
            }

            // Places the graph icons for each user.
            foreach(KeyValuePair<string, List<int>> pair in votes)
            {
                if (pair.Key == presenterID)
                {
                    continue;
                }

                Color playerColor = _colors[pair.Key];
                for (int i = 0; i < pair.Value.Count; i++)
                {
                    SetupAndPlaceVoteIcon(playerColor, i, pair.Value[i], pair.Value.Count);
                    currentSums[i] += pair.Value[i];
                }
            }

            // Places the average graph icons.
            float average = 0;
            for (int i = 0; i < currentSums.Count; i++)
            {
                average += currentSums[i];
                SetupAndPlaceVoteIcon(_averageColor, i, currentSums[i] / (votes.Count - 1), currentSums.Count);
            }
            average /= (float) currentSums.Count;
            // Stores the average
            _playerAverages[presenterID] = average;
        } // Graph

        /// <summary>
        /// Helper method used to easily place graph icons..
        /// </summary>
        /// <param name="color"></param>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <param name="columnCount"></param>
        private void SetupAndPlaceVoteIcon(Color color, int column, float value, int columnCount)
        {
            GameObject voteIconObj = Instantiate(_voteIconPrefab);
            voteIconObj.transform.SetParent(_container.transform);
            VoteIcon voteIcon = voteIconObj.GetComponent<VoteIcon>();
            voteIcon.image.color = color;
            voteIcon.rectTransform.anchoredPosition = new Vector2(column * (_container.sizeDelta.x / (columnCount - 1)), _container.sizeDelta.y / 2f + value * (_container.sizeDelta.y / (float) (_votingHandler.max - _votingHandler.min)));
        }
    }
}

