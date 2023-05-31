using LBS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LBSUnityEditor
{
    [UnityEditor.Callbacks.DidReloadScripts()]
    private static void OnScriptsReloaded()
    {
        var data = LBSController.CurrentLevel.data;
        data.Reload();

        var storage = LBSAssetsStorage.Instance;
        var bundles = storage.Get<Bundle>();
        foreach (var bundle in bundles)
        {
            bundle.Reload();
        }
    }
}
