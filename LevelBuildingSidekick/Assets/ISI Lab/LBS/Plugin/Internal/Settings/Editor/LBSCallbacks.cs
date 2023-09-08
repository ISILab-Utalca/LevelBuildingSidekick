using LBS;
using LBS.Bundles;
using LBS.Settings;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

[InitializeOnLoad]
public class LBSCallbacks
{
    static LBSCallbacks()
    {

        var onStart = SessionState.GetBool("start", true);
        if (onStart)
        {
            EditorApplication.update += OnStartEditor;
            SessionState.SetBool("start", false);
        }
    }

    private static void OnStartEditor() // <----
    {
        // busca y setea la instancia de "LBS Settings" en su singleton
        SettingsEditor.SearchSettingsInstance();
        ReloadBundles();

        EditorApplication.update -= OnStartEditor;

    }


    [UnityEditor.Callbacks.DidReloadScripts()]
    private static void OnScriptsReloaded() // <----
    {
        ReloadStorage();
        ReloadCurrentLevel();
        ReloadBundles();
    }

    public static void ReloadStorage()
    {
        var storage = LBSAssetsStorage.Instance;
        storage.SearchInProject();
    }

    public static void ReloadCurrentLevel()
    {
        var data = LBSController.CurrentLevel.data;
        data.Reload();
    }

    public static void ReloadBundles()
    {
        var storage = LBSAssetsStorage.Instance;
        var bundles = storage.Get<Bundle>();
        foreach (var bundle in bundles)
        {
            bundle.Reload();
        }
    }

}


