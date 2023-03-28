using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New LBS Storage",menuName = "LBS/Internal/AssetStorage")]
public class LBSAssetsStorage : ScriptableObject
{
    #region SINGLETON
    private static LBSAssetsStorage instance;
    public static LBSAssetsStorage Instance
    {
        get
        {
            if(instance == null)
            {
                instance = Utility.DirectoryTools.GetScriptable<LBSAssetsStorage>();
            }
            return instance;
        }
    }
    #endregion

    #region FIELDS
    [SerializeField]
    private List<Bundle> bundles = new List<Bundle>();

    [SerializeField]
    private List<LBSIdentifier> tags = new List<LBSIdentifier>();
    #endregion

    #region PROPERTIES
    public List<Bundle> Bundles
    {
        get
        {
            CleanBundles();
            return new List<Bundle>(bundles);
        }
    }

    public List<LBSIdentifier> Tags
    {
        get
        {
            CleanBundles();
            return new List<LBSIdentifier>(tags);
        }
    }
    #endregion

    #region METHODS
    public void AddTags(LBSIdentifier tag)
    {
        if(!tags.Contains(tag))
        {
            tags.Add(tag);
        }
    }
    public void RemoveTags(LBSIdentifier tag)
    {
        tags.Remove(tag);
    }

    public void AddBundle(Bundle bundle)
    {
        if(!bundles.Contains(bundle))
        {
            bundles.Add(bundle);
        }
    }

    public void RemoveBundle(Bundle bundle)
    {
        bundles.Remove(bundle);
    }

    public void CleanBundles()
    {
        bundles = bundles.Where(b => b != null).ToList();
    }

    public void CleanTags()
    {
        tags = tags.Where(b => b != null).ToList();
    }

    public void SearchAllInProject()
    {
        bundles = Utility.DirectoryTools.GetScriptables<Bundle>();
        tags = Utility.DirectoryTools.GetScriptables<LBSIdentifier>();
        AssetDatabase.SaveAssets();
    }
    #endregion
}

[CustomEditor(typeof(LBSAssetsStorage))]
public class LBSAssetsStorage_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(20);

        var storage = target as LBSAssetsStorage;
        if(GUILayout.Button("Search all in Project"))
        {
            storage.SearchAllInProject();
        }
    }
}