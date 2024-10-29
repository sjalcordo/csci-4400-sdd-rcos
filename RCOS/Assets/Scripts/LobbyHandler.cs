using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIOClient;
using UnityEngine.Events;

namespace Gameplay
{
    public class LobbyHandler : MonoBehaviour
    {
        private string _lobbyCode;

        // Keep track of the attempt timer (Coroutine)
        private Coroutine _lobbyCoroutine;

        // Public events that are called when creating a lobby succeeds or fails.
        public UnityEvent<string> OnLobbyCreationSuccess = new UnityEvent<string>();
        public UnityEvent OnLobbyCreationFail = new UnityEvent();

        // Start is run as the script is initialized
        public void Start()
        {
            // Adds a listener to when we receive a message from the server.
            Sockets.ServerUtil.manager.OnSocketEvent.AddListener(OnSocket);
        }

        private void OnSocket(string name, SocketIOResponse response)
        {
            // Only listen for verify lobby commands
            if (name != "verify-lobby") return;

            // Stop the timer for creating a lobby.
            StopCoroutine(_lobbyCoroutine);

            // Receive the lobby code from the server.
            _lobbyCode = response.GetValue<string>();
            Debug.Log("Lobby Code: " + _lobbyCode);
            // Invoke the on success event.
            OnLobbyCreationSuccess.Invoke(_lobbyCode);
        }

        public void AttemptCreateLobby()
        {
            // Don't re-attempt if we are already attempting
            if (_lobbyCoroutine != null) return;

            // Tell the server we are creating a lobby.
            Sockets.ServerUtil.manager.SendEvent("create-lobby");
            _lobbyCoroutine = StartCoroutine(WaitForLobbyCreation(10f));
        }

        public void DestroyLobby()
        {
            if (_lobbyCode == "" || _lobbyCode == null) return;

            Sockets.ServerUtil.manager.SendEvent("destroy-lobby", _lobbyCode);
            _lobbyCode = "";
        }

        private IEnumerator WaitForLobbyCreation(float time)
        {
            yield return new WaitForSeconds(time);

            Debug.Log("Failed to create lobby");
            OnLobbyCreationFail.Invoke();
        }
    }
}

