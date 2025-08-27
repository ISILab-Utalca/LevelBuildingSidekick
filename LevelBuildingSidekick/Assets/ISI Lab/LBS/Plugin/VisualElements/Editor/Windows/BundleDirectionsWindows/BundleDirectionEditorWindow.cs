using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Characteristics;
using ISILab.LBS.Components;
using ISILab.LBS.CustomComponents;
using ISILab.Macros;
using LBS.Bundles;
using System;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class BundleDirectionEditorWindow : EditorWindow
{
    #region VIEW ELEMENTS

    //Top Enums
    private LBSCustomEnumField directionTypeEnum;
    private LBSCustomEnumField tagGroupEnum;

    //Centre
    private VisualElement middleZone;
    private VisualElement centreVisual;

    //Temporals
    private ObjectField UpDirectionField;
    private ObjectField DownDirectionField;
    private ObjectField LeftDirectionField;
    private ObjectField RightDirectionField;

    //Bottom buttons
    private LBSCustomButton RevertButton;
    private LBSCustomButton SaveButton;

    #endregion

    #region SQUARE PREVIEW ELEMENTS

    private Texture2D renderTexture;
    private GameObject previewPrefab;
    private PreviewRenderUtility prevRenderUtil;
    private GameObject prefab;

    #endregion

    public LBSDirection target;

    public void CreateGUI()
    {
        var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("BundleDirectionEditorWindow");
        visualTree.CloneTree(rootVisualElement);

        directionTypeEnum = rootVisualElement.Q<LBSCustomEnumField>("DirectionTypeEnum");

        tagGroupEnum = rootVisualElement.Q<LBSCustomEnumField>("TagGroupEnum");

        middleZone = rootVisualElement.Q<VisualElement>("MiddleZone");
        centreVisual = rootVisualElement.Q<VisualElement>("Thumbnail");

        UpDirectionField = rootVisualElement.Q<LBSCustomObjectField>("UpDirectionField");
        DownDirectionField = rootVisualElement.Q<LBSCustomObjectField>("DownDirectionField");
        LeftDirectionField = rootVisualElement.Q<LBSCustomObjectField>("LeftDirectionField");
        RightDirectionField = rootVisualElement.Q<LBSCustomObjectField>("RightDirectionField");

        RevertButton = rootVisualElement.Q<LBSCustomButton>("RevertButton");

        SaveButton = rootVisualElement.Q<LBSCustomButton>("SaveButton");

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

        EditorApplication.update += UpdatePreview;

        #endregion
    }

    private void SetValuesFromBundle()
    {
        if (target == null)
            return;

        RightDirectionField.value = target.GetConnection(0)[0] == "" ? null : LBSAssetMacro.GetLBSTag(target.GetConnection(0)[0]);
        UpDirectionField.value = target.GetConnection(0)[1] == "" ? null : LBSAssetMacro.GetLBSTag(target.GetConnection(0)[1]);
        LeftDirectionField.value = target.GetConnection(0)[2] == "" ? null : LBSAssetMacro.GetLBSTag(target.GetConnection(0)[2]);
        DownDirectionField.value = target.GetConnection(0)[3] == "" ? null : LBSAssetMacro.GetLBSTag(target.GetConnection(0)[3]);

        prefab = target.Owner.Assets[0].obj;

        target.GetConnection();
        
        

        foreach (var connection in target.Connections)
        {
            Debug.Log(connection + " " + target.Connections.FindIndex(c => c == connection));
        }
    }

    private void UpdatePreview()
    {
        prevRenderUtil.BeginStaticPreview(new Rect(0, 0, 512, 512));

        prevRenderUtil.camera.transform.position = new Vector3(0, 5, 0);
        prevRenderUtil.camera.transform.rotation = Quaternion.Euler(90, 0, 0);
        prevRenderUtil.camera.clearFlags = CameraClearFlags.Color;
        prevRenderUtil.camera.backgroundColor = Color.gray;

        prevRenderUtil.camera.orthographic = true;

        prevRenderUtil.camera.orthographicSize = 1f;
        prevRenderUtil.camera.nearClipPlane = 0.1f;
        prevRenderUtil.camera.farClipPlane = 100f;

        prevRenderUtil.lights[0].intensity = 1f;
        prevRenderUtil.lights[0].transform.rotation = Quaternion.Euler(50f, 50f, 0);

        prevRenderUtil.camera.Render();

        renderTexture = prevRenderUtil.EndStaticPreview();

        centreVisual.style.backgroundImage = new StyleBackground(renderTexture);
    }

    void OnDisable()
    {
        EditorApplication.update -= UpdatePreview;
        prevRenderUtil?.Cleanup();
        if (renderTexture != null)
            DestroyImmediate(renderTexture);
    }


}
