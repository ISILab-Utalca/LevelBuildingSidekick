using ISILab.Commons.Utility;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS;
using ISILab.LBS.Template;
using ISILab.LBS.VisualElements;
using ISILab.LBS.VisualElements.Editor;
using LBS.Components;
using LBS.VisualElements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;

public class LBSMainWindow : EditorWindow
{
    #region PROPERTIES
    private LBSLevelData levelData => ISILab.LBS.LBS.loadedLevel.data;
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
    [MenuItem("Window/ISILab/Level Building Sidekick", priority = 0)]
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

        if (ISILab.LBS.LBS.loadedLevel == null)
        {
            ISILab.LBS.LBS.loadedLevel = LBSController.CreateNewLevel("new file");
        }

        levelData.OnReload += () =>
        {
            layerPanel.ResetSelection();
            questsPanel.ResetSelection();
        };

        VisualTreeAsset visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("LBSMainWindow");
        visualTree.CloneTree(rootVisualElement);

        // LayerTemplate
        layerTemplates = DirectoryTools.GetScriptablesByType<LayerTemplate>();

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
            ISILab.LBS.LBS.loadedLevel = data;
            RefreshWindow();
        };
        toolbar.OnLoadLevel += (data) =>
        {
            ISILab.LBS.LBS.loadedLevel = data;
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
        levelData.OnChanged += (lvl) =>
        {
            OnLevelDataChange(lvl);
        };

        // LayerPanel
        layerPanel = new LayersPanel(levelData, ref layerTemplates);
        extraPanel.Add(layerPanel);
        layerPanel.style.display = DisplayStyle.Flex;

        layerPanel.OnLayerVisibilityChange += (l) =>
        {
            DrawManager.Instance.RedrawLevel(levelData, mainView);
        };
        layerPanel.OnSelectLayer += (layer) =>
        {
            OnSelectedLayerChange(layer);
        };
        layerPanel.OnAddLayer += (layer) =>
        {
            var sw = new Stopwatch();
            sw.Start();
            OnSelectedLayerChange(layer);
            sw.Stop();
            Debug.Log("OnAddLayer: " + sw.ElapsedMilliseconds);

            sw.Restart();
            DrawManager.Instance.AddContainer(layer);
            sw.Stop();
            Debug.Log("DrawManager.Instance.AddContainer(layer): " + sw.ElapsedMilliseconds);
        };
        layerPanel.OnRemoveLayer += (l) =>
        {
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
        Toggle layerToggleButton = rootVisualElement.Q<Toggle>("LayerToggleButton");
        layerToggleButton.SetValueWithoutNotify(true);
        layerToggleButton.RegisterCallback<ChangeEvent<bool>>((_event) => {
            layerPanel.style.display = (layerToggleButton.value) ? DisplayStyle.Flex : DisplayStyle.None;
        });

        // Quest Panel Toggle
        Toggle toggleQuest = rootVisualElement.Q<Toggle>("QuestToggleButton");
        toggleQuest.RegisterCallback<ChangeEvent<bool>>( (evt) => {
            //var value = (questsPanel.style.display == DisplayStyle.None);
            questsPanel.style.display = (toggleQuest.value) ? DisplayStyle.Flex : DisplayStyle.None;
        });


        // 3D Generator Toggle
        Toggle toggleButton3D = rootVisualElement.Q<Toggle>("Gen3DToggleButton");
        //Toggle toggleButton3D = Gen3DBtn.Q<Toggle>("ToggleButton");
        toggleButton3D.RegisterCallback<ChangeEvent<bool>>((evt) =>
        {
            gen3DPanel.Init(_selectedLayer);
            //var value = (gen3DPanel.style.display == DisplayStyle.None);
            gen3DPanel.style.display = (toggleButton3D.value) ? DisplayStyle.Flex : DisplayStyle.None;
        });



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
        var layersIsEmpty = levelData.Layers.Count <= 0;
        var questIsEmpty = levelData.Quests.Count <= 0;

        noLayerSign.style.display = (layersIsEmpty && questIsEmpty) ? DisplayStyle.Flex : DisplayStyle.None;
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

        // Actualize ToolKit
        toolkit.Clear();

        toolkit.Init(layer);
        toolkit.SetActiveWhithoutNotify(0);

        // Actualize 3D panel
        gen3DPanel.Init(layer);

        // Actualize Bottom text
        selectedLabel.text = "selected: " + layer.Name;

    }
    #endregion

    private void OnFocus()
    {
        Undo.undoRedoPerformed += UNDO;
    }

    private void OnLostFocus()
    {
        Undo.undoRedoPerformed -= UNDO;
    }

    private void UNDO()
    {
        DrawManager.ReDraw();
        LBSInspectorPanel.ReDraw();
    }
}