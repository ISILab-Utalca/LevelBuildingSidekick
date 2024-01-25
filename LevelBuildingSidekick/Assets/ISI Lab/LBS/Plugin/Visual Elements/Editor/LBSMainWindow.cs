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
using static UnityEditor.Experimental.GraphView.GraphView;

// (!) Poner en un namespace
// (?) Cambiar el nombre de la clase por uno mejor? 
public class LBSMainWindow : EditorWindow 
{
    #region PROPERTIES
    private LBSLevelData levelData => LBS.LBS.loadedLevel.data;
    #endregion

    #region FIELDS
    // Selected
    private LBSLayer _selectedLayer;

    // Templates
    public List<LayerTemplate> layerTemplates;

    // Manager
    private ToolKit toolkit;
    private ToolKit questToolkit;
    private DrawManager drawManager;
    private LBSInspectorPanel inspectorManager;
    #endregion

    #region FIELDS-VIEWS
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
    private QuestsPanel questsPanel;
    private LayerInspector layerInspector;
    #endregion

    #region EVENTS
    public static Action OnWindowRepaint;
    #endregion

    #region STATIC METHODS
    [MenuItem("ISILab/Level Building Sidekick", priority = 0)]
    private static void ShowWindow()
    {
        var window = GetWindow<LBSMainWindow>();
        Texture icon = Resources.Load<Texture>("Icons/LBS_Logo1");
        window.titleContent = new GUIContent("Level builder", icon);
        window.minSize = new Vector2(800, 400);
    }
    #endregion

    #region METHODS
    public virtual void CreateGUI()
    {
        Init();
    }

    private void OnInspectorUpdate()
    {
        OnWindowRepaint?.Invoke();
    }

    /// <summary>
    /// Initialize the window.
    /// </summary>
    private void Init()
    {

        if (LBS.LBS.loadedLevel == null)
        {
            LBS.LBS.loadedLevel = LBSController.CreateNewLevel("new file", new Vector3(100, 100, 100));
        }

        levelData.OnReload += () =>
        {
            layerPanel.ResetSelection();
            questsPanel.ResetSelection();
        };

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
        inspectorManager.OnChangeTab += (s) =>
        {
            Debug.Log("inspectorManager.OnChangeTab");
        };

        // ToolKitManager
        toolkit = rootVisualElement.Q<ToolKit>(name: "Toolkit");
        toolkit.OnEndAction += (l) =>
        {
            // (!!) esta forma de dibujar, en donde se repinta todo, es la que no es eficiente,
            // hay que cambiarla a que repinte solo lo que este relacionado a las posciones editadas,
            // pero ahora quedo en que repintara, no todo, pero si toda la layer.
            //drawManager.RedrawLayer(l, mainView);
            drawManager.RedrawLevel(levelData, mainView); 
        };

        //QuestToolkit
        questToolkit = rootVisualElement.Q<ToolKit>(name: "QuestToolkit");

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
            drawManager.RedrawLevel(levelData, mainView);
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

        layerPanel.OnLayerVisibilityChange += (l) => {
            DrawManager.Instance.RedrawLevel(levelData, mainView);
        };
        layerPanel.OnSelectLayer += (layer) => { // esto llama implicitamente OnAddLayer
            OnSelectedLayerChange(layer);
        };
        layerPanel.OnAddLayer += (layer) => {
            OnSelectedLayerChange(layer);
            DrawManager.Instance.AddContainer(layer);
        }; 
        layerPanel.OnRemoveLayer += (l) => {
            drawManager.RemoveContainer(l);
        };

        // Gen3DPanel
        gen3DPanel = new Generator3DPanel();
        extraPanel.Add(gen3DPanel);
        gen3DPanel.style.display = DisplayStyle.None;
        gen3DPanel.OnExecute = () =>
        {
            gen3DPanel.Init(_selectedLayer);
        };

        //QuestsPanel
        questsPanel = new QuestsPanel(levelData);
        extraPanel.Add(questsPanel);    
        questsPanel.style.display = DisplayStyle.None;
        questsPanel.OnSelectQuest += OnSelectedLayerChange;

        // LayerButton
        var layerBtn = rootVisualElement.Q<Button>("LayerButton");
        layerBtn.clicked += () =>
        {
            var value = (layerPanel.style.display == DisplayStyle.None);
            layerPanel.style.display = (value) ? DisplayStyle.Flex : DisplayStyle.None;
        };

        // 3DButton
        var Gen3DBtn = rootVisualElement.Q<Button>("3DButton");
        Gen3DBtn.clicked += () =>
        {
            gen3DPanel.Init(_selectedLayer);
            var value = (gen3DPanel.style.display == DisplayStyle.None);
            gen3DPanel.style.display = (value) ? DisplayStyle.Flex : DisplayStyle.None;
        };

        var QuestBtn = rootVisualElement.Q<Button>("Quests");
        QuestBtn.clicked += () =>
        {
            var value = (questsPanel.style.display == DisplayStyle.None);
            questsPanel.style.display = (value) ? DisplayStyle.Flex : DisplayStyle.None;
        };

        layerPanel.OnSelectLayer += (l) => questsPanel.ResetSelection();
        questsPanel.OnSelectQuest += (l) => layerPanel.ResetSelection();

        LBSController.OnLoadLevel += (l) => _selectedLayer = null;

        drawManager.RedrawLevel(levelData, mainView);
    }

    /// <summary>
    /// Repaint the window.
    /// </summary>
    public new void Repaint()
    {
        base.Repaint();
        drawManager.RedrawLevel(levelData, mainView);
    }

    /// <summary>
    /// Refresh the window.
    /// </summary>
    private void RefreshWindow()
    {
        mainView.Clear();
        this.rootVisualElement.Clear();
        Init();
    }
    
    /// <summary>
    /// Called when the level data is changed.
    /// </summary>
    /// <param name="levelData"></param>
    private void OnLevelDataChange(LBSLevelData levelData)
    {
        noLayerSign.style.display = (levelData.Layers.Count <= 0) ? DisplayStyle.Flex : DisplayStyle.None;
        levelData.OnReload += () => layerPanel.ResetSelection();
        levelData.OnReload += () => questsPanel.ResetSelection();
    }

    /// <summary>
    /// Called when the selected layer is changed.
    /// </summary>
    /// <param name="layer"></param>
    private void OnSelectedLayerChange(LBSLayer layer)
    {
        _selectedLayer = layer;

        // Actualize Inspector panel 
        inspectorManager.SetTarget(layer);
        //inspectorManager.SetSelectedTab(layer.tabSelected);

        // Actualize ToolKit
        toolkit.Clear();
        toolkit.Init(layer); // esto no estas implementado (C:) se esta haciendo en inspectorManager.OnSelectedLayerChange(layer);
        toolkit.SetActiveWhithoutNotify(0);

        // Actualize 3D panel
        gen3DPanel.Init(layer);

        // Actualize Bottom text
        selectedLabel.text = "selected: " + layer.Name;

    }
    #endregion
}

