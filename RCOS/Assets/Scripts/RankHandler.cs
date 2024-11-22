/*
 *  AUTHOR: Sean (alcors@rpi.edu)
 *  DESC: Handles the ranking of each players and starts the post game.
 */

using Menus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SocketIOClient;

namespace Gameplay {
    public class RankHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GraphHandler _graphHandler;
        [SerializeField] private MenuHandler _menuHandler;
        [SerializeField] private LobbyHandler _lobbyHandler;
        [Space(5)]
        [SerializeField] private TMP_Text _nameTextObj;
        [SerializeField] private RawImage _pfpObj;

        private KeyValuePair<string, float>[] _orderedPlayers;

        private void Start()
        {
            // Adds a listener to when we receive a message from the server.
            Sockets.ServerUtil.manager.OnSocketEvent.AddListener(OnSocket);
        }

        /// <summary>
        /// Called when each event is received from the socket manager.
        /// Includes "on-get-rank".
        /// </summary>
        /// <param name="name">The name of the event passed.</param>
        /// <param name="response">The data given with the response.</param>
        private void OnSocket(string name, SocketIOResponse response)
        {
            // Only listen for verify lobby commands
            if (name != "on-get-rank") return;

            string hashedIP = response.GetValue<string>();
            if (_orderedPlayers == null || _orderedPlayers.Length <= 0)
            {
                Sockets.ServerUtil.manager.SendEvent("send-rank", hashedIP, -1);
                return;
            }

            for (int i = 0; i < _orderedPlayers.Length; i++)
            {
                if (hashedIP == _orderedPlayers[i].Key)
                {
                    Sockets.ServerUtil.manager.SendEvent("send-rank", hashedIP, i + 1);
                    return;
                }
            }

            Sockets.ServerUtil.manager.SendEvent("send-rank", hashedIP, -1);
        }

        /// <summary>
        /// Orders the players, sets the first player, moves to the post game menu, and sends the "start-post-game" signal to the server.
        /// </summary>
        public void StartPostGame()
        {
            _orderedPlayers = GetOrderedPlayers(_graphHandler.playerAverages);
            KeyValuePair<string, float> first = _orderedPlayers[0];
            _nameTextObj.text = _lobbyHandler.names[first.Key];
            _pfpObj.texture = b64toTex.convert(_lobbyHandler.b64Textures[first.Key]);
            _menuHandler.SwitchState(EMenuState.PostGame);
            Sockets.ServerUtil.manager.SendEvent("start-post-game");
        }

        /// <summary>
        /// Orders the players based on their average values, reverses it, and returns it as an array.
        /// </summary>
        /// <param name="sums"></param>
        /// <returns></returns>
        private KeyValuePair<string, float>[] GetOrderedPlayers(Dictionary<string, float> sums)
        {
            if (sums.Count == 0)
            {
                return null;
            }

            List<KeyValuePair<string, float>> sortedList = new List<KeyValuePair<string, float>>(sums);
            sortedList.Sort(
                delegate (KeyValuePair<string, float> firstPair,
                KeyValuePair<string, float> nextPair)
                {
                    return firstPair.Value.CompareTo(nextPair.Value);
                }
            );

            sortedList.Reverse();

            return sortedList.ToArray();
        }
    }
}

