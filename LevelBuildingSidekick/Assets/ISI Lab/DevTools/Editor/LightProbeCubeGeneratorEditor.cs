using UnityEngine;
using ISILab.LBS.Generators;
using UnityEditor;

[CustomEditor(typeof(LightProbeCubeGenerator))]
public class ObjectBuilderEditor : Editor
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
