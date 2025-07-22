using UnityEditor;
using UnityEngine;

namespace LBS.Bundles.Tools.Editor
{
    [CustomEditor(typeof(ScatterArea))]
    public class ScatterAreaEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            ScatterArea component = (ScatterArea)target;

            // Add a custom button
            if (GUILayout.Button("Run Scatter Area"))
            {
                // Call a method in the target component when the button is clicked
                component.RunCommand();
            }
        }
    }
}
