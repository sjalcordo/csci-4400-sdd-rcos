/*
 *  AUTHOR: Sean (alcors@rpi.edu)
 *  DESC: Used to send messages directly through the socket for testing purposes.
 */

using UnityEngine;

namespace Helpers
{
    public class DebugInspectorMessages : MonoBehaviour
    {
        // Inspector-Exposed Private variables.
        [SerializeField] private string _eventName;
        [SerializeField] private string _data;
        [SerializeField] private string _data2;

        /// <summary>
        /// Sends an image to the socket with an option 2nd parameter.
        /// </summary>
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

