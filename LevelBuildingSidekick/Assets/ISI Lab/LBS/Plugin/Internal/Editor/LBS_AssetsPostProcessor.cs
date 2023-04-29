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
                return;

            if (!asset.Contains(".asset"))
                return;

            var obj = AssetDatabase.LoadAssetAtPath<Object>(asset);

            if (obj is Bundle_Old)
            {
                storage.AddBundle(obj as Bundle_Old);
            }
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

            storage.CleanBundles();
        }
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