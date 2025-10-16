using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using ISILab.LBS.Assistants;
using ISILab.LBS.VisualElements.Editor;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using ISILab.LBS.CustomComponents;
using ISILab.LBS.Editor.Windows;
using ISILab.LBS.Manipulators;
using ISILab.LBS.Modules;
using LBS.Components;
using LBS.VisualElements;

namespace ISILab.LBS.Editor
{
    [LBSCustomEditor("QuestAssistant", typeof(QuestAssistant))]
    public class QuestAssistantEditor : LBSCustomEditor, IToolProvider
    {
        
        #region FIELDS
        private static class UIElementNames
        {
            public const string VisualTree = "QuestAssistantEditor";
            public const string LockedLayerContainer = "LockedLayerContainer";
            public const string LayerList = "LayerList";
            public const string SuggestionList = "SuggestionList";
            public const string AddLayerButton = "AddLayerButton";
            public const string GenerateSuggestionsButton = "GenerateSuggestions";
            public const string ConnectAll = "ConnectAll";
            public const string SuggestionField = "SuggestionField";
            public const string NoSuggestionPanel = "NoSuggestionPanel";
            public const string RemoveAllSuggestions = "RemoveSuggestions";
        }

        private QuestAssistant _questAssistant;
        private QuestGraph _questGraph;
        private ListView _layerList;
        private ListView _suggestionList;
        private Button _addLayerButton;
        private Button _autoConnectButton;
        private Button _generateSuggestionsButton;
        private Button _removeSuggestionsButton;
        private VisualElement _lockedContextEntryContainer;
        private LBSPanelTextIcon _noSuggestionPanel;
        private LBSCustomUnsignedIntegerField _suggestionField;
 
        private CancellationTokenSource _currentTaskCts;

        #endregion

        #region CONSTRUCTORS
        public QuestAssistantEditor(QuestAssistant target) : base(target)
        {
            SetInfo(target);
            CreateVisualElement();
        }
        #endregion

        #region METHODS
        public sealed override void SetInfo(object target)
        {
            this.target = target as QuestAssistant;
            _questAssistant = target as QuestAssistant;
            _questGraph = _questAssistant?.OwnerLayer.GetModule<QuestGraph>();
        }

        /// <summary>
        /// Creates and configures the visual elements for the editor.
        /// </summary>
        protected sealed override VisualElement CreateVisualElement()
        {
            Clear();
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>(UIElementNames.VisualTree);
            visualTree.CloneTree(this);

            _lockedContextEntryContainer = this.Q<VisualElement>(UIElementNames.LockedLayerContainer);
            _layerList = this.Q<ListView>(UIElementNames.LayerList);
            _suggestionList = this.Q<ListView>(UIElementNames.SuggestionList);
            _addLayerButton = this.Q<Button>(UIElementNames.AddLayerButton);
            _generateSuggestionsButton = this.Q<Button>(UIElementNames.GenerateSuggestionsButton);
            _removeSuggestionsButton = this.Q<Button>(UIElementNames.RemoveAllSuggestions);
            _autoConnectButton = this.Q<Button>(UIElementNames.ConnectAll);
            _noSuggestionPanel = this.Q<LBSPanelTextIcon>(UIElementNames.NoSuggestionPanel);
            _suggestionField = this.Q<LBSCustomUnsignedIntegerField>(UIElementNames.SuggestionField);
            _suggestionField.value = _questAssistant.SuggestionAmount;
            
            _suggestionField.RegisterValueChangedCallback(evt =>
            {
                _questAssistant.SuggestionAmount = evt.newValue; 
            });
            
            _addLayerButton.clicked += ShowAddLayerMenu;
            _generateSuggestionsButton.clicked += RunTask;

            _removeSuggestionsButton.clicked += () =>
            {
                _questGraph.Suggestions.Clear();
                UpdateSuggestionsDisplay();
            };
            _autoConnectButton.style.display = DisplayStyle.None;
            
            SetupLayerContextList();
            SetupSuggestionList();
            AddLockedLayer();
            return this;
        }

        void CancelCurrentTask()
        {
            if(_currentTaskCts == null) return;
            if(_currentTaskCts.IsCancellationRequested) return;
            _currentTaskCts.Cancel();
        }
        
        private void RunTask()
        {
            _currentTaskCts?.Cancel();

            _currentTaskCts = new CancellationTokenSource();
            var token = _currentTaskCts.Token;

            var taskbar =LBSMainWindow.Instance.rootVisualElement.Q<ToolBarMain>();;
            
            taskbar.OnProgressCancelled -= CancelCurrentTask;
            taskbar.OnProgressCancelled += CancelCurrentTask;
            
            void ReportProgress(float normalized)
            {
                // Use update so progress applies immediately
                EditorApplication.update += UpdateOnce;
                void UpdateOnce()
                {
                    taskbar.SetProgressPercent(normalized);
                    EditorApplication.update -= UpdateOnce;
                }
            }
            
            taskbar.EnableProcess(true, _questAssistant.Name);
            Task.Run(() =>
            {
                try
                {
                    var bundleToActions = _questAssistant.GenerateSuggestions((int)GetSuggestionCount(), progress =>
                    {
                        // progress from 0 to 0.5
                        ReportProgress(0.05f * progress);
                    }, token);

                    if (token.IsCancellationRequested)
                    {
                        ReportProgress(0);
                        return;
                    }
                    
                    var suggestions = _questAssistant.CreateNewSuggestions(bundleToActions, progress =>
                    {
                        // progress from 0 to 95
                        ReportProgress(0.05f + 0.95f * progress);
                    }, token);
                    
                    if (token.IsCancellationRequested)
                    {
                        ReportProgress(0);
                        return;
                    }
                    
                    
                    // Once done, update UI safely
                    EditorApplication.delayCall += () =>
                    {
                        _questGraph.Suggestions.AddRange(suggestions);
                        UpdateSuggestionsDisplay();
                        taskbar.EnableProcess(false);
                    };
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[QuestAssistant] Task failed: {ex}");
                    EditorApplication.delayCall += () => taskbar.EnableProcess(false);
                }

            }, token);
        }

