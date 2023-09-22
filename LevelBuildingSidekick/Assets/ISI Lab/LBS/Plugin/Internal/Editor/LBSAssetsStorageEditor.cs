using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LBSAssetsStorage))]
public class LBSAssetsStorageEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(20);

        if(GUILayout.Button("Search all in Project"))
        {
            SearchAllInProject();
        }
    }

    private void SearchAllInProject()
    {
        var storage = (target as LBSAssetsStorage);
        storage.Clear();

        var SOs = Utility.DirectoryTools.GetScriptables<ScriptableObject>();
        foreach (var s in SOs)
        {
            storage.AddElement(s);
        }

        EditorUtility.SetDirty(storage);
        AssetDatabase.SaveAssets();
    }
}

public static class StorageExtension
{
    public static void SearchInProject(this LBSAssetsStorage storage)
    {
        storage.Clear();

        var SOs = Utility.DirectoryTools.GetScriptables<ScriptableObject>();
        foreach (var s in SOs)
        {
            storage.AddElement(s);
        }

        EditorUtility.SetDirty(storage);
        AssetDatabase.SaveAssets();
    }
}