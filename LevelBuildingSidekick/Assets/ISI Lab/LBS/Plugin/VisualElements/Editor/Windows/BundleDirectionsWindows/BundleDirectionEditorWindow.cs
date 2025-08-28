using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Characteristics;
using ISILab.LBS.Components;
using ISILab.LBS.CustomComponents;
using ISILab.Macros;
using LBS.Bundles;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using ISILab.Extensions;
using static UnityEditor.Progress;
using ISILab.LBS.Internal;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class BundleDirectionEditorWindow : EditorWindow
{
    #region VIEW ELEMENTS

    //Top Enums
    private LBSCustomEnumField directionTypeEnum;
    private LBSCustomEnumField tagGroupEnum;

    //Centre
    private VisualElement middleZone;
    private VisualElement centreVisual;

    //Directions
    private LBSCustomDropdown UpDirectionDropdown;
    private LBSCustomDropdown DownDirectionDropdown;
    private LBSCustomDropdown LeftDirectionDropdown;
    private LBSCustomDropdown RightDirectionDropdown;

    //Bottom buttons
    private LBSCustomButton RevertButton;
    private LBSCustomButton SaveButton;

    #endregion

    #region ELEMENTS

    private List<LBSTag> allTags;
    private List<string> currentTagList;

    public LBSDirection target;

    #region SQUARE PREVIEW ELEMENTS

    private Texture2D renderTexture;
    private GameObject previewPrefab;
    private PreviewRenderUtility prevRenderUtil;
    private GameObject prefab;

    #endregion

    #endregion

    public void CreateGUI()
    {
        var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("BundleDirectionEditorWindow");
        visualTree.CloneTree(rootVisualElement);

        directionTypeEnum = rootVisualElement.Q<LBSCustomEnumField>("DirectionTypeEnum");

        tagGroupEnum = rootVisualElement.Q<LBSCustomEnumField>("TagGroupEnum");

        middleZone = rootVisualElement.Q<VisualElement>("MiddleZone");
        centreVisual = rootVisualElement.Q<VisualElement>("Thumbnail");

        UpDirectionDropdown = rootVisualElement.Q<LBSCustomDropdown>("UpDirectionDropdown");
        DownDirectionDropdown = rootVisualElement.Q<LBSCustomDropdown>("DownDirectionDropdown");
        LeftDirectionDropdown = rootVisualElement.Q<LBSCustomDropdown>("LeftDirectionDropdown");
        RightDirectionDropdown = rootVisualElement.Q<LBSCustomDropdown>("RightDirectionDropdown");

        allTags = LBSAssetsStorage.Instance.Get<LBSTag>();
        InitDropdowns();

        RevertButton = rootVisualElement.Q<LBSCustomButton>("RevertButton");
        RevertButton.clicked += RevertChanges;

        SaveButton = rootVisualElement.Q<LBSCustomButton>("SaveButton");
        SaveButton.clicked += SaveTags;

        #region Square Preview Setup

        SetValuesFromBundle();

        renderTexture = new Texture2D(512, 512, TextureFormat.RGBA32, false);

        centreVisual.style.backgroundImage = new StyleBackground(renderTexture);

        prevRenderUtil = new PreviewRenderUtility();
        prevRenderUtil.cameraFieldOfView = 30f;

        if (prefab != null)
        {
            previewPrefab = prevRenderUtil.InstantiatePrefabInScene(prefab);
            previewPrefab.transform.position = Vector3.zero;
        }

        EditorApplication.delayCall += StepPreview;

        #endregion
    }

    private void InitDropdowns()
    {
        var allPossibleTags = GetPossibleTagsFromBundle(target.Owner.Parent(), allTags);
        var tagLabels = allPossibleTags.Select(t => t.Label).ToList();

        RightDirectionDropdown.choices = tagLabels;
        UpDirectionDropdown.choices = tagLabels;
        LeftDirectionDropdown.choices = tagLabels;
        DownDirectionDropdown.choices = tagLabels;
    }

    private List<LBSTag> GetPossibleTagsFromBundle(Bundle bundle, List<LBSTag> identifierTags)
    {
        var connections = bundle.GetChildrenCharacteristics<LBSDirection>();
        var tags = connections.SelectMany(c => c.Connections).ToList().RemoveDuplicates();
        if (tags.Remove("Empty")) tags.Insert(0, "Empty");
        var idents = tags.Select(s => identifierTags.Find(i => s == i.Label)).ToList().RemoveEmpties();

        return idents;
    }

    private void SetValuesFromBundle()
    {
        if (target == null)
            return;

        RightDirectionDropdown.value = target.GetConnection()[0];
        UpDirectionDropdown.value = target.GetConnection()[1];
        LeftDirectionDropdown.value = target.GetConnection()[2];
        DownDirectionDropdown.value = target.GetConnection()[3];

        currentTagList = new List<string>()
        {
            RightDirectionDropdown.value,
            UpDirectionDropdown.value,
            LeftDirectionDropdown.value,
            DownDirectionDropdown.value
        };

        prefab = target.Owner.Assets[0].obj;
    }


    private void StepPreview()
    {
        prevRenderUtil.BeginStaticPreview(new Rect(0, 0, 512, 512));

        prevRenderUtil.camera.transform.position = new Vector3(0, 5, 0);
        prevRenderUtil.camera.transform.rotation = Quaternion.Euler(90, 0, 0);

        prevRenderUtil.camera.orthographic = true;

        prevRenderUtil.camera.orthographicSize = 1f;
        prevRenderUtil.camera.nearClipPlane = 0.1f;
        prevRenderUtil.camera.farClipPlane = 100f;

        prevRenderUtil.lights[0].intensity = 1f;
        prevRenderUtil.lights[0].transform.rotation = Quaternion.Euler(50f, 50f, 0);

        prevRenderUtil.camera.Render();

        renderTexture = prevRenderUtil.EndStaticPreview();

        centreVisual.style.backgroundImage = new StyleBackground(renderTexture);

        prevRenderUtil?.Cleanup();
    }

    private void SaveTags()
    {
        if (target == null)
            return;

        target.SetConnection(ConvertStringToLBSTag(RightDirectionDropdown.value), 0);
        target.SetConnection(ConvertStringToLBSTag(UpDirectionDropdown.value), 1);
        target.SetConnection(ConvertStringToLBSTag(LeftDirectionDropdown.value), 2);
        target.SetConnection(ConvertStringToLBSTag(DownDirectionDropdown.value), 3);

        UpdateWindow();
        EditorUtility.SetDirty(target.Owner);
        AssetDatabase.SaveAssets();
    }

    private void RevertChanges()
    {
        RightDirectionDropdown.value = currentTagList[0];
        UpDirectionDropdown.value = currentTagList[1];
        LeftDirectionDropdown.value = currentTagList[2];
        DownDirectionDropdown.value = currentTagList[3];
    }

    private void UpdateWindow()
    {
        Selection.activeObject = null;
        EditorApplication.delayCall += () => Selection.activeObject = target.Owner;
    }

    void OnDestroy()
    {
        prevRenderUtil?.Cleanup();
        if (renderTexture != null)
            DestroyImmediate(renderTexture);
    }

    private LBSTag ConvertStringToLBSTag(string tagLabel)
    {
        if (string.IsNullOrEmpty(tagLabel))
            return null;

        return allTags.FirstOrDefault(tag => tag.Label == tagLabel);
    }

    private string ConvertLBSTagToString(LBSTag tag)
    {
        if (tag == null)
            return "Empty";
        return tag.Label;
    }


}
