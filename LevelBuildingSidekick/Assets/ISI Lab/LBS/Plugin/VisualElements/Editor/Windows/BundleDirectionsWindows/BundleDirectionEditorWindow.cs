using ISILab.Commons.Utility.Editor;
using ISILab.LBS.CustomComponents;
using ISILab.Macros;
using UnityEditor;
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

    private LBSCustomEnumField upDirectionEnum;
    private LBSCustomEnumField downDirectionEnum;
    private LBSCustomEnumField leftDirectionEnum;
    private LBSCustomEnumField rightDirectionEnum;

    //Bottom buttons
    private LBSCustomButton RevertButton;
    private LBSCustomButton SaveButton;

    #endregion

    private Texture2D renderTexture;
    private Background previewImageBG;
    private GameObject previewCube;

    void CreateGUI()
    {
        var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("BundleDirectionEditorWindow");
        visualTree.CloneTree(rootVisualElement);

        directionTypeEnum = rootVisualElement.Q<LBSCustomEnumField>("DirectionTypeEnum");

        tagGroupEnum = rootVisualElement.Q<LBSCustomEnumField>("TagGroupEnum");


        middleZone = rootVisualElement.Q<VisualElement>("MiddleZone");

        // Create a render texture and camera for preview
        renderTexture = new Texture2D(256, 256, TextureFormat.RGBA32, false);

        previewImageBG = new Background
        {
            texture = renderTexture,
        };
        middleZone.style.backgroundImage = new StyleBackground(previewImageBG);

        previewCube = LBSAssetMacro.LoadAssetByGuid<GameObject>("53fbf72f0ddda114e850c0c7ad033716");

        EditorApplication.update += CheckPreview;

        centreVisual = rootVisualElement.Q<VisualElement>("Thumbnail");

        upDirectionEnum = rootVisualElement.Q<LBSCustomEnumField>("UpDirectionEnum");
        downDirectionEnum = rootVisualElement.Q<LBSCustomEnumField>("DownDirectionEnum");
        leftDirectionEnum = rootVisualElement.Q<LBSCustomEnumField>("LeftDirectionEnum");
        rightDirectionEnum = rootVisualElement.Q<LBSCustomEnumField>("RightDirectionEnum");

        RevertButton = rootVisualElement.Q<LBSCustomButton>("RevertButton");

        SaveButton = rootVisualElement.Q<LBSCustomButton>("SaveButton");


    }

    private void CheckPreview()
    {
        if (renderTexture == null && previewCube != null)
        {
            renderTexture = AssetPreview.GetAssetPreview(previewCube);

            // Still loading → returns null, keep waiting
            if (renderTexture == null)
                return;

            // Once ready, assign it
            previewImageBG = renderTexture;

            // Stop polling
            EditorApplication.update -= CheckPreview;
        }
    }
}
