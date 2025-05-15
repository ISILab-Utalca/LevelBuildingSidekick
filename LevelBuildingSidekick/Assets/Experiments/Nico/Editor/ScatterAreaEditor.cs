using UnityEditor;
using UnityEngine;

namespace Experiments.Nico
{
    [CustomEditor(typeof(ScatterArea))]
    public class ScatterAreaEditor : Editor
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
