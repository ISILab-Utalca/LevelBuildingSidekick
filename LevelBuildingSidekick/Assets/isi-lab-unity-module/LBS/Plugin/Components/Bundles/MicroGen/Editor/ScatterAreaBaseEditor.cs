using UnityEditor;
using UnityEngine;

namespace LBS.Bundles.Tools.Editor
{
    [CustomEditor(typeof(ScatterAreaBase), true)]
    public class ScatterAreaBaseEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            ScatterAreaBase component = (ScatterAreaBase)target;

            // Add a custom button
            if (GUILayout.Button("Run Scatter Tool"))
            {
                // Call a method in the target component when the button is clicked
                component.RunCommand();
            }
        }
    }
}
