using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class LBS_AssetsPostProcessor : AssetPostprocessor
{
    public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        OnPostImportProcess(importedAssets);
        OnPostDeleteProcess(deletedAssets);
        OnPostMoveProcess(movedAssets);
        OnPostMoveFromPathsProcess(movedFromAssetPaths);
    }

    public static void OnPostImportProcess(string[] importedAssets)
    {
        var storage = LBSAssetsStorage.Instance;

        foreach (var asset in importedAssets)
        {
            if (asset.Contains(".meta"))
                continue;

            if (!asset.Contains(".asset"))
                continue;

            var obj = AssetDatabase.LoadAssetAtPath<ScriptableObject>(asset);

            if( obj == null)
                continue;

            storage.AddElement(obj);
        }

        AssetDatabase.SaveAssets();
    }


    public static void OnPostDeleteProcess(string[] deletedAssets)
    {
        var storage = LBSAssetsStorage.Instance;

        foreach (var asset in deletedAssets)
        {
            if (asset.Contains(".meta"))
                return;

            if (!asset.Contains(".asset"))
                return;
        }

        storage.CleanAllEmpties();

        AssetDatabase.SaveAssets();
    }

    public static void OnPostMoveProcess(string[] movedAssets)
    {
        // do nothing
    }

    public static void OnPostMoveFromPathsProcess(string[] movedFromAssetPaths)
    {
        // do nothing
    }
}