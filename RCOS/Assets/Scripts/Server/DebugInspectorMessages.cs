using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Helpers
{
    public class DebugInspectorMessages : MonoBehaviour
    {
        [SerializeField] private string _eventName;
        [SerializeField] private string _data;
        [SerializeField] private string _data2;

        public void SendMessage()
        {
            if (_data2 != "")
            {
                Sockets.ServerUtil.manager.SendEvent(_eventName, _data, _data2);
                return;
            } 
            Sockets.ServerUtil.manager.SendEvent(_eventName, _data);
        }
    }
}

