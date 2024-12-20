/*
 *  AUTHOR: Sean (alcors@rpi.edu)
 *  DESC: Class that manages the current lobby, connected players, their icons, and their setup.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIOClient;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Gameplay
{
    // Class that stores the name and texture of a connected player.
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
        [SerializeField] private ProgressHandler _progressHandler;

        // Reference Prefabs for lobby handling.
        [SerializeField] private GameObject _playerIconPrefab;
        [SerializeField] private GameObject _playerContainer;
        [SerializeField] private Button _startButton;

        [Space(8)]
        [SerializeField] private float _connectTime = 2f;

        private string _lobbyCode;

        // Keep track of the attempt timer (Coroutine)
        private Coroutine _lobbyCoroutine;
        // Keep track of the hashedIPs, names, textures, and icons of each player.
        private List<string> _hashedIPs = new List<string>();
        public List<string> hashedIPs => _hashedIPs;
        // Keep track of the names for each user.
        private Dictionary<string, string> _names = new Dictionary<string, string>();
        public Dictionary<string, string> names => _names;
        // Keep track of the b64 strings for each user.
        private Dictionary<string, string> _b64Textures = new Dictionary<string, string>();
        public Dictionary<string, string> b64Textures => _b64Textures;
        // Keep track of the player icons for each user.
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

        /// <summary>
        /// Called when each event is received from the socket manager.
        /// Includes "verify-lobby", "new-player", "on-set-name", "on-set-pfp", "request-player-info".
        /// </summary>
        /// <param name="name">The name of the event passed.</param>
        /// <param name="response">The data given with the response.</param>
        private void OnSocket(string name, SocketIOResponse response)
        {
            // Only listen for verify lobby commands
            if (name != "verify-lobby" && name != "new-player" && name != "on-set-name" && name != "on-set-pfp" && name != "request-player-info" && name != "on-request-player-b64") return;

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
                case "request-player-info":
                    GetPlayerInfo();
                    break;
                case "on-request-player-b64":
                    string user = response.GetValue<string>(0);
                    Sockets.ServerUtil.manager.SendEvent("send-player-b64", user, b64Textures[user]);
                    break;
            }
        }

        /// <summary>
        /// Goes through every player to create a name and b64 array and sends that to the server.
        /// </summary>
        private void GetPlayerInfo()
        {
            List<string> names = new List<string>();
            List<string> b64 = new List<string>();

            foreach (string hashedIP in _hashedIPs)
            {
                if (!_names.ContainsKey(hashedIP)) { continue; }
                string[] playerArray = new string[2];
                names.Add(_names[hashedIP]);
                b64.Add(_b64Textures[hashedIP]);
            }
            Sockets.ServerUtil.manager.SendEvent("on-request-player-info", names.ToArray(), b64.ToArray());
        }

        /// <summary>
        /// Called when the server returns a lobby code.
        /// </summary>
        /// <param name="lobbyCode"></param>
        private void OnLobbyVerification(string lobbyCode)
        {
            // Stop the timer for creating a lobby.
            StopCoroutine(_lobbyCoroutine);

            // Receive the lobby code from the server.
            _lobbyCode = lobbyCode;

            // Invoke the on success event.
            OnLobbyCreationSuccess.Invoke(_lobbyCode);
        }

        /// <summary>
        /// Called to setup a new player object without a name or pfp.
        /// </summary>
        /// <param name="hashedIP"></param>
        private void OnPlayerSetup(string hashedIP)
        {
            if (!_hashedIPs.Contains(hashedIP))
            {
                GameObject IconObj = GameObject.Instantiate(_playerIconPrefab, _playerContainer.transform);

                PlayerIcon playerIcon = IconObj.GetComponent<PlayerIcon>();
                playerIcon.SetHashedIP(hashedIP);
                playerIcon.SetLobbyHandler(this);

                _playerIcons[hashedIP] = playerIcon;

                _hashedIPs.Add(hashedIP);

                _progressHandler.AddProgressBar(hashedIP);
            }
        }

        /// <summary>
        /// Called to set the player name and add it to the respective dictionaries.
        /// If enough players are connncted it will activate the start button.
        /// </summary>
        /// <param name="hashedIP"></param>
        /// <param name="name"></param>
        private void OnPlayerSetName(string hashedIP, string name)
        {
            if (!_hashedIPs.Contains(hashedIP))
            {
                return;
            }

            _names[hashedIP] = name;
            _playerIcons[hashedIP].SetName(name);
            _progressHandler.SetProgressBarName(hashedIP, name);
            if (_hashedIPs.Count > 1)
            {
                _startButton.interactable = true;
            }
        }

        /// <summary>
        /// Adds the player's PFP and updates the progress bar.
        /// </summary>
        /// <param name="hashedIP"></param>
        /// <param name="b64"></param>
        private void OnPlayerSetPfp(string hashedIP, string b64)
        {
            if (!_hashedIPs.Contains(hashedIP))
            {
                return;
            }

            Texture texture = b64toTex.convert(b64);
            _b64Textures[hashedIP] = b64;
            _playerIcons[hashedIP].SetPfp(texture);
            _progressHandler.SetProgressBarPfp(hashedIP, texture);
        }

        /// <summary>
        /// Attempts to create a lobby and waits to receive verification.
        /// </summary>
        public void AttemptCreateLobby()
        {
            // Don't re-attempt if we are already attempting
            if (_lobbyCoroutine != null) return;

            // Tell the server we are creating a lobby.
            Sockets.ServerUtil.manager.SendEvent("create-lobby");
            _lobbyCoroutine = StartCoroutine(WaitForLobbyCreation(_connectTime));
        }

        /// <summary>
        /// Destroys the lobby and clears the lobby code.
        /// </summary>
        public void DestroyLobby()
        {
            if (_lobbyCode == "" || _lobbyCode == null) return;

            Sockets.ServerUtil.manager.SendEvent("destroy-lobby", _lobbyCode);
            _lobbyCode = "";
        }

        /// <summary>
        /// Removes the player from the dictionaries and list, and sends to the server to remove the player.
        /// </summary>
        /// <param name="hashedIP"></param>
        public void RemovePlayer(string hashedIP)
        {
            Sockets.ServerUtil.manager.SendEvent("remove-player", hashedIP);
            _hashedIPs.Remove(hashedIP);
            _names.Remove(hashedIP);
            _b64Textures.Remove(hashedIP);
            _playerIcons.Remove(hashedIP);
        }

        private IEnumerator WaitForLobbyCreation(float time)
        {
            yield return new WaitForSeconds(time);

            Debug.Log("Failed to create lobby");
            OnLobbyCreationFail.Invoke();
        }
    }
}

