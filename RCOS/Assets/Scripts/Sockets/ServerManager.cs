using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;
using UnityEngine.Events;

namespace Sockets
{
    public static class ServerUtil
    {
        private static ServerManager _instance;
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
        [SerializeField] private string _serverURI = "http://companionship.cs.rpi.edu:3000";
        [SerializeField] private bool _useLocalHost;
        [SerializeField] private bool _joinOnStart;
        private SocketIOUnity _socket;
        public SocketIOUnity socket => _socket;

        public SocketEvent OnSocketEvent;
        public UnityEvent OnDisconnect;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            string uri = _useLocalHost ? "http://127.0.0.1:3000" : _serverURI;

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

            ///// reserved socketio events
            _socket.OnConnected += (sender, e) =>
            {
                Debug.Log("socket.OnConnected");
            };

            socket.OnAnyInUnityThread((name, response) =>
            {
                OnSocketEvent.Invoke(name, response);
            });

            _socket.OnDisconnected += (sender, e) =>
            {
                Debug.Log("disconnect: " + e);
            };
        }

        private void Start()
        {
            if (_joinOnStart)
            {
                Connect();
            }
        }

        public void Connect()
        {
            _socket.Connect();
        }

        public void SendEvent(string eventName)
        {
            _socket.Emit(eventName);
        }

        public void SendEvent(string eventName, params object[] data)
        {
            _socket.Emit(eventName, data);
        }

        private void OnDestroy()
        {
            OnDisconnect.Invoke();
            _socket.Disconnect();
        }
    }
}

