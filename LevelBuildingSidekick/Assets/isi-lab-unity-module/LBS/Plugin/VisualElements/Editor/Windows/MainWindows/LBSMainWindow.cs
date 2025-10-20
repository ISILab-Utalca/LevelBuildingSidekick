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
using ISILab.LBS.Settings;
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
        public static NotifierViewer notifier;

        #endregion

        #region MAIN VIEW

        // Work canvas
        private MainView mainView;

        // Help overlays
        private static VisualElement helpOverlay;
        private VisualElement noLayerSign;
        private LBSSideBarPanel sideBarPanel;

// Grid position
        public static Vector2Int _gridPosition;

        #endregion

        #region UI LABELS

        private Label selectedLabel;
        private static Label positionLabel;

        #endregion

        #region PANELS & UI SECTIONS VISUALELEMENTS

        public LayersPanel layerPanel;
        public Generator3DPanel gen3DPanel;
        public VisualElement extraPanel;
        public VisualElement inspectorPanelContainer;

        private VisualElement helpOverlayAnchor;
        private ToolBarMain topToolBar;
        private InfoToolbar infoToolBar;
        private LBSWaitTaskOverlay taskOverlay;

        private ScrollView subPanelScrollView;
        
        [UxmlAttribute]
        private SplitView splitView;
        [UxmlAttribute]
        private LayerInspector layerInspector;


        #endregion


        #region EVENTS
        public static Action OnWindowRepaint;
        public static Action OnLayerChange;
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
        
        #endregion

        public LBSMainWindow(): base()
        {
            // UI can't be referenced here because inherit from a scriptable object!
            //Debug.Log("[Main Window] - Constructor");
        }
        
        private void OnEnable()
        {
            Debug.Log("[Main Window] - OnEnable");
            _instance = this;
            
            #region LOAD UI TREE
            //MainWindows UXML 
            VisualTreeAsset visualTree = LBSAssetMacro.LoadAssetByGuid<VisualTreeAsset>("352a58bb499307540a1e69ea48063f29");
            visualTree.CloneTree(rootVisualElement);
            #endregion
            
            splitView = rootVisualElement.Q<SplitView>("SplitView");
            
            helpOverlayAnchor = rootVisualElement.Q<VisualElement>("HelpOverlayAnchor");
            
            topToolBar = rootVisualElement.Q<ToolBarMain>("ToolBar");
            infoToolBar =  rootVisualElement.Q<InfoToolbar>("InfoToolbar");
            
            mainView = rootVisualElement.Q<MainView>("MainView");
            
            noLayerSign = rootVisualElement.Q<VisualElement>("NoLayerSign");
            selectedLabel = rootVisualElement.Q<Label>("SelectedLabel");
            positionLabel = rootVisualElement.Q<Label>("PositionLabel");
            
            notifier = rootVisualElement.Q<NotifierViewer>("NotifierViewer");
            
            inspectorPanelContainer = rootVisualElement.Q<VisualElement>("Inspector");
            inspectorManager = rootVisualElement.Q<LBSInspectorPanel>("InspectorPanel");
            sideBarPanel = rootVisualElement.Q<LBSSideBarPanel>("LBSSideBarPanel");
            
            subPanelScrollView = rootVisualElement.Q<ScrollView>("SubPanelScrollView");
            
            extraPanel = rootVisualElement.Q<VisualElement>("ExtraPanel");
            taskOverlay = rootVisualElement.Q<LBSWaitTaskOverlay>("TaskOverlay");
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
        
        
        #region METHODS
        public virtual void CreateGUI()
        {
            Debug.Log("[Main Window] - CreateGUI");
            Init();
            rootVisualElement.focusable = true;
            rootVisualElement.Focus();
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
            #region LOAD SCRIPTABLES TEMPLATE
            layerTemplates = DirectoryTools.GetScriptablesByType<LayerTemplate>();
            layerTemplates.Sort((a, b) => a.order.CompareTo(b.order));
            #endregion

            #region HELP OVERLAY

            DisplayHelp();

            #endregion

            #region NOTIFIER TOOLBAR

            infoToolBar.Bind(this);

            #endregion
            

            #region MAIN VIEW
            
            mainView.OnClearSelection += () =>
            {
                if (_selectedLayer != null)
                {
                    var il = Reflection.MakeGenericScriptable(_selectedLayer);
                    Selection.SetActiveObjectWithContext(il, il);
                }
            };

            #endregion

            #region TOOLBARS
            topToolBar.Bind(this);
            
            topToolBar.OnLoadLevel += data =>
            {
                LBS.loadedLevel = data;
                RefreshWindow();
                //drawManager.RedrawLevel(levelData);
            };
            
            topToolBar.OnThemeChanged += data => ChangeTheme(data);
            OnLayerChange += topToolBar.LevelChange;

            //S = SAVE = Save level
            rootVisualElement.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.ctrlKey && evt.keyCode == KeyCode.S)
                {
                    topToolBar.SaveLevel();
                    evt.StopPropagation();
                }    
            }, TrickleDown.TrickleDown);
            //O = OPEN = Load level
            rootVisualElement.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.ctrlKey && evt.keyCode == KeyCode.O)
                {
                    topToolBar.LoadLevel();
                    evt.StopPropagation();
                }
            }, TrickleDown.TrickleDown);
            //N = NEW = New level
            rootVisualElement.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.ctrlKey && evt.keyCode == KeyCode.N)
                {
                    topToolBar.NewLevel();
                    evt.StopPropagation();
                }
            }, TrickleDown.TrickleDown);
            #endregion


            #region PANELS - INSPECTOR, EXTRA, LAYERS, GENERATOR


            inspectorManager.InitTabs(ref layerTemplates);
            
            subPanelScrollView.Q<VisualElement>("unity-content-and-vertical-scroll-container").pickingMode = PickingMode.Ignore;
            subPanelScrollView.Q<VisualElement>("unity-content-viewport").pickingMode = PickingMode.Ignore;
            subPanelScrollView.Q<VisualElement>("unity-content-container").pickingMode = PickingMode.Ignore;
            
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

            #region SIDE TOOLBAR TOGGLES
            sideBarPanel?.Bind(this);
            #endregion

            #region INSPECTOR TOGGLE BUTTON

            
            var buttonHideInspector = rootVisualElement.Q<Button>("ButtonDisplayInspector");
            buttonHideInspector.RegisterCallback<ClickEvent>(_ =>
            {
                if (inspectorPanelContainer.ClassListContains("lbs_inspectorhide"))
                {
                    inspectorPanelContainer.RemoveFromClassList("lbs_inspectorhide");
                    splitView.fixedPaneInitialDimension = 400f;
                }
                else
                {
                    inspectorPanelContainer.AddToClassList("lbs_inspectorhide");
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
            
            
            #region THEME SET
            ChangeTheme(LBSSettings.Instance.view.LBSTheme);
            #endregion
        }


        /// <summary>
        /// Called when changing tabs from the toggle buttons in this class
        /// </summary>
        /// <param name="toggleVe"></param>
        public void ChangeInspectorPanelTab(Toggle toggleVe)
        {
            sideBarPanel.OnToggleButtonClick();
            toggleVe.SetValueWithoutNotify(true);
            if(toggleVe == sideBarPanel.layerDataTab) LBSInspectorPanel.ActivateDataTab();
            if(toggleVe == sideBarPanel.behaviorTab) LBSInspectorPanel.ActivateBehaviourTab();
            if(toggleVe == sideBarPanel.assistantTab) LBSInspectorPanel.ActivateAssistantTab();
        }

        /// <summary>
        /// Activates visually the corresponding toggle button, only call this from inspector panel
        /// </summary>
        /// <param name="panel"></param>
        public void InspectorToggleButtonChange(string panel)
        {
            if (sideBarPanel == null)
            {
                sideBarPanel = rootVisualElement.Q<LBSSideBarPanel>("SideBarPanel");
            }
            Toggle toggleVe = null;
            if(panel == LBSInspectorPanel.DataTab) toggleVe = sideBarPanel.layerDataTab; 
            if(panel == LBSInspectorPanel.BehavioursTab) toggleVe = sideBarPanel.behaviorTab; 
            if(panel == LBSInspectorPanel.AssistantsTab) toggleVe = sideBarPanel.assistantTab;
            if (toggleVe is null) return;
            
            sideBarPanel.OnToggleButtonClick();
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
        public void RefreshWindow()
        {
            mainView.Clear();
            this.rootVisualElement.Clear();
            
            //Repaint();
            OnDisable();
            OnEnable();
            CreateGUI();
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
                _selectedLayer.OnChange -= NotifyChange;
                _selectedLayer.OnChangeUpdate();
            }
            _selectedLayer = layer;
            
            if (_selectedLayer is not null)
            {
                _selectedLayer.OnChange += NotifyChange;
            }
            
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

        public static void NotifyChange()
        {
            OnLayerChange?.Invoke();
        }

        public List<LBSLayer> GetLayers()
        {
            List<LBSLayer> layers = new List<LBSLayer>();
            if(layerPanel == null || layerPanel.Data == null) return layers;
            return layerPanel.Data.Layers;
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
            //So for some reason, THIS executes about 3-4 times every time it's executed. I have NO idea why this is and at this point I'm too scared to ask. -Alice
            if (_selectedLayer is not null)
            {
                _selectedLayer.OnChangeUpdate();
                DrawManager.Instance.UpdateLayer(_selectedLayer);
            }
            else DrawManager.ReDraw();
            
            LBSInspectorPanel.ReDraw();
        }
        
        // Allows to send notifications from threads other than main. Seems to work, although I'm not completely sure it's safe.
        public static void MessageNotifyDelayed(string message, LogType logType = LogType.Log, int duration = 3)
        {
            EditorApplication.delayCall += () => MessageNotify(message, logType, duration);
        }
        
        public static void MessageNotify(string message, LogType logType = LogType.Log, int duration = 3)
        {
            notifier?.SendNotification(message, logType, duration);
        }
        
        public void MessageManipulator(string description)
        {
            infoToolBar?.SmallMessage(description);

        }
        
        public static void GridPosition(Vector2 pos)
        {
            _gridPosition = pos.ToInt();
            if (positionLabel == null) return;
            string text = "Grid Position: " + pos.ToInt();
            positionLabel.text = text;
        }

        public void DisplayHelp()
        {
            helpOverlay = new HintsController();
            helpOverlay.style.position = Position.Absolute;
            helpOverlayAnchor.Add(helpOverlay);
            if(helpOverlay == null) return;
            helpOverlay.style.display = helpOverlay.style.display == DisplayStyle.None ?  DisplayStyle.Flex : DisplayStyle.None;
        }
        #endregion
        
        public void ChangeTheme(LBSSettings.Interface.InterfaceTheme _newTheme)
        {
            switch (_newTheme)
            {
                case  LBSSettings.Interface.InterfaceTheme.Light:
                    rootVisualElement.ClearClassList();
                    rootVisualElement.AddToClassList("light");
                    //Repaint();
                    break;
                case  LBSSettings.Interface.InterfaceTheme.Dark:
                    rootVisualElement.ClearClassList();
                    rootVisualElement.AddToClassList("dark");
                    //Repaint();
                    break;
                case LBSSettings.Interface.InterfaceTheme.Alt:
                    rootVisualElement.ClearClassList();
                    rootVisualElement.AddToClassList("alt");
                    //Repaint();
                    break;
                default:
                    break;
            }
        }
        
    }

}
