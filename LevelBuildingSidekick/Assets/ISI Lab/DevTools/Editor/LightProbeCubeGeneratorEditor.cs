using ISILab.LBS.Generators;
using UnityEditor;
using UnityEngine;

namespace ISI_Lab.DevTools.Editor
{
    [CustomEditor(typeof(LightProbeCubeGenerator))]
    public class ObjectBuilderEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            LightProbeCubeGenerator myScript = (LightProbeCubeGenerator)target;
            if(GUILayout.Button("Build Object"))
            {
                myScript.Execute();
            }
        }
    }
}   
