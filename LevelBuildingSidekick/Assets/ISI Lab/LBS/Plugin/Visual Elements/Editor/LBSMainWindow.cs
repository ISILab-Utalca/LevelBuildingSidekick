using LBS.ElementView;
using LBS.VisualElements;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class LBSMainWindow : EditorWindow
{
    // Data
    public LBSLevelData levelData;

    //
    private VisualElement _extra;

    // Visual Elements
    private ButtonGroup toolPanel;
    private VisualElement extraPanel;
    private VisualElement noLayerSign;

    [MenuItem("ISILab/LBS plugin/Main window", priority = 0)]
    public static void ShowWindow()
    {
        var window = GetWindow<LBSMainWindow>();
        Texture icon = Resources.Load<Texture>("Icons/Logo"); // (!!) implementar 
        window.titleContent = new GUIContent("Level builder", icon);
    }

    public virtual void CreateGUI()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("LBSMainWindow");
        visualTree.CloneTree(rootVisualElement);

        levelData = new LBSLevelData(); // (!)
        //levelData.on

        // ToolBar
        var toolbar = rootVisualElement.Q<ToolBarMain>("ToolBar");

        // MainView 
        var mainView = rootVisualElement.Q<MainView>("MainView");

        // ToolPanel
        toolPanel = rootVisualElement.Q<ButtonGroup>("ToolPanel");

        // InspectorContent
        var inspector = rootVisualElement.Q<VisualElement>("InspectorContent");

        // ExtraPanel
        extraPanel = rootVisualElement.Q<VisualElement>("ExtraPanel");

        // NoLayerSign
        noLayerSign = rootVisualElement.Q<VisualElement>("NoLayerSign");
        levelData.OnChanged += (lvl) => {
            noLayerSign.style.display = (lvl.Layers.Count <= 0) ? DisplayStyle.None : DisplayStyle.Flex;
        };

        // LayerButton
        var layerBtn = rootVisualElement.Q<Button>("LayerButton");
        layerBtn.clicked += () =>
        {
            if(_extra == null || _extra.GetType() != typeof(LayersPanel))
            {
                extraPanel.Clear();
                _extra = new LayersPanel(levelData);
                extraPanel.Add(_extra);
            }
            else
            {
                extraPanel.Clear();
                _extra = null;
            }
        };

        // IAButton
        var IABtn = rootVisualElement.Q<Button>("IAButton");
        IABtn.clicked += () =>
        {
            if (_extra == null || _extra.GetType() != typeof(AIPanel))
            {
                extraPanel.Clear();
                _extra = new AIPanel();
                extraPanel.Add(_extra);
            }
            else
            {
                extraPanel.Clear();
                _extra = null;
            }
        };

        // 3DButton
        var Gen3DBtn = rootVisualElement.Q<Button>("3DButton");
        Gen3DBtn.clicked += () =>
        {
            if (_extra == null || _extra.GetType() != typeof(Generator3DPanel))
            {
                extraPanel.Clear();
                _extra = new Generator3DPanel();
                extraPanel.Add(_extra);
            }
            else
            {
                extraPanel.Clear();
                _extra = null;
            }
        };
    }
}
