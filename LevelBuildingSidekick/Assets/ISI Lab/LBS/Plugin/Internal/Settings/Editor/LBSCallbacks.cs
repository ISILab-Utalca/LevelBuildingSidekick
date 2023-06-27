using LBS;
using LBS.Settings;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

[InitializeOnLoad]
public class CheckPackages
{
    static ListRequest Request;

    static CheckPackages()
    {
        CheckForPackages();
    }

    static void CheckForPackages()
    {
        Request = Client.List(offlineMode: true);
        EditorApplication.update += progress;
    }

    static void progress()
    {
        string packageId = "com.unity.nuget.newtonsoft-json";

        if (Request.IsCompleted)
        {
            if (Request.Status == StatusCode.Success)
            {
                if (!ContainsPackage(Request.Result, packageId))
                {
                    Debug.Log("Package '" + packageId + "' is missing");
                    string installCommand = "com.unity.nuget.newtonsoft-json";
                    //AssetDatabase.ImportPackage("Packages/com.unity.nuget.newtonsoft-json", true);
                    Client.Add(installCommand);

                }
            }
            else if (Request.Status >= StatusCode.Failure)
            {
                Debug.Log("Could not check for packages: " + Request.Error.message);
            }

            EditorApplication.update -= progress;
        }
    }

    public static bool ContainsPackage(PackageCollection packages, string packageId)
    {
        foreach (var package in packages)
        {
            if (string.Compare(package.name, packageId) == 0)
                return true;

            foreach (var dependencyInfo in package.dependencies)
                if (string.Compare(dependencyInfo.name, packageId) == 0)
                    return true;
        }

        return false;
    }
}

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


