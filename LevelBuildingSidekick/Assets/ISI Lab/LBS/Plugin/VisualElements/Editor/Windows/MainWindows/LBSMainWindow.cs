using ISILab.Commons.Utility;
using ISILab.Commons.Utility.Editor;


using ISILab.LBS.Template;
using ISILab.LBS.VisualElements;
using ISILab.LBS.VisualElements.Editor;
using ISILab.Macros;
using LBS.Components;
using LBS.VisualElements;
using System;

using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using ISILab.Commons.VisualElements.Editor;
using ISILab.Extensions;
using ISILab.LBS.Manipulators;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;

namespace ISILab.LBS.Editor.Windows{

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class LBSMainWindow : EditorWindow
    {
        #region PROPERTIES

        private LBSLevelData levelData
        {
            get => LBS.loadedLevel.data;
            set => LBS.loadedLevel.data = value;
        }

        private LBSLevelData backUpData;

        #endregion

        #region DATA & STATE

        // Selected
        public LBSLayer _selectedLayer;

        // Templates
        public List<LayerTemplate> layerTemplates;

        #endregion

        #region MANAGERS

        private ToolKit toolkit;
        private DrawManager drawManager;
        private LBSInspectorPanel inspectorManager;

        #endregion

        #region NOTIFICATIONS

        // Tool notification
        private static Label toolLabel;

        // Warning notification
        private static VisualElement warningNotification;
        private static Label warningLabel;

        private static NotifierViewer notifier;

        #endregion

        #region MAIN VIEW

        // Work canvas
        private MainView mainView;

        // Help overlays
        private static VisualElement helpOverlay;
        private static VisualElement toggleButtons;
        private VisualElement noLayerSign;

// Grid position
        public static Vector2Int _gridPosition;

        #endregion

        #region UI LABELS

        private Label selectedLabel;
        private static Label positionLabel;

        #endregion

        #region PANELS & UI SECTIONS

        private LayersPanel layerPanel;
        private Generator3DPanel gen3DPanel;
        private VisualElement inspectorPanel;
        private VisualElement extraPanel;

        #endregion

        #region INSPECTORS

        [UxmlAttribute]
        private SplitView splitView;

        [UxmlAttribute]
        private LayerInspector layerInspector;

        #endregion

        #region TOGGLES

        private static Toggle layerDataButton;
        private static Toggle behaviourButton;
        private static Toggle assistantButton;

        #endregion


        #region EVENTS
        public static Action OnWindowRepaint;
        #endregion

        #region STATIC METHODS
        
        private static LBSMainWindow _instance;
        public static LBSMainWindow Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GetWindow<LBSMainWindow>();
                }
                return _instance;
            }
        }
        
        private void OnEnable()
        {
            _instance = this;
        }
        
        private void OnDisable()
        {
            if (_instance == this)
                _instance = null;
        }


        
        [MenuItem("Window/ISILab/Level Building Sidekick", priority = 0)]
        private static void ShowWindow()
        {
            var window = GetWindow<LBSMainWindow>();
            Texture icon = LBSAssetMacro.LoadAssetByGuid<Texture>("e3db8d94c144db946ac8dd18f0bb7a9b");
            window.titleContent = new GUIContent("Level Builder", icon);
            window.minSize = new Vector2(800, 400);
        }
        
        public static void MessageNotify(string message, LogType logType = LogType.Log, int duration = 3)
        {       
            if (notifier == null) return; 
            notifier.SendNotification(message, logType, duration);
        }
        
        public static void MessageManipulator(string description)
        {       
            if (toolLabel == null) return;
            toolLabel.text = description;
        }
        
        public static void GridPosition(Vector2 pos)
        {
            _gridPosition = pos.ToInt();
            if (positionLabel == null) return;
            string text = "Grid Position: " + pos.ToInt();
            positionLabel.text = text;
        }

        public static void DisplayHelp()
        {
            if(helpOverlay == null) return;
            helpOverlay.style.display = helpOverlay.style.display == DisplayStyle.None ?  DisplayStyle.Flex : DisplayStyle.None;
        }
        #endregion

        #region METHODS
        public virtual void CreateGUI()
        {
            Init();

            //KeyDownEvent
            //rootVisualElement.RegisterCallback<KeyDownEvent>(OnKeyDown, TrickleDown.TrickleDown);
            rootVisualElement.focusable = true;
            rootVisualElement.Focus();
        }

        private void OnKeyDown(KeyDownEvent evt)
        {
            
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
            #region LOAD & BACKUP LEVEL DATA

            if (LBS.loadedLevel == null)
            {
                if (levelData == null)
                {
                    LBS.loadedLevel = LBSController.CreateNewLevel();
                }
                else
                {
                    backUpData = levelData;
                    LBS.loadedLevel = LBSController.CreateNewLevel();
                    levelData = backUpData;
                }
            }

            levelData!.OnReload += () => layerPanel.ResetSelection();

            #endregion

            #region LOAD UI TREE

            VisualTreeAsset visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("LBSMainWindow");
            visualTree.CloneTree(rootVisualElement);

            #endregion

            #region LOAD SCRIPTABLES

            layerTemplates = DirectoryTools.GetScriptablesByType<LayerTemplate>();

            #endregion

            #region SPLIT VIEW SETUP

            splitView = rootVisualElement.Q<SplitView>("SplitView");
            if (splitView == null) Debug.LogError("Cannot find SplitView");

            #endregion

            #region HELP OVERLAY

            var helpOverlayAnchor = rootVisualElement.Q<VisualElement>("HelpOverlayAnchor");
            helpOverlay = new HintsController();
            helpOverlay.style.position = Position.Absolute;
            helpOverlayAnchor.Add(helpOverlay);
            DisplayHelp();

            #endregion

            #region NOTIFIER

            notifier = rootVisualElement.Q<NotifierViewer>("NotifierViewer");
            var cleanButton = rootVisualElement.Q<VisualElement>("CleanNotificationsButton");
            var disableNotificationButton = rootVisualElement.Q<VisualElement>("DisableNotificationsButton");
            notifier.SetButtons(cleanButton, disableNotificationButton);

            #endregion

            #region TOOL & WARNING INFO

            toolLabel = rootVisualElement.Q<VisualElement>("ToolInformation").Q<Label>("ToolText");
            warningNotification = rootVisualElement.Q<VisualElement>("WarningNotification");
            warningLabel = rootVisualElement.Q<Label>("WarningText");
            warningNotification.visible = false;

            #endregion

            #region MAIN VIEW

            mainView = rootVisualElement.Q<MainView>("MainView");
            mainView.OnClearSelection += () =>
            {
                if (_selectedLayer != null)
                {
                    var il = Reflection.MakeGenericScriptable(_selectedLayer);
                    Selection.SetActiveObjectWithContext(il, il);
                }
            };

            #endregion

            #region TOOLBAR

        var toolbar = rootVisualElement.Q<ToolBarMain>("ToolBar");
        toolbar.OnNewLevel += data =>
        {
            LBS.loadedLevel = data;
            RefreshWindow();
        };
        toolbar.OnLoadLevel += data =>
        {
            LBS.loadedLevel = data;
            RefreshWindow();
            drawManager.RedrawLevel(levelData);
        };
        rootVisualElement.RegisterCallback<KeyDownEvent>(evt =>
        {
            if (evt.ctrlKey && evt.keyCode == KeyCode.S)
            {
                LBSController.SaveFile();
                evt.StopPropagation();
            }    
        }, TrickleDown.TrickleDown);

            #endregion

            #region LABELS & MISC UI

            noLayerSign = rootVisualElement.Q<VisualElement>("NoLayerSign");
            selectedLabel = rootVisualElement.Q<Label>("SelectedLabel");
            positionLabel = rootVisualElement.Q<Label>("PositionLabel");

            #endregion

            #region PANELS - INSPECTOR, EXTRA, LAYERS, GENERATOR

            inspectorManager = rootVisualElement.Q<LBSInspectorPanel>("InpectorPanel");
            inspectorManager.InitTabs(ref layerTemplates);
        
            var subPanelScrollView = rootVisualElement.Q<ScrollView>("SubPanelScrollView");
            subPanelScrollView.Q<VisualElement>("unity-content-and-vertical-scroll-container").pickingMode = PickingMode.Ignore;
            subPanelScrollView.Q<VisualElement>("unity-content-viewport").pickingMode = PickingMode.Ignore;
            subPanelScrollView.Q<VisualElement>("unity-content-container").pickingMode = PickingMode.Ignore;

            extraPanel = rootVisualElement.Q<VisualElement>("ExtraPanel");

            layerPanel = new LayersPanel(levelData, ref layerTemplates);
            extraPanel.Add(layerPanel);
            layerPanel.style.display = DisplayStyle.Flex;

            layerPanel.OnLayerVisibilityChange += _ => DrawManager.Instance.RedrawLevel(levelData);
            layerPanel.OnLayerOrderChange += _ => DrawManager.Instance.RedrawLevel(levelData, true);
            layerPanel.OnSelectLayer += OnSelectedLayerChange;
            layerPanel.OnAddLayer += layer =>
            {
                var sw = new Stopwatch();
                sw.Start();
             //   sw.Stop(); Debug.Log("OnAddLayer: " + sw.ElapsedMilliseconds + " ms");
                sw.Restart();
                DrawManager.Instance.AddContainer(layer);
              //  sw.Stop(); Debug.Log("DrawManager.Instance.AddContainer: " + sw.ElapsedMilliseconds + " ms");
            };
            layerPanel.OnRemoveLayer += l =>
            {
                //      drawManager.RemoveContainer(l);
                if (levelData.LayerCount != 0) return;
            
                toolkit.Clear();
                OnSelectedLayerChange(null);
            };

            gen3DPanel = new Generator3DPanel();
            extraPanel.Add(gen3DPanel);
            gen3DPanel.style.display = DisplayStyle.None;
            gen3DPanel.OnExecute = () => gen3DPanel.Init(_selectedLayer);

            #endregion

            #region EXTRA TOOLBAR TOGGLES

            var layerToggleButton = rootVisualElement.Q<Toggle>("LayerToggle");
            layerToggleButton.SetValueWithoutNotify(true);
            layerToggleButton.RegisterCallback<ChangeEvent<bool>>(_ =>
            {
                layerPanel.style.display = layerToggleButton.value ? DisplayStyle.Flex : DisplayStyle.None;
            });
        
            var toggleButton3D = rootVisualElement.Q<Toggle>("Gen3DToggle");
            toggleButton3D.RegisterCallback<ChangeEvent<bool>>(_ =>
            {
                gen3DPanel.Init(_selectedLayer);
                gen3DPanel.style.display = toggleButton3D.value ? DisplayStyle.Flex : DisplayStyle.None;
            });

            toggleButtons = rootVisualElement.Q<VisualElement>("ToggleButtonContainer");

            layerDataButton = rootVisualElement.Q<Toggle>("LayerDataButton");
            behaviourButton = rootVisualElement.Q<Toggle>("BehaviourButton");
            assistantButton = rootVisualElement.Q<Toggle>("AssistantButton");

            layerDataButton.RegisterCallback<ClickEvent>(_ => ChangeInspectorPanelTab(layerDataButton));
            behaviourButton.RegisterCallback<ClickEvent>(_ => ChangeInspectorPanelTab(behaviourButton));
            assistantButton.RegisterCallback<ClickEvent>(_ => ChangeInspectorPanelTab(assistantButton));

            var tagsButton = rootVisualElement.Q<Toggle>("TagsButton");
            tagsButton.RegisterCallback<ClickEvent>(_ =>
            {
                OnToggleButtonClick();
                tagsButton.SetValueWithoutNotify(true);
            });

            var bundlesButton = rootVisualElement.Q<Toggle>("BundlesButton");
            bundlesButton.RegisterCallback<ClickEvent>(_ =>
            {
                OnToggleButtonClick();
                bundlesButton.SetValueWithoutNotify(true);
            });

            #endregion

            #region INSPECTOR TOGGLE BUTTON

            inspectorPanel = rootVisualElement.Q<VisualElement>("Inspector");
            var buttonHideInspector = rootVisualElement.Q<Button>("ButtonDisplayInspector");
            buttonHideInspector.RegisterCallback<ClickEvent>(_ =>
            {
                if (inspectorPanel.ClassListContains("lbs_inspectorhide"))
                {
                    inspectorPanel.RemoveFromClassList("lbs_inspectorhide");
                    splitView.fixedPaneInitialDimension = 400f;
                }
                else
                {
                    inspectorPanel.AddToClassList("lbs_inspectorhide");
                    splitView.fixedPaneInitialDimension = 80f;
                }
                splitView.MarkDirtyRepaint();
            });

            #endregion

            #region TOOLKIT

            toolkit = rootVisualElement.Q<ToolKit>("Toolkit");

            #endregion
        
            #region MAIN INIT & EVENTS

            LBSController.OnLoadLevel += _ => _selectedLayer = null;
            OnLevelDataChange(levelData);
            levelData.OnChanged += OnLevelDataChange;

        drawManager = new DrawManager();
        inspectorManager.CreateContainers(levelData, mainView);
        drawManager.RedrawLevel(levelData);

            #endregion
        }


        /// <summary>
        /// Called when changing tabs from the toggle buttons in this class
        /// </summary>
        /// <param name="toggleVe"></param>
        private void ChangeInspectorPanelTab(Toggle toggleVe)
        {
            OnToggleButtonClick();
            toggleVe.SetValueWithoutNotify(true);
            if(toggleVe == layerDataButton) LBSInspectorPanel.ActivateDataTab();
            if(toggleVe == behaviourButton) LBSInspectorPanel.ActivateBehaviourTab();
            if(toggleVe == assistantButton) LBSInspectorPanel.ActivateAssistantTab();
        }

        /// <summary>
        /// Activates visually the corresponding toggle button, only call this from inspector panel
        /// </summary>
        /// <param name="panel"></param>
        public static void InspectorToggleButtonChange(string panel)
        {
            Toggle toggleVe = null;
            if(panel == LBSInspectorPanel.DataTab) toggleVe = layerDataButton; 
            if(panel == LBSInspectorPanel.BehavioursTab) toggleVe = behaviourButton; 
            if(panel == LBSInspectorPanel.AssistantsTab) toggleVe = assistantButton;
            if (toggleVe is null) return;
            
            OnToggleButtonClick();
            toggleVe.SetValueWithoutNotify(true);
        }

        /// <summary>
        /// Repaint the window.
        /// </summary>
        public new void Repaint()
        {
            base.Repaint();
            drawManager.RedrawLevel(levelData);
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
          
        }

        /// <summary>
        /// Called when the selected layer is changed.
        /// </summary>
        /// <param name="layer"></param>
        private void OnSelectedLayerChange(LBSLayer layer)
        {
            if (_selectedLayer is not null)
            {
                _selectedLayer.OnChangeUpdate();

            }
            _selectedLayer = layer;
           
            toolkit.Clear();
            inspectorManager.SetTarget(layer);
            toolkit.SetActive(typeof(SelectManipulator));
            toolkit.SetSeparators();
            
            gen3DPanel.Init(layer);
            
            string layerName = layer is not null ? layer.Name : "-";
            selectedLabel.text = "Selected: " + layerName;

        }
        public static void WarningManipulator(string description = null)
        {
            if (warningLabel == null) return;
            warningLabel.text = description;
            warningNotification.visible = description != null;
        }

        public List<LBSLayer> GetLayers()
        {
            List<LBSLayer> layers = new List<LBSLayer>();
            if(layerPanel == null || layerPanel.Data == null) return layers;
            return layerPanel.Data.Layers;
        }

        private static void OnToggleButtonClick()
        {
            foreach (var child in toggleButtons.Children())
            {
                if (child is Toggle toggle)
                {
                    toggle.SetValueWithoutNotify(false); // Deselect
                }
            }
        }
        
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
            if(_selectedLayer is not null ) DrawManager.Instance.RedrawLayer(_selectedLayer);
            else DrawManager.ReDraw();
            
            LBSInspectorPanel.ReDraw();
        }
        #endregion
        
    }

}
