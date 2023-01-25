using LBS;
using LBS.Components;
using LBS.ElementView;
using LBS.VisualElements;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

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

    // Panels
    private LayersPanel layerPanel;
    private AIPanel aiPanel;
    private Generator3DPanel gen3DPanel;

    // Manager
    private ToolkitManager toolkitManager;
    private DrawManager drawManager;
    private InspectorManager inspectorManager;


    [MenuItem("ISILab/LBS plugin/Main window", priority = 0)]
    public static void ShowWindow()
    {
        var window = GetWindow<LBSMainWindow>();
        Texture icon = Resources.Load<Texture>("Icons/Logo");
        window.titleContent = new GUIContent("Level builder", icon);
    }

    public virtual void CreateGUI()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("LBSMainWindow");
        visualTree.CloneTree(rootVisualElement);

        // LayerTemplate
        layerTemplates = Utility.DirectoryTools.GetScriptablesByType<LayerTemplate>();

        // ToolPanel
        toolPanel = rootVisualElement.Q<ButtonGroup>("ToolsGroup");

        // ModeSelector
        modeSelector = rootVisualElement.Q<ModeSelector>("ModeSelector");
        modeSelector.OnSelectionChange += (mode) =>
        {
            OnSelectedModeChange(mode, _selectedLayer);
        };

        // MainView 
        mainView = rootVisualElement.Q<MainView>("MainView");

        // DrawManager
        drawManager = new DrawManager(ref mainView, ref layerTemplates);

        // InspectorContent
        var inspector = rootVisualElement.Q<VisualElement>("InspectorContent");

        // InspectorManager
        inspectorManager = new InspectorManager(ref mainView, inspector);

        // ToolKitManager
        toolkitManager = new ToolkitManager(ref toolPanel, ref modeSelector, ref mainView,ref inspectorManager, ref layerTemplates);
        toolkitManager.OnEndSomeAction += () =>
        {
            drawManager.RefreshView(ref _selectedLayer, _selectedMode);
        };

        // ToolBar
        var toolbar = rootVisualElement.Q<ToolBarMain>("ToolBar");

        // ExtraPanel
        extraPanel = rootVisualElement.Q<VisualElement>("ExtraPanel");

        // NoLayerSign
        noLayerSign = rootVisualElement.Q<VisualElement>("NoLayerSign");

        // SelectedLabel
        selectedLabel = rootVisualElement.Q<Label>("SelectedLabel");

        // Init Data
        levelData = LBSController.CurrentLevel.data; // (!) cargar archivo aqui de alguna forma
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
            OnSelectedLayerChange(layer);
        };

        // AIPanel
        aiPanel = new AIPanel(levelData);
        extraPanel.Add(aiPanel);
        aiPanel.style.display = DisplayStyle.None;

        // Gen3DPanel
        gen3DPanel = new Generator3DPanel(levelData);
        extraPanel.Add(gen3DPanel);
        gen3DPanel.style.display = DisplayStyle.None;


        // LayerButton
        var layerBtn = rootVisualElement.Q<Button>("LayerButton");
        layerBtn.clicked += () =>
        {
            var value = (layerPanel.style.display == DisplayStyle.None);
            layerPanel.style.display = (value) ? DisplayStyle.Flex : DisplayStyle.None;
        };

        // IAButton
        var IABtn = rootVisualElement.Q<Button>("IAButton");
        IABtn.clicked += () =>
        {
            var value = (aiPanel.style.display == DisplayStyle.None);
            aiPanel.style.display = (value) ? DisplayStyle.Flex : DisplayStyle.None;
        };

        // 3DButton
        var Gen3DBtn = rootVisualElement.Q<Button>("3DButton");
        Gen3DBtn.clicked += () =>
        {
            var value = (gen3DPanel.style.display == DisplayStyle.None);
            gen3DPanel.style.display = (value) ? DisplayStyle.Flex : DisplayStyle.None;
        };
    }

    public void OnLevelDataChange(LBSLevelData levelData)
    {
        noLayerSign.style.display = (levelData.Layers.Count <= 0) ? DisplayStyle.Flex : DisplayStyle.None;
        modeSelector.style.display = (levelData.Layers.Count <= 0) ? DisplayStyle.None : DisplayStyle.Flex;
    }

    public void OnSelectedModeChange(string mode, LBSLayer layer)
    {
        _selectedMode = mode;
        var modes = _selectedLayer.GetModes(layerTemplates);
        object tools;
        modes.TryGetValue(mode,out tools);

        var module = layer.GetModule(0); // (!!) implementar cuando se pueda seleccionar un modulo
        toolkitManager.SetTools(tools, ref levelData, ref layer, ref module);
    }

    public void OnSelectedLayerChange(LBSLayer layer)
    {
        _selectedLayer = layer;
        
        // actualize modes
        var modes = _selectedLayer.GetModes(layerTemplates);
        modeSelector.SetChoices(modes);
        modeSelector.Index = 0;
        modeSelector.style.display = DisplayStyle.Flex;
        OnSelectedModeChange(modes.Keys.First(), _selectedLayer);

        selectedLabel.text = "selected: " + layer.Name;

        // (?) Actualize IAs?

        // (?) Actualize gen3D?

    }
}
