using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Characteristics;
using ISILab.LBS.Components;
using ISILab.LBS.CustomComponents;
using ISILab.Macros;
using LBS.Bundles;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using ISILab.LBS.Internal;
using ISILab.LBS;
using ISILab.LBS.VisualElements;

public class BundleDirectionEditorWindow : EditorWindow
{
    #region VIEW ELEMENTS

    //Top Enums
    private LBSCustomEnumField directionTypeEnum;
    private LBSCustomDropdown tagGroupDropdown;

    //Centre
    private VisualElement centreThumbnail;
    private VisualElement centreFrame;

    //Directions
    private LBSCustomDropdown UpDirectionDropdown;
    private LBSCustomDropdown DownDirectionDropdown;
    private LBSCustomDropdown LeftDirectionDropdown;
    private LBSCustomDropdown RightDirectionDropdown;

    private LBSCustomDropdown URDirectionDropdown; //Upper Right
    private LBSCustomDropdown ULDirectionDropdown; //Upper Left
    private LBSCustomDropdown LRDirectionDropdown; //Lower Right
    private LBSCustomDropdown LLDirectionDropdown; //Lower Left

    //Zoom Buttons
    private LBSCustomUnsignedIntegerField zoomScaleInt;

    //Bottom buttons
    private LBSCustomButton RevertButton;
    private LBSCustomButton SaveButton;

    #endregion

    #region ELEMENTS

    private List<LBSTag> allTags;
    private List<string> currentTagList;
    private string currentTagGroup;

    private List<LBSTagGroup> allTagGroups;
    private List<LBSTagGroup> tagBundles;

    public LBSDirection target;

    private VectorImage edgeFrame;
    private VectorImage vertexFrame;

    private int currentType = 0;

    #region SQUARE PREVIEW ELEMENTS

    private Texture2D renderTexture;
    private GameObject previewPrefab;
    private PreviewRenderUtility prevRenderUtil;
    private GameObject prefab;

    private float fovScale;

    #endregion

    #endregion

