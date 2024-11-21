/*
 *  AUTHOR: Sean (alcors@rpi.edu)
 *  DESC: Listens to voting signals from the connected clients and keeps track of the vote, who sent it, and when.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SocketIOClient; // Used to receive signals from SocketIO.

namespace Gameplay
{

    public class VotingHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private LobbyHandler _lobbyHandler;
        [SerializeField] private PresentationHandler _presentationHandler;
        [SerializeField] private GraphHandler _graphHandler;

        [Header("Parameters")]
        [SerializeField] private float _timePerUnit;
        [SerializeField] private int _max;
        [SerializeField] private int _min;

        // The Nested Dictionary is in the form
        // Key: Presenting User's Hashed IP
        // Value:   Key:    Voting Player's Hashed IP
        //          Value:  List of Vote Counts
        private Dictionary<string, Dictionary<string, List<int>> > _voteSections = new Dictionary<string, Dictionary<string, List<int>> >();
        private Coroutine _votingTimer;
        private string _currentPresenter;

        // Start is run as the script is initialized
        private void Start()
        {
            // Adds a listener to when we receive a message from the server.
            Sockets.ServerUtil.manager.OnSocketEvent.AddListener(OnSocket);
        }
        private void OnSocket(string name, SocketIOResponse response)
        {
            // Only listen for verify lobby commands
            if (name != "on-upvote" && name != "on-downvote") return;

            string hashedIP = response.GetValue<string>(0);

            bool invalidPresenter = _currentPresenter == null || _currentPresenter == "";
            bool invalidSection = !_voteSections.ContainsKey(_currentPresenter) || _voteSections[_currentPresenter] == null;
            bool invalidUser = !_voteSections[_currentPresenter].ContainsKey(hashedIP) || _voteSections[_currentPresenter][hashedIP] == null;
            if (invalidPresenter || invalidSection || invalidUser) return;

            int index = _voteSections[_currentPresenter][hashedIP].Count - 1;

            Debug.Log("Received " + name);
            switch (name)
            {
                case "on-upvote":
                    _voteSections[_currentPresenter][hashedIP][index] = Mathf.Clamp(_voteSections[_currentPresenter][hashedIP][index] + 1, _min, _max);
                    break;
                case "on-downvote":
                    _voteSections[_currentPresenter][hashedIP][index] = Mathf.Clamp(_voteSections[_currentPresenter][hashedIP][index] - 1, _min, _max);
                    break;
            }
        }
         
        public void StartVoting()
        {
            _votingTimer = StartCoroutine(CreateVotingUnit());
        }

        public void StopVoting()
        {
            StopCoroutine(_votingTimer);
        }

        public void SendToGraph(string hashedIP)
        {
            if (!_voteSections.ContainsKey(hashedIP)) {
                return;
            }

            _graphHandler.Graph(_voteSections[hashedIP]);
            Debug.Log("Sending to graph");
        }

        private IEnumerator CreateVotingUnit()
        {
            if (_currentPresenter == null || _currentPresenter == "") 
                yield return null;

            if (!_voteSections.ContainsKey(_currentPresenter))
            {
                _voteSections[_currentPresenter] = new Dictionary<string, List<int>>();
            }

            foreach (string hashedIP in _lobbyHandler.hashedIPs)
            {
                if (!_voteSections[_currentPresenter].ContainsKey(hashedIP)) {
                    _voteSections[_currentPresenter][hashedIP] = new List<int>();
                }
                _voteSections[_currentPresenter][hashedIP].Add(0);
            }

            yield return new WaitForSeconds(_timePerUnit);

            _votingTimer = StartCoroutine(CreateVotingUnit());
        }
    }
}

