using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(LighProbeCubeGenerator))]
public class ObjectBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LighProbeCubeGenerator myScript = (LighProbeCubeGenerator)target;
        if(GUILayout.Button("Build Object"))
        {
            myScript.Execute();
        }
    }
}