    public void CreateGUI()
    {
        var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("BundleDirectionEditorWindow");
        visualTree.CloneTree(rootVisualElement);

        allTagGroups = DirectoryTools.GetScriptables<LBSTagGroup>();
        tagBundles = allTagGroups.Where(bundle => !AssetDatabase.GetAssetPath(bundle).Contains("Deprecated")).ToList();

        tagGroupDropdown = rootVisualElement.Q<LBSCustomDropdown>("TagGroupDropdown");
        tagGroupDropdown.RegisterValueChangedCallback(evt =>
        {
            if (evt.newValue != evt.previousValue)
            {
                ChangeTagsFromBundle();
            }
        });

        centreThumbnail = rootVisualElement.Q<VisualElement>("Thumbnail");
        centreFrame = rootVisualElement.Q<VisualElement>("Frame");

        UpDirectionDropdown = rootVisualElement.Q<LBSCustomDropdown>("UpDirectionDropdown");
        DownDirectionDropdown = rootVisualElement.Q<LBSCustomDropdown>("DownDirectionDropdown");
        LeftDirectionDropdown = rootVisualElement.Q<LBSCustomDropdown>("LeftDirectionDropdown");
        RightDirectionDropdown = rootVisualElement.Q<LBSCustomDropdown>("RightDirectionDropdown");

        URDirectionDropdown = rootVisualElement.Q<LBSCustomDropdown>("URDirectionDropdown"); //Upper Right
        ULDirectionDropdown = rootVisualElement.Q<LBSCustomDropdown>("ULDirectionDropdown"); //Upper Left
        LRDirectionDropdown = rootVisualElement.Q<LBSCustomDropdown>("LRDirectionDropdown"); //Lower Right
        LLDirectionDropdown = rootVisualElement.Q<LBSCustomDropdown>("LLDirectionDropdown"); //Lower Left

        edgeFrame = LBSAssetMacro.LoadAssetByGuid<VectorImage>("533a887ccf1a0b444a147165d3fb6a6b");
        vertexFrame = LBSAssetMacro.LoadAssetByGuid<VectorImage>("f0de0c827bb8a654e8cc48b71ca0b057");

        directionTypeEnum = rootVisualElement.Q<LBSCustomEnumField>("DirectionTypeEnum");
        directionTypeEnum.RegisterValueChangedCallback(evt =>
        {
            if (evt.newValue != evt.previousValue)
            {
                currentType = (int)(object)evt.newValue;
                SwitchVisibility(currentType);
            }
        });

        zoomScaleInt = rootVisualElement.Q<LBSCustomUnsignedIntegerField>("ZoomScaleInt");
        zoomScaleInt.RegisterValueChangedCallback(evt =>
        {
            if (evt.newValue != evt.previousValue)
            {
                fovScale = 1 + (evt.newValue * 0.1f);
                StepPreview();
            }
        });

        Init();

        RevertButton = rootVisualElement.Q<LBSCustomButton>("RevertButton");
        RevertButton.clicked += RevertChanges;

        SaveButton = rootVisualElement.Q<LBSCustomButton>("SaveButton");
        SaveButton.clicked += SaveTags;

        #region Square Preview Setup

        renderTexture = new Texture2D(512, 512, TextureFormat.RGBA32, false);

        centreThumbnail.style.backgroundImage = new StyleBackground(renderTexture);

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

    

    private void Init()
    {
        currentType = (int)(object)directionTypeEnum.value;
        SwitchVisibility(currentType);

        allTags = LBSAssetsStorage.Instance.Get<LBSTag>();

        var tagLabels = tagBundles[0].Tags;

        tagGroupDropdown.choices = tagBundles.Select(bundle => bundle.name).ToList();
        tagGroupDropdown.SetValueWithoutNotify(DetectTagGroupFromFirstTag(ConvertStringToLBSTag(RightDirectionDropdown.value)));

        currentTagGroup = tagGroupDropdown.value;

        SetChoices(tagBundles.Find(bundle => bundle.name == tagGroupDropdown.value)?.Tags ?? tagLabels);
        SetValuesFromBundle();
        WriteDefaults();

        fovScale = 1 + (zoomScaleInt.value * 0.1f);
    }

    private string DetectTagGroupFromFirstTag(LBSTag tag)
    {
        foreach (var bundle in tagBundles)
        {
            if (bundle.Tags.Contains(tag))
                return bundle.name;
        }
        return tagGroupDropdown.choices[0];
    }

    private List<string> TagListToStringList(List<LBSTag> tags)
    {
        var list = new List<string>();
        foreach (var tag in tags)
        {
            list.Add(tag.name);
        }

        return list;
    }

    private void ChangeTagsFromBundle()
    {
        var selectedBundle = tagBundles[tagGroupDropdown.index];
        var tagLabels = selectedBundle.Tags;

        AlternateTags(tagLabels);
    }

    private void SetChoices(List<LBSTag> tags)
    {
        RightDirectionDropdown.choices = TagListToStringList(tags);
        UpDirectionDropdown.choices = TagListToStringList(tags);
        LeftDirectionDropdown.choices = TagListToStringList(tags);
        DownDirectionDropdown.choices = TagListToStringList(tags);

        URDirectionDropdown.choices = TagListToStringList(tags);
        ULDirectionDropdown.choices = TagListToStringList(tags);
        LLDirectionDropdown.choices = TagListToStringList(tags);
        LRDirectionDropdown.choices = TagListToStringList(tags);
    }

    private void AlternateTags(List<LBSTag> tags)
    {
        List<int> temp = new()
        {
            RightDirectionDropdown.index,
            UpDirectionDropdown.index,
            LeftDirectionDropdown.index,
            DownDirectionDropdown.index,
            URDirectionDropdown.index,
            ULDirectionDropdown.index,
            LLDirectionDropdown.index,
            LRDirectionDropdown.index
        };

        SetChoices(tags);

        RightDirectionDropdown.index = Mathf.Clamp(temp[0], 0, RightDirectionDropdown.choices.Count - 1);
        UpDirectionDropdown.index = Mathf.Clamp(temp[1], 0, UpDirectionDropdown.choices.Count - 1);
        LeftDirectionDropdown.index = Mathf.Clamp(temp[2], 0, LeftDirectionDropdown.choices.Count - 1);
        DownDirectionDropdown.index = Mathf.Clamp(temp[3], 0, DownDirectionDropdown.choices.Count - 1);

        URDirectionDropdown.index = Mathf.Clamp(temp[4], 0, URDirectionDropdown.choices.Count - 1);
        ULDirectionDropdown.index = Mathf.Clamp(temp[5], 0, ULDirectionDropdown.choices.Count - 1);
        LLDirectionDropdown.index = Mathf.Clamp(temp[6], 0, LLDirectionDropdown.choices.Count - 1);
        LRDirectionDropdown.index = Mathf.Clamp(temp[7], 0, LRDirectionDropdown.choices.Count - 1);
    }

    private void SwitchVisibility(int type)
    {
        switch (type)
        {
            case 0:
                // Vertex Based
                UpDirectionDropdown.style.display = DisplayStyle.Flex;
                DownDirectionDropdown.style.display = DisplayStyle.Flex;
                LeftDirectionDropdown.style.display = DisplayStyle.Flex;
                RightDirectionDropdown.style.display = DisplayStyle.Flex;
                URDirectionDropdown.style.display = DisplayStyle.None;
                ULDirectionDropdown.style.display = DisplayStyle.None;
                LLDirectionDropdown.style.display = DisplayStyle.None;
                LRDirectionDropdown.style.display = DisplayStyle.None;

                centreFrame.style.backgroundImage = new StyleBackground(vertexFrame);
                break;
            case 1:
                // Edge Based
                UpDirectionDropdown.style.display = DisplayStyle.None;
                DownDirectionDropdown.style.display = DisplayStyle.None;
                LeftDirectionDropdown.style.display = DisplayStyle.None;
                RightDirectionDropdown.style.display = DisplayStyle.None;
                URDirectionDropdown.style.display = DisplayStyle.Flex;
                ULDirectionDropdown.style.display = DisplayStyle.Flex;
                LLDirectionDropdown.style.display = DisplayStyle.Flex;
                LRDirectionDropdown.style.display = DisplayStyle.Flex;

                centreFrame.style.backgroundImage = new StyleBackground(edgeFrame);
                break;
        }

        SetValuesFromBundle();
    }

    /* UNUSED METHODS
     
    private List<LBSTag> GetPossibleTagsFromBundle(Bundle bundle, List<LBSTag> identifierTags)
    {
        var connections = bundle.GetChildrenCharacteristics<LBSDirection>();
        var tags = connections.SelectMany(c => c.Connections).ToList().RemoveDuplicates();
        if (tags.Remove("Empty")) tags.Insert(0, "Empty");
        var idents = tags.Select(s => identifierTags.Find(i => s == i.Label)).ToList().RemoveEmpties();

        return idents;
    }

    
    private List<LBSTag> GetPossibleTagsFromTag(LBSTag tag, List<LBSTag> identifierTags)
    {
        var connections = tag.GetChildrenCharacteristics<LBSDirection>();
        var tags = connections.SelectMany(c => c.Connections).ToList().RemoveDuplicates();
        if (tags.Remove("Empty")) tags.Insert(0, "Empty");
        var idents = tags.Select(s => identifierTags.Find(i => s.Label == i.Label)).ToList().RemoveEmpties();
        return idents;
    }
    */

    private void SetValuesFromBundle()
    {
        if (target == null)
            return;

        switch (currentType)
        {
            case 0:
                RightDirectionDropdown.value = URDirectionDropdown.value ?? target.GetConnection()[0];
                UpDirectionDropdown.value = ULDirectionDropdown.value ?? target.GetConnection()[1];
                LeftDirectionDropdown.value = LLDirectionDropdown.value ?? target.GetConnection()[2];
                DownDirectionDropdown.value = LRDirectionDropdown.value ?? target.GetConnection()[3];
                break;
            case 1:
                URDirectionDropdown.value = RightDirectionDropdown.value ?? target.GetConnection()[0];
                ULDirectionDropdown.value = UpDirectionDropdown.value ?? target.GetConnection()[1];
                LLDirectionDropdown.value = LeftDirectionDropdown.value ?? target.GetConnection()[2];
                LRDirectionDropdown.value = DownDirectionDropdown.value ?? target.GetConnection()[3];
                break;
        }

        prefab = target.Owner.Assets[0].obj;
    }

    private void WriteDefaults()
    {
        currentTagList?.Clear();

        currentTagList = new List<string>()
        {
            RightDirectionDropdown.value,
            UpDirectionDropdown.value,
            LeftDirectionDropdown.value,
            DownDirectionDropdown.value
        };
    }


    private void StepPreview()
    {
        prevRenderUtil.BeginStaticPreview(new Rect(0, 0, 512, 512));

        prevRenderUtil.camera.transform.position = new Vector3(0, 10, 0);
        prevRenderUtil.camera.transform.rotation = Quaternion.Euler(90, 0, 0);

        prevRenderUtil.camera.orthographic = true;

        prevRenderUtil.camera.orthographicSize = fovScale;
        prevRenderUtil.camera.nearClipPlane = 0.1f;
        prevRenderUtil.camera.farClipPlane = 100f;

        prevRenderUtil.lights[0].intensity = 1f;
        prevRenderUtil.lights[0].transform.rotation = Quaternion.Euler(50f, 50f, 0);

        prevRenderUtil.camera.Render();

        renderTexture = prevRenderUtil.EndStaticPreview();

        centreThumbnail.style.backgroundImage = new StyleBackground(renderTexture);
    }

    private void SaveTags()
    {
        if (target == null)
            return;

        switch (currentType)
        {
            case 0:
                target.SetConnection(ConvertStringToLBSTag(RightDirectionDropdown.value), 0);
                target.SetConnection(ConvertStringToLBSTag(UpDirectionDropdown.value), 1);
                target.SetConnection(ConvertStringToLBSTag(LeftDirectionDropdown.value), 2);
                target.SetConnection(ConvertStringToLBSTag(DownDirectionDropdown.value), 3);
                break;
            case 1:
                target.SetConnection(ConvertStringToLBSTag(URDirectionDropdown.value), 0);
                target.SetConnection(ConvertStringToLBSTag(ULDirectionDropdown.value), 1);
                target.SetConnection(ConvertStringToLBSTag(LLDirectionDropdown.value), 2);
                target.SetConnection(ConvertStringToLBSTag(LRDirectionDropdown.value), 3);
                break;
        }

        UpdateWindow();
        EditorUtility.SetDirty(target.Owner);
        AssetDatabase.SaveAssets();
    }

    private void RevertChanges()
    {
        if (!EditorUtility.DisplayDialog("Revert Changes", "All unsaved changes will be lost. Are you sure you want to continue?", "Yes", "No"))
            return;

        RightDirectionDropdown.value = currentTagList[0];
        UpDirectionDropdown.value = currentTagList[1];
        LeftDirectionDropdown.value = currentTagList[2];
        DownDirectionDropdown.value = currentTagList[3];

        URDirectionDropdown.value = currentTagList[0];
        ULDirectionDropdown.value = currentTagList[1];
        LLDirectionDropdown.value = currentTagList[2];
        LRDirectionDropdown.value = currentTagList[3];

        tagGroupDropdown.SetValueWithoutNotify(currentTagGroup);

        SetChoices(tagBundles.Find(bundle => bundle.name == tagGroupDropdown.value)?.Tags);
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

        return allTags.Find(tag => tag.Label == tagLabel);
    }

    private string ConvertLBSTagToString(LBSTag tag)
    {
        if (tag == null)
            return "Empty";
        return tag.Label;
    }


}
