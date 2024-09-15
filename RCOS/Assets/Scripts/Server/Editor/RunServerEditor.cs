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

