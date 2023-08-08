using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

[InitializeOnLoad]
public static class ImportJson
{
    static AddRequest Request;
    static EmbedRequest ERequest;
    static ListRequest LRequest;
    static String targetPackage;

    static ImportJson()
    {
        AssemblyReloadEvents.beforeAssemblyReload += Add;
    }

    static void Add()
    {
        if(!IsPackageInstalled("com.unity.nuget.newtonsoft-json"))
        {
            Request = Client.Add("com.unity.nuget.newtonsoft-json");
            EditorApplication.update += Progress;
            LRequest = Client.List();
            EditorApplication.update += LProgress;
        }
    }

    public static bool IsPackageInstalled(string packageId)
    {
        if (!File.Exists("Packages/manifest.json"))
            return false;

        string jsonText = File.ReadAllText("Packages/manifest.json");
        return jsonText.Contains(packageId);
    }


    static void Progress()
    {
        if (Request.IsCompleted)
        {
            if (Request.Status == StatusCode.Success)
                Debug.Log("Installed: " + Request.Result.packageId);
            else if (Request.Status >= StatusCode.Failure)
                Debug.Log(Request.Error.message);

            EditorApplication.update -= Progress;
        }
    }

    static void LProgress()
    {
        if (LRequest.IsCompleted)
        {
            if (LRequest.Status == StatusCode.Success)
            {
                foreach (var package in LRequest.Result)
                {
                    // Only retrieve packages that are currently installed in the
                    // project (and are neither Built-In nor already Embedded)
                    if (package.isDirectDependency && package.source
                        != PackageSource.BuiltIn && package.source
                        != PackageSource.Embedded)
                    {
                        targetPackage = package.name;
                        break;
                    }
                }

            }
            else
                Debug.Log(LRequest.Error.message);

            EditorApplication.update -= LProgress;

            Embed(targetPackage);

        }
    }

    static void Embed(string inTarget)
    {
        // Embed a package in the project
        Debug.Log("Embed('" + inTarget + "') called");
        ERequest = Client.Embed(inTarget);
        EditorApplication.update += Progress;

    }
}
