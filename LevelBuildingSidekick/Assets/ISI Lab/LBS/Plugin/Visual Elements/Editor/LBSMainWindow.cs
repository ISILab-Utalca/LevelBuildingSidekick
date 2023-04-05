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
    public LBSLevelData levelData;

    // Selected
    private LBSLayer _selectedLayer;
    private string _selectedMode; // (??) esto deberia ser mas que un string?

    // Templates
    public List<LayerTemplate> layerTemplates;

    // Visual Elements
    private ButtonGroup toolPanel;
    private VisualElement extraPanel;
    private VisualElement noLayerSign;
    private ModeSelector modeSelector;
    private MainView mainView;
    private Label selectedLabel;
    private VisualElement floatingPanelContent;

    // Panels
    private LayersPanel layerPanel;
    private AIPanel aiPanel;
    private Generator3DPanel gen3DPanel;

    // Manager
    private ToolkitManager toolkitManager;
    private DrawManager drawManager;
    private LBSInspectorPanel inspectorManager;


    [MenuItem("ISILab/Level Building Sidekick", priority = 0)]
    public static void ShowWindow()
    {
        var window = GetWindow<LBSMainWindow>();
        Texture icon = Resources.Load<Texture>("Icons/Logo");
        window.titleContent = new GUIContent("Level builder", icon);
        window.minSize = new Vector2(800, 400);
    }

    private static LBSMainWindow _ShowWindow()
    {
        var window = GetWindow<LBSMainWindow>();
        Texture icon = Resources.Load<Texture>("Icons/Logo");
        window.titleContent = new GUIContent("Level builder", icon);
        return window;
    }

    public virtual void CreateGUI()
    {
        Init();
    }

    public void Init()
    {
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

        // ModeSelector
        modeSelector = rootVisualElement.Q<ModeSelector>("ModeSelector");
        modeSelector.OnSelectionChange += (mode) =>
        {
            OnApplyTrasformers(_selectedMode, mode);
            OnSelectedModeChange(mode, _selectedLayer);
        };

        // MainView 
        mainView = rootVisualElement.Q<MainView>("MainView");
        mainView.OnClearSelection = () =>
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
        toolkitManager = new ToolkitManager(ref toolPanel, ref modeSelector, ref mainView, ref inspectorManager, ref layerTemplates);
        toolkitManager.OnEndSomeAction += () =>
        {
            drawManager.RefreshView(ref _selectedLayer, levelData.Layers, _selectedMode);
        };

        // ToolBar
        var toolbar = rootVisualElement.Q<ToolBarMain>("ToolBar");
        toolbar.OnNewLevel += (data) =>
        {
            LevelBackUp.Instance().level = data;
            levelData = LevelBackUp.Instance().level.data;
            RefreshWindow();
        };
        toolbar.OnLoadLevel += (data) =>
        {
            LevelBackUp.Instance().level = data;
            levelData = LevelBackUp.Instance().level.data;
            RefreshWindow();
            drawManager.RefreshView(ref _selectedLayer, levelData.Layers, _selectedMode);
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
        levelData = LBSController.CurrentLevel.data;
        OnLevelDataChange(levelData);
        levelData.OnChanged += (lvl) => {
            OnLevelDataChange(lvl);
        };

        // LayerPanel
        layerPanel = new LayersPanel(ref levelData, ref layerTemplates);
        extraPanel.Add(layerPanel);
        layerPanel.style.display = DisplayStyle.Flex;
        layerPanel.OnSelectLayer += (layer) =>
        {
            if(!layer.Equals(_selectedLayer))
            {
                OnSelectedLayerChange(layer);
            }

            if (_selectedLayer != null)
            {
                var il = Reflection.MakeGenericScriptable(_selectedLayer);
                Selection.SetActiveObjectWithContext(il, il);
            }
        };
        layerPanel.OnLayerVisibilityChange += () =>
        {
            drawManager.RefreshView(ref _selectedLayer, levelData.Layers, _selectedMode);
        };

        // AIPanel
        aiPanel = new AIPanel();
        aiPanel.OnAIExecute += () =>
        {
            drawManager.RefreshView(ref _selectedLayer, levelData.Layers, _selectedMode);
        };
        aiPanel.OnEndExecute += () =>
        {
            OnSelectedLayerChange2(_selectedLayer);
        };

        extraPanel.Add(aiPanel);
        aiPanel.style.display = DisplayStyle.None;
        
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

        // IAButton
        var IABtn = rootVisualElement.Q<Button>("AIButton");
        IABtn.clicked += () =>
        {
            aiPanel.Init(ref _selectedLayer);
            var value = (aiPanel.style.display == DisplayStyle.None);
            aiPanel.style.display = (value) ? DisplayStyle.Flex : DisplayStyle.None;

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
    }

    private void TryCollapseMenuPanels()
    {
        if (layerPanel?.style.display == DisplayStyle.None &&
            aiPanel?.style.display == DisplayStyle.None &&
            gen3DPanel?.style.display == DisplayStyle.None)
        {
            floatingPanelContent.style.display = DisplayStyle.None;
        }
        else
        {
            floatingPanelContent.style.display = DisplayStyle.Flex;
        }
    }

    private void RefreshWindow()
    {
        mainView.Clear();
        this.rootVisualElement.Clear();
        Init();
        mainView.OnClearSelection?.Invoke();
    }

    public void OnLevelDataChange(LBSLevelData levelData)
    {
        noLayerSign.style.display = (levelData.Layers.Count <= 0) ? DisplayStyle.Flex : DisplayStyle.None;
        //modeSelector.style.display = (levelData.Layers.Count <= 0) ? DisplayStyle.None : DisplayStyle.Flex;
    }

    public void OnApplyTrasformers(string modeFrom, string modeTo)
    {
        var ModuleFrom = _selectedLayer.GetMode(layerTemplates, modeFrom).module;
        var ModuleTo = _selectedLayer.GetMode(layerTemplates, modeTo).module;

        var transformers = _selectedLayer.GetTrasformers(layerTemplates);
        var trans = transformers.Find(t => t.From.FullName.Equals(ModuleFrom) && t.To.FullName.Equals(ModuleTo)); // (!) lo de los fullname es parche ya que ".ModuleType no funciona"
        
        if(trans == null)
        {
            //Debug.LogWarning("No existe trasformador que trasforme de '" + ModuleFrom + "' a '" + ModuleTo); // (!!!)
        }
        else
        {
            trans.Switch(ref _selectedLayer);
        }
    }

    public void OnSelectedModeChange(string mode, LBSLayer layer)
    {
        _selectedLayer = layer;

        var oldMode = _selectedMode;
        _selectedMode = mode;
        var modes = _selectedLayer.GetToolkit(layerTemplates);

        // Init tools
        object tools = null;
        modes.TryGetValue(mode,out tools);
        var module = layer.GetModule(0); // (!!) implementar cuando se pueda seleccionar un modulo
        toolkitManager.SetTools(tools, ref levelData, ref layer, ref module);
        modeSelector.style.display = (levelData.Layers.Count <= 0) ? DisplayStyle.None : DisplayStyle.Flex;

        drawManager.RefreshView(ref _selectedLayer,levelData.Layers, _selectedMode);
    }

    public void OnSelectedLayerChange(LBSLayer layer)
    {
        _selectedLayer = layer;
        
        // actualize modes
        var modes = _selectedLayer.GetToolkit(layerTemplates);
        modeSelector.SetChoices(modes);
        modeSelector.Index = 0;
        modeSelector.style.display = DisplayStyle.Flex;
        OnSelectedModeChange(modes.Keys.First(), _selectedLayer);

        selectedLabel.text = "selected: " + layer.Name;
    }

    public void OnSelectedLayerChange2(LBSLayer layer) // esto es un parche se deberia ir cuando se mejore el paso de selected layer y los comportamientos
    {
        _selectedLayer = layer;

        // actualize modes
        var modes = _selectedLayer.GetToolkit(layerTemplates);
        modeSelector.SetChoices(modes);
        modeSelector.Index = modes.Count -1;
        modeSelector.style.display = DisplayStyle.Flex;
        OnSelectedModeChange(modes.Keys.Last(), _selectedLayer);

        selectedLabel.text = "selected: " + layer.Name;
    }
}
