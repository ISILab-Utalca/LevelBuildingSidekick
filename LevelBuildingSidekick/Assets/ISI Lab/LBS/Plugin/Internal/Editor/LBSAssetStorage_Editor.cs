using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[LBSCustomEditor(typeof(LBSAssetsStorage))]
public class LBSAssetsStorage_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(20);

        var storage = target as LBSAssetsStorage;
        if(GUILayout.Button("Search all in Project"))
        {
            storage.SearchAllInProject();
        }
    }
}