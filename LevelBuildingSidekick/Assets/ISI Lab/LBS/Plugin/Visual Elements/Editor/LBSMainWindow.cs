using LBS;
using LBS.Components;
using LBS.VisualElements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;


public class LBSMainWindow : EditorWindow
{
    // Data
    private LBSLevelData levelData => LBS.LBS.loadedLevel.data;

    // Selected
    private LBSLayer _selectedLayer;

    // Templates
    public List<LayerTemplate> layerTemplates;

    // Visual Elements
    private ButtonGroup toolPanel;
    private VisualElement extraPanel;
    private VisualElement noLayerSign;
    private MainView mainView; // work canvas
    private Label selectedLabel;
    private VisualElement floatingPanelContent;

    // Panels
    private LayersPanel layerPanel;
    private Generator3DPanel gen3DPanel;
    private LayerInspector layerInspector;

    // Manager
    private ToolKit toolkit;
    private DrawManager drawManager;
    private LBSInspectorPanel inspectorManager;

    public static Action OnWindowRepaint;


    [MenuItem("ISILab/Level Building Sidekick", priority = 0)]
    private static void ShowWindow()
    {
        var window = GetWindow<LBSMainWindow>();
        Texture icon = Resources.Load<Texture>("Icons/LBS_Logo1");
        window.titleContent = new GUIContent("Level builder", icon);
        window.minSize = new Vector2(800, 400);

        
        //window.RefreshWindow();
    }

    private static LBSMainWindow _ShowWindow()
    {
        var window = GetWindow<LBSMainWindow>();
        Texture icon = Resources.Load<Texture>("Icons/LBS_Logo1");
        window.titleContent = new GUIContent("Level builder", icon);

        LBS.LBS.loadedLevel = LBSController.CreateNewLevel("new file", new Vector3(100, 100, 100));
        return window;
    }

    public virtual void CreateGUI()
    {
        Init();
    } 

