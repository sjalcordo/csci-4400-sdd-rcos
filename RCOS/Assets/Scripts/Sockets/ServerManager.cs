/*
 *  AUTHOR: Sean (alcors@rpi.edu)
 *  DESC: Public static helper class ServerUtil as well as the ServerManager which handles the connection to the SocketIO server.
 */
using System.Collections.Generic;
using UnityEngine;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;
using UnityEngine.Events;

namespace Sockets
{
    // Used to reference the ServerManager without needing to couple with a specific reference.
    public static class ServerUtil
    {
        private static ServerManager _instance;
        // If the manager does not exist, attempt to find it and if we cannot find it, create one and store the reference to it.
        public static ServerManager manager
        {
            get
            {
                if (_instance == null)
                {
                    GameObject serverManagerObj = GameObject.Find("ServerManager");
                    if (serverManagerObj == null)
                    {
                        serverManagerObj = new GameObject("ServerManager");
                        _instance = serverManagerObj.AddComponent<ServerManager>();
                        _instance.Connect();
                    }
                    else
                    {
                        _instance = serverManagerObj.GetComponent<ServerManager>();
                    }
                }
                return _instance;
            }
        }
    }

    [System.Serializable]
    public class SocketEvent : UnityEvent<string, SocketIOResponse> {}

    public class ServerManager : MonoBehaviour
    {
        // Private variables exposed to the inspector.
        [Header("Parameters")]
        [SerializeField] private string _serverURI = "http://companionship.cs.rpi.edu:3000";
        [SerializeField] private bool _useLocalHost;
        [SerializeField] private bool _joinOnStart;

        // Socket and a read-only accessor.
        private SocketIOUnity _socket;
        public SocketIOUnity socket => _socket;

        // Public events for the publisher-subscriber pattern.
        public SocketEvent OnSocketEvent;
        public UnityEvent OnDisconnect;

        private void Awake()
        {
            // Mark this object to not be destroyed upon scene swapping.
            //DontDestroyOnLoad(gameObject);

            // Depending on if we use the local host or not, connect using a localhost URL or the given URI.
            string uri = _useLocalHost ? "http://127.0.0.1:3000" : _serverURI;

            // Creates a new socket with a Unity Token.
            _socket = new SocketIOUnity(uri, new SocketIOOptions
            {
                Query = new Dictionary<string, string>
                {
                    {"token", "UNITY" }
                }
                ,
                EIO = 4
                ,
                Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
            });
            _socket.JsonSerializer = new NewtonsoftJsonSerializer();

            // reserved Socket IO events
            _socket.OnConnected += (sender, e) =>
            {
                Debug.Log("socket.OnConnected");
            };

            socket.OnAnyInUnityThread((name, response) =>
            {
                Debug.Log("Received Message:\n\tName:" + name + "\n\tData:" + response);
                OnSocketEvent.Invoke(name, response);
            });

            _socket.OnDisconnected += (sender, e) =>
            {
                Debug.Log("disconnect: " + e);
            };
        } // Awake

        private void Start()
        {
            if (_joinOnStart)
            {
                Connect();
            }
        }

        /// <summary>
        /// Connects the socket to the given URI.
        /// </summary>
        public void Connect()
        {
            _socket.Connect();
        }

        /// <summary>
        /// Sends an event with only its name and null parameters.
        /// </summary>
        public void SendEvent(string eventName)
        {
            _socket.Emit(eventName);
        }

        /// <summary>
        /// Seds an event with its name and an array of parameters.
        /// </summary>
        public void SendEvent(string eventName, params object[] data)
        {
            _socket.Emit(eventName, data);
        }

        // This is a Unity Message called when the object or script is destroyed.
        private void OnDestroy()
        {
            OnDisconnect.Invoke();
            _socket.Disconnect();
        }
    }
}

