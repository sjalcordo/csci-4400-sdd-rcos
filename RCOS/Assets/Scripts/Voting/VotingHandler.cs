using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIOClient;
using UnityEngine.Events;

namespace Gameplay
{
    public class VotingHandler : MonoBehaviour
    {

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

            Debug.Log("Vote registered at " + Time.time + " : " + response.GetValue<string>());
        }
    }
}

