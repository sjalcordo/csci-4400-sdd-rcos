using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIOClient;
using UnityEngine.Events;

namespace Gameplay
{
    public class Player
    {
        public string name;
        public Texture texture;

        public Player() { }
        public Player(string name)
        {
            this.name = name;
        }
    }

    public class LobbyHandler : MonoBehaviour
    {
        [SerializeField] private GameObject _playerIconPrefab;
        [SerializeField] private GameObject _playerContainer;

        [Space(8)]
        private string _lobbyCode;

        // Keep track of the attempt timer (Coroutine)
        private Coroutine _lobbyCoroutine;

        private Dictionary<string, Player> _players = new Dictionary<string, Player>();
        public Dictionary<string, Player> players => _players;

        private Dictionary<string, PlayerIcon> _playerIcons = new Dictionary<string, PlayerIcon>();

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
            if (name != "verify-lobby" && name != "new-player" && name != "on-set-name" && name != "on-set-pfp") return;

            switch (name)
            {
                case "verify-lobby":
                    OnLobbyVerification(response.GetValue<string>());
                    break;
                case "new-player":
                    OnPlayerSetup(response.GetValue<string>(0));
                    break;
                case "on-set-name":
                    OnPlayerSetName(response.GetValue<string>(0), response.GetValue<string>(1));
                    break;
                case "on-set-pfp":
                    OnPlayerSetPfp(response.GetValue<string>(0), response.GetValue<string>(1));
                    break;
            }
        }

        private void OnLobbyVerification(string lobbyCode)
        {
            // Stop the timer for creating a lobby.
            StopCoroutine(_lobbyCoroutine);

            // Receive the lobby code from the server.
            _lobbyCode = lobbyCode;
            Debug.Log("Lobby Code: " + _lobbyCode);
            // Invoke the on success event.
            OnLobbyCreationSuccess.Invoke(_lobbyCode);
        }

        private void OnPlayerSetup(string hashedIP)
        {
            if (!_players.ContainsKey(hashedIP))
            {
                GameObject IconObj = GameObject.Instantiate(_playerIconPrefab, _playerContainer.transform);
                PlayerIcon playerIcon = IconObj.GetComponent<PlayerIcon>();
                _playerIcons[hashedIP] = playerIcon;
            }
            _players[hashedIP] = new Player();
            Debug.Log("New Player: " + hashedIP);
        }

        private void OnPlayerSetName(string hashedIP, string name)
        {
            if (!_players.ContainsKey(hashedIP))
            {
                return;
            }
            _playerIcons[hashedIP].SetName(name);
        }

        private void OnPlayerSetPfp(string hashedIP, string b64)
        {
            if (!_players.ContainsKey(hashedIP))
            {
                return;
            }
            _players[hashedIP].texture = b64toTex.convert(b64);
            _playerIcons[hashedIP].SetPfp(b64toTex.convert(b64));
        }

        public void AttemptCreateLobby()
        {
            // Don't re-attempt if we are already attempting
            if (_lobbyCoroutine != null) return;

            // Tell the server we are creating a lobby.
            Sockets.ServerUtil.manager.SendEvent("create-lobby");
            _lobbyCoroutine = StartCoroutine(WaitForLobbyCreation(2f));
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

