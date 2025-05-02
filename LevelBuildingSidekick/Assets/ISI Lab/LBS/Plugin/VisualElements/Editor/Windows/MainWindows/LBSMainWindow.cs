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
        private static NotifierViewer notifier;

        //Tool notif
        private VisualElement toolInformation;
        private static Label toolLabel;

        //Warning notif
        private static VisualElement warningNotification;
        private static Label warningLabel;

        [UxmlAttribute]
        private SplitView splitView;
        #endregion

        #region FIELDS-VIEWS
        
        // Visual Elements
        private MainView mainView; // work canvas
        
        private ButtonGroup toolPanel;
        private VisualElement extraPanel;
        private VisualElement noLayerSign;
        private Label selectedLabel;
        private static Label positionLabel;
        private VisualElement floatingPanelContent;
        private VisualElement toggleButtons;
        private VisualElement hideBar;
        private VisualElement inspectorPanel;
        private static VisualElement helpOverlay;

        // Panels
        private LayersPanel layerPanel;
        private Generator3DPanel gen3DPanel;
        private QuestsPanel questsPanel;
        
        [UxmlAttribute]
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
            Texture icon = LBSAssetMacro.LoadAssetByGuid<Texture>("e3db8d94c144db946ac8dd18f0bb7a9b");
            window.titleContent = new GUIContent("Level Builder", icon);
            window.minSize = new Vector2(800, 400);
        }
        
        public static void MessageNotify(string message, LogType logType = LogType.Log, int duration = 2)
        {       
            if (notifier == null) return; 
            notifier.SendNotification(message, logType, duration);
        }
        
        public static void MessageManipulator(string description)
        {       
            if (warningLabel == null) return;
            if (toolLabel == null) return;
            warningLabel.text = description;
        }
        
        public static void GridPosition(string gridPosition)
        {
            if (positionLabel == null) return; 
            positionLabel.text = gridPosition;
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

            if (LBS.loadedLevel == null)
            {
                if(levelData==null)
                {
                    LBS.loadedLevel = LBSController.CreateNewLevel("new file");
                } else
                {
                    backUpData = levelData;
                    LBS.loadedLevel = LBSController.CreateNewLevel("new file");
                    levelData = backUpData;
                }
            }

            levelData!.OnReload += () =>
            {
                layerPanel.ResetSelection();
                questsPanel.ResetSelection();
            };

            VisualTreeAsset visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("LBSMainWindow");
            visualTree.CloneTree(rootVisualElement);

            // LayerTemplate
            layerTemplates = DirectoryTools.GetScriptablesByType<LayerTemplate>();

            // SplitView
            splitView = rootVisualElement.Q<SplitView>("SplitView");
            if(splitView == null) Debug.LogError("Cannot find SplitView");

            // Help
            var helpOverlayAnchor = rootVisualElement.Q<VisualElement>("HelpOverlayAnchor");
            helpOverlay = new HintsController();
            helpOverlay.style.position = Position.Absolute; 
            helpOverlayAnchor.Add(helpOverlay);
            DisplayHelp();
            
            // Message Notifier
            notifier = rootVisualElement.Q<NotifierViewer>("NotifierViewer");
            var cleanButton = rootVisualElement.Q<VisualElement>("CleanNotificationsButton");
            var disableNotificationButton = rootVisualElement.Q<VisualElement>("DisableNotificationsButton");
            notifier.SetButtons(cleanButton, disableNotificationButton);

            //Tools
            toolInformation = rootVisualElement.Q<VisualElement>("ToolInformation");
            toolLabel = rootVisualElement.Q<Label>("ToolText");
            

            //Warning
            warningNotification = rootVisualElement.Q<VisualElement>("WarningNotification");
            warningLabel = rootVisualElement.Q<Label>("WarningText");
            warningNotification.visible = false;

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

            // ToolKitManager
            toolkit = rootVisualElement.Q<ToolKit>(name: "Toolkit");
            toolkit.OnEndAction += (l) =>
            {
                drawManager.RedrawLayer(l, mainView);
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

            // NoLayerSign
            noLayerSign = rootVisualElement.Q<VisualElement>("NoLayerSign");

            // SelectedLabel
            selectedLabel = rootVisualElement.Q<Label>("SelectedLabel");
            
            // PositionLabel
            positionLabel = rootVisualElement.Q<Label>("PositionLabel");
          
            #region Panels
            
            // InspectorContent
            inspectorManager = rootVisualElement.Q<LBSInspectorPanel>("InpectorPanel");
            
            // SubPanelScrollView
            var subPanelScrollView = rootVisualElement.Q<ScrollView>("SubPanelScrollView");
            subPanelScrollView.Q<VisualElement>("unity-content-and-vertical-scroll-container").pickingMode = PickingMode.Ignore;
            subPanelScrollView.Q<VisualElement>("unity-content-viewport").pickingMode = PickingMode.Ignore;
            subPanelScrollView.Q<VisualElement>("unity-content-container").pickingMode = PickingMode.Ignore;
            
            // ToolPanel
            toolPanel = rootVisualElement.Q<ButtonGroup>("ToolsGroup");
            
            // ExtraPanel
            extraPanel = rootVisualElement.Q<VisualElement>("ExtraPanel");
            
            // FloatingPanelContent
            floatingPanelContent = rootVisualElement.Q<VisualElement>("FloatingPanelContent");
            
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
                Debug.Log("OnAddLayer: " + sw.ElapsedMilliseconds + " ms");
                sw.Restart();
                DrawManager.Instance.AddContainer(layer);
                sw.Stop();
                Debug.Log("DrawManager.Instance.AddContainer(layer): " + sw.ElapsedMilliseconds + " ms");
            };
            layerPanel.OnRemoveLayer += (l) =>
            {
                drawManager.RemoveContainer(l);
                if(levelData.LayerCount==0)
                {
                    toolkit.Clear();
                }
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
            #endregion
            
            #region Extra Toolbar
            // LayerButton
            Toggle layerToggleButton = rootVisualElement.Q<Toggle>("LayerToggle");
            layerToggleButton.SetValueWithoutNotify(true);
            layerToggleButton.RegisterCallback<ChangeEvent<bool>>((_event) => {
                layerPanel.style.display = (layerToggleButton.value) ? DisplayStyle.Flex : DisplayStyle.None;
            });

            // Quest Panel Toggle
            Toggle toggleQuest = rootVisualElement.Q<Toggle>("QuestToggle");
            toggleQuest.RegisterCallback<ChangeEvent<bool>>( (evt) => {
                //var value = (questsPanel.style.display == DisplayStyle.None);
                questsPanel.style.display = (toggleQuest.value) ? DisplayStyle.Flex : DisplayStyle.None;
            });


            // 3D Generator Toggle
            Toggle toggleButton3D = rootVisualElement.Q<Toggle>("Gen3DToggle");
            //Toggle toggleButton3D = Gen3DBtn.Q<Toggle>("ToggleButton");
            toggleButton3D.RegisterCallback<ChangeEvent<bool>>((evt) =>
            {
                gen3DPanel.Init(_selectedLayer);
                //var value = (gen3DPanel.style.display == DisplayStyle.None);
                gen3DPanel.style.display = (toggleButton3D.value) ? DisplayStyle.Flex : DisplayStyle.None;
            });
            
            // Plus toggle
            Toggle toggleButtonPlus = rootVisualElement.Q<Toggle>("PlusToggle");
            
            
            // Toggle buttons only one active at a time
            toggleButtons = rootVisualElement.Q<VisualElement>("ToggleButtonContainer");
            
            Toggle layerDataButton = rootVisualElement.Q<Toggle>("LayerDataButton");
            layerDataButton.RegisterCallback<ClickEvent>((evt) =>
            {
                OnToggleButtonClick();
                layerDataButton.SetValueWithoutNotify(true);
                LBSInspectorPanel.ShowInspector(LBSInspectorPanel.DataTab);
            });
            
            Toggle behaviourButton = rootVisualElement.Q<Toggle>("BehaviourButton");
            behaviourButton.RegisterCallback<ClickEvent>((evt) =>
            {
                OnToggleButtonClick();
                behaviourButton.SetValueWithoutNotify(true);
                LBSInspectorPanel.ShowInspector(LBSInspectorPanel.BehavioursTab);
            });
            
            Toggle assistantButton = rootVisualElement.Q<Toggle>("AssistantButton");
            assistantButton.RegisterCallback<ClickEvent>((evt) =>
            {
                OnToggleButtonClick();
                assistantButton.SetValueWithoutNotify(true);
                LBSInspectorPanel.ShowInspector(LBSInspectorPanel.AssistantsTab);
            });

            Toggle tagsButton = rootVisualElement.Q<Toggle>("TagsButton");  
            tagsButton.RegisterCallback<ClickEvent>((evt) =>
            {
                OnToggleButtonClick();
                tagsButton.SetValueWithoutNotify(true);
                //LBSInspectorPanel.ShowInspector(LBSInspectorPanel.AssistantsTab);
            });
            
            Toggle bundlesButton = rootVisualElement.Q<Toggle>("BundlesButton");     
            bundlesButton.RegisterCallback<ClickEvent>((evt) =>
            {
                OnToggleButtonClick();
                bundlesButton.SetValueWithoutNotify(true);
                //LBSInspectorPanel.ShowInspector(LBSInspectorPanel.AssistantsTab);
            });
            
            #endregion
            
            //inspector panel 
            inspectorPanel = rootVisualElement.Q<VisualElement>("Inspector");
            // topBar
            hideBar = rootVisualElement.Q<VisualElement>("HideBar");
            // Hide toggle
            Button buttonHideInspector = rootVisualElement.Q<Button>("ButtonDisplayInspector");
            buttonHideInspector.RegisterCallback<ClickEvent>((evt) =>{
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
            
            
            layerPanel.OnSelectLayer += (l) => questsPanel.ResetSelection();
            questsPanel.OnSelectQuest += (l) => layerPanel.ResetSelection();
            
            LBSController.OnLoadLevel += (l) => _selectedLayer = null;
            

            
            // Init Data
            OnLevelDataChange(levelData);
            levelData.OnChanged += (lvl) =>
            {
                OnLevelDataChange(lvl);
            };
            
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
          
        }

        /// <summary>
        /// Called when the selected layer is changed.
        /// </summary>
        /// <param name="layer"></param>
        private void OnSelectedLayerChange(LBSLayer layer)
        {
            _selectedLayer = layer;
            
            inspectorManager.SetTarget(layer);
            
            toolkit.Clear();
            toolkit.Init(layer);
            toolkit.SetActiveWhithoutNotify(0);
            
            gen3DPanel.Init(layer);
            
            selectedLabel.text = "Selected: " + layer.Name;

        }
        public static void WarningManipulator(string description = null)
        {
            if (warningLabel == null) return;
            warningLabel.text = description;
            warningNotification.visible = description != null ? true : false;
        }

        public List<LBSLayer> GetLayers()
        {
            List<LBSLayer> layers = new List<LBSLayer>();
            if(layerPanel == null || layerPanel.data == null) return layers;
            return layerPanel.data.Layers;
        }

        private void OnToggleButtonClick()
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
            DrawManager.ReDraw();
            LBSInspectorPanel.ReDraw();
        }
        #endregion
        
    }

}
