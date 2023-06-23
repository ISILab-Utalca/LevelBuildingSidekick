using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SchemaHCLog)), CanEditMultipleObjects]
public class SchemaHCLog_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Export .CSV"))
        {
            (target as SchemaHCLog).ExportCSV();
        }
        GUILayout.Space(20);
        base.OnInspectorGUI();


    }
}