    private void Init()
    {
        
        if (LBS.LBS.loadedLevel == null)
        {
            LBS.LBS.loadedLevel = LBSController.CreateNewLevel("new file", new Vector3(100, 100, 100));
        }

        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("LBSMainWindow");
        visualTree.CloneTree(rootVisualElement);

        // LayerTemplate
        layerTemplates = Utility.DirectoryTools.GetScriptablesByType<LayerTemplate>();

        // SubPanelScrollView
        var subPanelScrollView = rootVisualElement.Q<ScrollView>("SubPanelScrollView");
        subPanelScrollView.Q<VisualElement>("unity-content-and-vertical-scroll-container").pickingMode = PickingMode.Ignore;
        subPanelScrollView.Q<VisualElement>("unity-content-viewport").pickingMode = PickingMode.Ignore;
        subPanelScrollView.Q<VisualElement>("unity-content-container").pickingMode = PickingMode.Ignore;

        // ToolPanel
        toolPanel = rootVisualElement.Q<ButtonGroup>("ToolsGroup");

        // LayerInspector
        layerInspector = rootVisualElement.Q<LayerInspector>("LayerInspector");

        // MainView 
        mainView = rootVisualElement.Q<MainView>("MainView");
        mainView.OnClearSelection += () =>
        {
            if (_selectedLayer != null)
            {
                var il = Reflection.MakeGenericScriptable(_selectedLayer);
                Selection.SetActiveObjectWithContext(il, il);
            }
        };

        // DrawManager
        drawManager = new DrawManager(ref mainView, ref layerTemplates);

        // InspectorContent
        inspectorManager = rootVisualElement.Q<LBSInspectorPanel>("InpectorPanel");

        // ToolKitManager
        toolkit = rootVisualElement.Q<ToolKit>();
        toolkit.OnEndAction += () =>
        {
            drawManager.Redraw(levelData, mainView);
        };

        // ToolBar
        var toolbar = rootVisualElement.Q<ToolBarMain>("ToolBar");
        toolbar.OnNewLevel += (data) =>
        {
            LBS.LBS.loadedLevel = data;
            RefreshWindow();
        };
        toolbar.OnLoadLevel += (data) =>
        {
            LBS.LBS.loadedLevel = data;
            RefreshWindow();
            drawManager.Redraw(levelData, mainView);
        };

        // ExtraPanel
        extraPanel = rootVisualElement.Q<VisualElement>("ExtraPanel");

        // NoLayerSign
        noLayerSign = rootVisualElement.Q<VisualElement>("NoLayerSign");

        // SelectedLabel
        selectedLabel = rootVisualElement.Q<Label>("SelectedLabel");


        // FloatingPanelContent
        floatingPanelContent = rootVisualElement.Q<VisualElement>("FloatingPanelContent");

        // Init Data
        OnLevelDataChange(levelData);
        levelData.OnChanged += (lvl) => {
            OnLevelDataChange(lvl);
        };

        // LayerPanel
        layerPanel = new LayersPanel(levelData, ref layerTemplates);
        extraPanel.Add(layerPanel);
        layerPanel.style.display = DisplayStyle.Flex;
        layerPanel.OnLayerVisibilityChange += (l) =>
        {
            drawManager.Redraw(levelData, mainView);
        };
        layerPanel.OnSelectLayer += ShowinfoLayer;
        layerPanel.OnAddLayer += ShowinfoLayer;

        // Gen3DPanel
        gen3DPanel = new Generator3DPanel();
        extraPanel.Add(gen3DPanel);
        gen3DPanel.style.display = DisplayStyle.None;
        gen3DPanel.OnExecute = () =>
        {
            gen3DPanel.Init(_selectedLayer);
        };

        // LayerButton
        var layerBtn = rootVisualElement.Q<Button>("LayerButton");
        layerBtn.clicked += () =>
        {
            var value = (layerPanel.style.display == DisplayStyle.None);
            layerPanel.style.display = (value) ? DisplayStyle.Flex : DisplayStyle.None;

            TryCollapseMenuPanels();
        };

        // 3DButton
        var Gen3DBtn = rootVisualElement.Q<Button>("3DButton");
        Gen3DBtn.clicked += () =>
        {
            gen3DPanel.Init(_selectedLayer);
            var value = (gen3DPanel.style.display == DisplayStyle.None);
            gen3DPanel.style.display = (value) ? DisplayStyle.Flex : DisplayStyle.None;

            TryCollapseMenuPanels();
        };

        LBSController.OnLoadLevel += (l) => _selectedLayer = null;

        drawManager.Redraw(levelData, mainView);
    }

    private void ShowinfoLayer(LBSLayer layer)
    {
        if (!layer.Equals(_selectedLayer))
        {
            OnSelectedLayerChange(layer);
        }
    }

    private void TryCollapseMenuPanels()
    {
        if (layerPanel?.style.display == DisplayStyle.None &&
            gen3DPanel?.style.display == DisplayStyle.None)
        {
            floatingPanelContent.style.display = DisplayStyle.None;
        }
        else
        {
            floatingPanelContent.style.display = DisplayStyle.Flex;
        }
    }

    public new void Repaint()
    {
        base.Repaint();
        drawManager.Redraw(levelData, mainView);
    }

    private void RefreshWindow()
    {
        mainView.Clear();
        this.rootVisualElement.Clear();
        Init();
    }

    private void OnLevelDataChange(LBSLevelData levelData)
    {
        noLayerSign.style.display = (levelData.Layers.Count <= 0) ? DisplayStyle.Flex : DisplayStyle.None;
    }

    private void OnSelectedLayerChange(LBSLayer layer)
    {
        _selectedLayer = layer;

        // Actualize ToolKit
        toolkit.Clear();
        toolkit.Init(layer); // esto no estas implementado (C:) se esta haciendo en inspectorManager.OnSelectedLayerChange(layer);

        // Actualize Inspector panel 
        inspectorManager.OnSelectedLayerChange(layer);

        // Actualize 3D panel
        gen3DPanel.Init(layer);

        // Actualize Bottom text
        selectedLabel.text = "selected: " + layer.Name;

    }

    private void OnInspectorUpdate()
    {
        OnWindowRepaint?.Invoke();
    }
}


