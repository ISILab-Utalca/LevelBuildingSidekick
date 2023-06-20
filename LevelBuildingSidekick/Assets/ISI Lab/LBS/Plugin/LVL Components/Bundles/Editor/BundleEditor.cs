using LBS.Settings;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Bundle))]
public class BundleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var bundle = target as Bundle;

        GUILayout.Label("inDev Tools: presset");

        if (GUILayout.Button("Set default interior"))
        {
            SetDefaultinterior(bundle);
            AssetDatabase.SaveAssets();
        }

        if (GUILayout.Button("Set default Exterior"))
        {
            SetDefaultExterior(bundle);
            AssetDatabase.SaveAssets();
        }

        GUILayout.Space(20);
        GUILayout.Label("inDev Tools: characteristics");

        if(GUILayout.Button("Add Connection GROUP Charac"))
        {
            AddConnGroupCharc(bundle);
            AssetDatabase.SaveAssets();
        }

        if (GUILayout.Button("Add Connection Characteristic"))
        {
            AddConnectionCharacteristic(bundle);
            AssetDatabase.SaveAssets();
        }

        if (GUILayout.Button("Add Tag characteristic"))
        {
            AddTagCharc(bundle);
            AssetDatabase.SaveAssets();
        }
    }

    private void AddConnGroupCharc(Bundle bundle)
    {
        var charc = new LBSDirectionedGroup();
        bundle.AddCharacteristic(charc);

        EditorUtility.SetDirty(bundle);
    }

    private void AddTagCharc(Bundle bundle)
    {
        var charc = new LBSTagsCharacteristic(null);
        bundle.AddCharacteristic(charc);

        EditorUtility.SetDirty(bundle);
    }

    private void AddConnectionCharacteristic(Bundle bundle)
    {
        var charc = new LBSDirection("Connections", new List<string>() { "", "", "", "" });
        bundle.AddCharacteristic(charc);

        EditorUtility.SetDirty(bundle);
    }

    private void SetDefaultinterior(Bundle bundle)
    {
        // Get settings
        var setting = LBSSettings.Instance;

        // Get current path
        var path = AssetDatabase.GetAssetPath(bundle).Replace("/" + bundle.name + ".asset", "");
        Debug.Log(bundle + " - " + path);

        // Get tags
        var targets = new List<string>() { "Door", "Wall", "Floor", "Empty" };
        var allTags = Utility.DirectoryTools.GetScriptables<LBSIdentifier>();
        var matchingTags = allTags.Where(tag => targets.Contains(tag.Label)).ToList();

        // Create ID
        var id = ScriptableObject.CreateInstance<LBSIdentifier>();
        var tagName = ISILab.Commons.Commons.CheckNameFormat(allTags.Select(t => t.Label), "Schema");
        AssetDatabase.CreateAsset(id, path + "/" + tagName + ".asset");
        id.Init(tagName, new Color().RandomColor(), null);

        // Init bundle
        bundle.ID = id;
        bundle.ClearChilds();
        for (int i = 0; i < matchingTags.Count; i++)
        {
            // Set child bundle
            var b = ScriptableObject.CreateInstance<Bundle>();
            b.ID = matchingTags[i];
            b.AddCharacteristic(new LBSTagsCharacteristic(matchingTags[i]));
            bundle.AddChild(b);

            // Save child bundle
            var nn = Utility.DirectoryTools.GetScriptables<Bundle>();
            var name = ISILab.Commons.Commons.CheckNameFormat(nn.Select(t => t.name), "Sub bundle");
            AssetDatabase.CreateAsset(b, path + "/" + name + ".asset");
        }

    }

    private void SetDefaultExterior(Bundle bundle)
    {
        var charac = new LBSDirectionedGroup();
        bundle.AddCharacteristic(charac);
    }
}