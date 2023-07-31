using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class PackageVerificator
{
    static AddRequest Request;

    static PackageVerificator()
    {
        // Add a package to the project
        string package = "com.unity.nuget.newtonsoft-json";

        if (!IsPackageInstalled(package))
        {
            Request = Client.Add(package);
            EditorApplication.update += Progress;
        }
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

    public static bool IsPackageInstalled(string packageId)
    {
        if (!File.Exists("Packages/manifest.json"))
            return false;

        string jsonText = File.ReadAllText("Packages/manifest.json");
        return jsonText.Contains(packageId);
    }
}
