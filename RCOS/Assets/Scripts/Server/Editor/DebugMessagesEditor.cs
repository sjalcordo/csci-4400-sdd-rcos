using UnityEditor;
using UnityEngine;

namespace Helpers
{
    [CustomEditor(typeof(DebugInspectorMessages))]
    public class DebugMessagesEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DebugInspectorMessages script = (DebugInspectorMessages) target;
            DrawDefaultInspector();
            if (GUILayout.Button("Send Message"))
            {
                script.SendMessage();
            }
        }
    }
}

