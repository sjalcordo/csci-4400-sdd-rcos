/*
 *  AUTHOR: Sean (alcors@rpi.edu)
 *  DESC: Runs an executable that holds the webserver that is used for sending files to connected
 *        webclients and the connection through Socket.IO.
 */

using System.Diagnostics; // Used to open an external program.
using UnityEngine;

namespace Webserver
{
    public class RunServer : MonoBehaviour
    {
        [Tooltip("The path of the server, originating at source folder.")]
        [SerializeField] private string _serverPath;
        [Tooltip("The name of the server application.")]
        [SerializeField] private string _applicationName;
        [Tooltip("Whether to hide the window of the server or show the terminal.")]
        [SerializeField] private bool _hideServer;
        [Tooltip("If start server is called but a server is already running, should we kill it.")]
        [SerializeField] private bool _killExistingServer;


        // The process of the server running, used to close as well.
        private Process _serverProcess;

        /// <summary>
        /// This starts the server if possible. If a server already exists, then it will kill the server if that is set, or it will do nothing.
        /// </summary>
        public void StartServer()
        {
            // If we aren't set to kill the server but it already exists, do nothing.
            if (_serverProcess != null && !_killExistingServer) {
                return;
            }
            // Otherwise, kill the server before starting the process of creating a new one.
            if (_serverProcess != null)
            {
                _serverProcess.Kill();
            }

            _serverProcess = new Process();

            // Set the server path
            _serverProcess.StartInfo.FileName = Application.streamingAssetsPath + "/" + _serverPath + "/" + _applicationName;
            // Sets the working directory properly. 
            _serverProcess.StartInfo.WorkingDirectory = Application.streamingAssetsPath + "/" + _serverPath;
            // Sets to use shell execution.
            _serverProcess.StartInfo.UseShellExecute = true;
            if (_hideServer)
                _serverProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            _serverProcess.Start();
        }

        /// <summary>
        /// This will stop an existing server.
        /// </summary>
        public void StopServer()
        {
            if (_serverProcess == null) return;

            _serverProcess.Kill();
        }

        public void ConnectToServer()
        {
            Sockets.ServerUtil.manager.Connect();
        }

        private void OnApplicationQuit()
        {
            StopServer();
        }
    }
}

