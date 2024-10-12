using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;

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
                    GameObject serverManagerObj = new GameObject("Server Manager");
                    _instance = serverManagerObj.AddComponent<ServerManager>();
                }
                return _instance;
            }
        }
    }

    public class ServerManager : MonoBehaviour
    {
        [SerializeField] private string _uri = "http://companionship.cs.rpi.edu:3000";
        [SerializeField] private bool _joinOnStart;
        private SocketIOUnity _socket;
        public SocketIOUnity socket => _socket;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            _socket = new SocketIOUnity(_uri, new SocketIOOptions
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
            _socket.OnPing += (sender, e) =>
            {
                Debug.Log("Ping");
            };
            _socket.OnPong += (sender, e) =>
            {
                Debug.Log("Pong: " + e.TotalMilliseconds);
            };
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
    }
}

