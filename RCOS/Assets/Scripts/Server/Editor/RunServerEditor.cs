/*
 *  AUTHOR: Sean (alcors@rpi.edu)
 *  DESC: Editor script used to draw custom inspector GUI to include custom buttons.
 */
using UnityEditor;
using UnityEngine;

namespace Webserver
{
    [CustomEditor(typeof(RunServer))]
    public class RunServerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            RunServer script = (RunServer)target;
            DrawDefaultInspector();

            // Creates buttons in the Inspector.
            if (GUILayout.Button("Start Server"))
            {
                script.StartServer();
            }
            if (GUILayout.Button("Stop Server"))
            {
                script.StopServer();
            }
        }
    }
}