        private uint GetSuggestionCount()
        {
            return _suggestionField.value;
        }

        #region LAYERS
        private void SetupLayerContextList()
        {
            _layerList.reorderable = false;
            _layerList.makeItem = () => new LayerContextEntry();
            _layerList.bindItem = BindLayerContextEntry;
            _layerList.itemsSource = _questAssistant.Data.ContextLayers;
            UpdateContextDisplay();
        }
        
        private void UpdateContextDisplay()
        {
            _layerList.Rebuild();
            _layerList.style.display = _questAssistant.Data.ContextLayers.Count > 0 ? DisplayStyle.Flex : DisplayStyle.None;
        }
        
        private void BindLayerContextEntry(VisualElement element, int index)
        {
            if (element is not LayerContextEntry layerContextEntry) return;

            layerContextEntry.UpdateData(_questAssistant.Data.ContextLayers[index]);
            layerContextEntry.EvaluateOverlap(_questAssistant.Data.ContextLayers);
            layerContextEntry.OnRemoveButtonClicked = null;
            layerContextEntry.OnRemoveButtonClicked += () =>
            {
                _questAssistant.Data.ContextLayers.RemoveAt(index);
                UpdateContextDisplay();
            };
        }
        
        private void AddLockedLayer()
        {
            var lockedLayer = new LayerContextEntry();
            lockedLayer.UpdateData(_questGraph.OwnerLayer);
            lockedLayer.SetEnabled(false);
            _lockedContextEntryContainer.Add(lockedLayer);
        }
        
        private void ShowAddLayerMenu()
        {
            var menu = new GenericMenu();
            foreach (var layer in _questAssistant.Data.Layers)
            {
                if (!_questGraph.OwnerLayer.Equals(layer))
                {
                    menu.AddItem(new GUIContent(layer.Name), _questAssistant.Data.ContextLayers.Contains(layer), ToggleLayerContext, layer);
                }
            }
            menu.ShowAsContext();
        }
        
        private void ToggleLayerContext(object layer)
        {
            if (layer is not LBSLayer lbsLayer)
            {
                Debug.LogError("Invalid layer object.");
                return;
            }

            if (_questAssistant.Data.ContextLayers.Contains(lbsLayer))
                _questAssistant.Data.ContextLayers.Remove(lbsLayer);
            else
                _questAssistant.Data.ContextLayers.Add(lbsLayer);

            UpdateContextDisplay();
        }
        #endregion

        #region SUGGESTIONS
        private void SetupSuggestionList()
        {
            _suggestionList.reorderable = false;
            _suggestionList.makeItem = () => new QuestNodeSuggestion();
            _suggestionList.bindItem = BindQuestNodeSuggestion;
            _suggestionList.itemsSource = _questGraph.Suggestions;
            UpdateSuggestionsDisplay();
        }
        
        private void UpdateSuggestionsDisplay()
        {
            bool hasSuggestions = _suggestionList.itemsSource.Count > 0;
            _suggestionList.Rebuild();
            _noSuggestionPanel.style.display = hasSuggestions ? DisplayStyle.None : DisplayStyle.Flex;
            _suggestionList.style.display = hasSuggestions ? DisplayStyle.Flex : DisplayStyle.None;
            // redraw to display suggestions
            DrawManager.Instance.RedrawLayer(_questGraph.OwnerLayer);
            MarkDirtyRepaint();
        }
        
        private void BindQuestNodeSuggestion(VisualElement element, int index)
        {
            if (element is not QuestNodeSuggestion suggestionEntry) return;

            suggestionEntry.UpdateData(_questGraph.Suggestions[index]);
            _questGraph.OnRemoveSuggestion += (suggestionToRemove) =>
            {
                _questGraph.Suggestions.Remove(suggestionToRemove);
            };
            _questGraph.OnRemoveSuggestion -= HandleRemoveSuggestion;
            _questGraph.OnRemoveSuggestion += HandleRemoveSuggestion;
        }
        
        private void HandleRemoveSuggestion(QuestNode node)
        {
            UpdateSuggestionsDisplay();
        }
        #endregion
        
        public override void OnFocus()
        {
            _questGraph.displaySuggestions = true;
            DrawManager.Instance.RedrawLayer(_questGraph.OwnerLayer);
        }
        
        public override void OnUnfocus()
        {
            LBSMainWindow.Instance.rootVisualElement.Q<ToolBarMain>().CancelProgress();
            _questGraph.displaySuggestions = false;
            DrawManager.Instance.RedrawLayer(_questGraph.OwnerLayer);
        }
        
        public void SetTools(ToolKit toolkit) { }
        #endregion
    }
}