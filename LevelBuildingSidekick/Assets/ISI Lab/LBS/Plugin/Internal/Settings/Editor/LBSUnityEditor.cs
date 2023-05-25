using LBS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LBSUnityEditor
{
    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptsReloaded()
    {
        var data = LBSController.CurrentLevel.data;
        data.Reload();
    }
}
