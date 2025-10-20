using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ISILab.Commons.Utility.Editor;
using ISILab.Extensions;
using ISILab.LBS.Assistants;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Components;
using ISILab.LBS.CustomComponents;
using ISILab.LBS.Editor.Windows;
using ISILab.LBS.Manipulators;
using ISILab.LBS.Modules;
using ISILab.LBS.VisualElements;
using ISILab.LBS.VisualElements.Editor;
using LBS.VisualElements;
using ISILab.Macros;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;

namespace ISILab.LBS.Editor
{
    [LBSCustomEditor("GrammarAssistant", typeof(GrammarAssistant))]
    public class GrammarAssistantEditor : LBSCustomEditor, IToolProvider
    {
        #region FIELDS
        private QuestGraph _questGraph;
        private GrammarAssistant _grammarAssistant;
        private QuestBehaviour _questBehaviour;

        private const float ActionBorderThickness = 1f;
        private const float BackgroundOpacity = 0.25f;
        
        private CancellationTokenSource _currentTaskCts;
        #endregion

        #region VIEW
        private ObjectField _grammarField;

        private VisualElement _nextInvalidPanel;
        private VisualElement _prevInvalidPanel;
        private VisualElement _expandInvalidPanel;

        private ListView _nextSuggested;
        private ListView _prevSuggested;
        private ListView _expandSuggested;

        private Label _nodeIDLabel;
        private Label _paramActionLabel;
        private VisualElement _actionColor;
        private VisualElement _actionIcon;
        #endregion

        #region CONSTRUCTORS
        public GrammarAssistantEditor() { }

        public GrammarAssistantEditor(GrammarAssistant target) : base(target)
        {
            CreateVisualElement();
            SetInfo(target);
        }
        #endregion

        #region METHODS
        public sealed override void SetInfo(object paramTarget)
        {
            target = paramTarget as GrammarAssistant;
            _grammarAssistant = LBSLayerHelper.GetObjectFromLayerChild<GrammarAssistant>(paramTarget);
            _questBehaviour = LBSLayerHelper.GetObjectFromLayerChild<QuestBehaviour>(paramTarget);

            if (_questBehaviour != null)
            {
                _questGraph = _questBehaviour.Graph;
                _questGraph.OnGraphNodeSelected += UpdatePanel;
            }

            UpdatePanel();
        }

        protected sealed override VisualElement CreateVisualElement()
        {
            Clear();

            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("GrammarAssistantEditor");
            visualTree.CloneTree(this);

            _grammarField = this.Q<ObjectField>("Grammar");

            _nextInvalidPanel = this.Q<VisualElement>("NextInvalidPanel");
            _prevInvalidPanel = this.Q<VisualElement>("PrevInvalidPanel");
            _expandInvalidPanel = this.Q<VisualElement>("ExpandInvalidPanel");

            _nextSuggested = this.Q<ListView>("NextSuggested");
            _prevSuggested = this.Q<ListView>("PrevSuggested");
            _expandSuggested = this.Q<ListView>("ExpandSuggested");

            _paramActionLabel = this.Q<Label>("ParamAction");
            _nodeIDLabel = this.Q<Label>("ParamID");
            _actionColor = this.Q<VisualElement>("ActionColor");
            _actionIcon = this.Q<VisualElement>("ActionIcon");

            return this;
        }

        private void UpdatePanel(GraphNode selectedGraphNode = null)
        {
            if (_questGraph is null) return;

            _grammarField.value = _questGraph.Grammar;
            string selectedAction = GetActionToSet();
            _paramActionLabel.text = "none";
            _nodeIDLabel.text = "none";

            if (string.IsNullOrEmpty(selectedAction) || _questGraph.GetNodeAsQuest() is null)
            {
                ResetPanels();
                return;
            }

            QuestNode currentQuest = _questGraph.GetNodeAsQuest();
            _paramActionLabel.text = currentQuest.QuestAction;
            _nodeIDLabel.text = currentQuest.ID;
            SetBaseDataValues(_questGraph.GetNodeData());
            
            RunTask(currentQuest, selectedAction);
        }

        void CancelCurrentTask()
        {
            if(_currentTaskCts == null) return;
            if(_currentTaskCts.IsCancellationRequested) return;
            _currentTaskCts.Cancel();
        }
        
        void RunTask(QuestNode currentQuest, string selectedAction)
        {
            _currentTaskCts?.Cancel();

            _currentTaskCts = new CancellationTokenSource();
            var token = _currentTaskCts.Token;

            var taskbar = LBSMainWindow.Instance.rootVisualElement.Q<ToolBarMain>();
            
            taskbar.OnProgressCancelled -= CancelCurrentTask;
            taskbar.OnProgressCancelled += CancelCurrentTask;
            
            ResetPanels();
            
            taskbar.EnableProcess(true, _grammarAssistant.Name);
            Task.Run(() =>
            {
                try
                {
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

                    string[] nextArray = _grammarAssistant
                        .GetAllValidNextActionsInsert(selectedAction, _questGraph, progress =>
                        {
                            // progress from 0 → 0.33
                            ReportProgress(0.33f * progress);
                        }, token)
                        ?.ToArray() ?? Array.Empty<string>();

                    if (token.IsCancellationRequested)
                    {
                        ReportProgress(0);
                        return;
                    }
                    
                    string[] prevArray = _grammarAssistant
                        .GetAllValidPrevActionsInsert(selectedAction, _questGraph, progress =>
                        {
                            // progress from 0.33 → 0.66
                            ReportProgress(0.33f + 0.33f * progress);
                        }, token)
                        ?.ToArray() ?? Array.Empty<string>();
                    
                    if (token.IsCancellationRequested)
                    {
                        ReportProgress(0);
                        return;
                    }
                    
                    List<string>[] expandArray = _grammarAssistant
                        .GetAllExpansions(selectedAction, progress =>
                        {
                            // progress from 0.67 → 1.0
                            ReportProgress(0.67f + 0.33f * progress);
                        }, token)
                        ?.Select(l => l?.ToList() ?? new List<string>())
                        .ToArray() ?? Array.Empty<List<string>>();
                    
                    if (token.IsCancellationRequested)
                    {
                        ReportProgress(0);
                        return;
                    }
                    
                    // Once done, update UI safely
                    EditorApplication.delayCall += () =>
                    {
                        UpdateNextSuggestions(nextArray, currentQuest);
                        UpdatePrevSuggestions(prevArray, currentQuest);
                        UpdateExpandSuggestions(expandArray, currentQuest);
                        taskbar.EnableProcess(false);
                    };
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[GrammarAssistant] Task failed: {ex}");
                    EditorApplication.delayCall += () => taskbar.EnableProcess(false);
                }

            }, token);



        }

        #region Helpers
        
        public void SetTools(ToolKit toolkit) { }

        private string GetActionToSet()
        {
            return _questGraph.GetNodeAsQuest()?.QuestAction;
        }

        private void SetBaseDataValues(BaseQuestNodeData data)
        {
            if (data == null) return;

            var backgroundColor = data.Color;
            backgroundColor.a = BackgroundOpacity;
            _actionColor.SetBackgroundColor(backgroundColor);

            _actionIcon.style.unityBackgroundImageTintColor = data.Color;
            _actionColor.SetBorder(data.Color, ActionBorderThickness);
        }
        
        private void ResetPanels()
        {
            TogglePanel(_nextInvalidPanel, _nextSuggested, false);
            TogglePanel(_prevInvalidPanel, _prevSuggested, false);
            TogglePanel(_expandInvalidPanel, _expandSuggested, false);
        }

        private void TogglePanel(VisualElement invalidPanel, ListView listView, bool hasData)
        {
            if (invalidPanel != null)
                invalidPanel.style.display = hasData ? DisplayStyle.None : DisplayStyle.Flex;
            if (listView != null)
                listView.style.display = hasData ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private Action<VisualElement, int> SafeBind(Action<VisualElement, int> binder)
        {
            return (element, index) =>
            {
                try { binder(element, index); }
                catch (Exception ex)
                {
                    Debug.LogError($"Bind failed at index {index}: {ex}");
                }
            };
        }

        private void UpdateSuggestionList(
            string[] data,
            VisualElement invalidPanel,
            ListView listView,
            Func<string, Action> actionFactory)
        {
            if (invalidPanel != null)
                invalidPanel.style.display = data.Any() ? DisplayStyle.None : DisplayStyle.Flex;

            if (listView == null) return;

            listView.style.display = data.Any() ? DisplayStyle.Flex : DisplayStyle.None;
            listView.itemsSource = data;
            listView.makeItem = () => new SuggestionActionButton();
            listView.bindItem = SafeBind((element, index) =>
            {
                if (element is not SuggestionActionButton button) return;
                if (index < 0 || index >= data.Length) return;

                string action = data[index];
                button.SetAction(action, actionFactory(action));
            });
            listView.Rebuild();
        }
        #endregion

        #region Suggestion Updates
        private void UpdateNextSuggestions(string[] nextArray, QuestNode currentQuest)
        {
            UpdateSuggestionList(nextArray, _nextInvalidPanel, _nextSuggested,
                action => _grammarAssistant.InsertNextAction(action, currentQuest));
        }

        private void UpdatePrevSuggestions(string[] prevArray, QuestNode currentQuest)
        {
            UpdateSuggestionList(prevArray, _prevInvalidPanel, _prevSuggested,
                action => _grammarAssistant.InsertPreviousAction(action, currentQuest));
        }

        private void UpdateExpandSuggestions(List<string>[] expandArray, QuestNode currentQuest)
        {
            if (_expandInvalidPanel != null)
                _expandInvalidPanel.style.display = expandArray.Any() ? DisplayStyle.None : DisplayStyle.Flex;

            if (_expandSuggested == null) return;

            _expandSuggested.style.display = expandArray.Any() ? DisplayStyle.Flex : DisplayStyle.None;
            _expandSuggested.itemsSource = expandArray;
            _expandSuggested.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
            _expandSuggested.makeItem = MakeExpandFoldout;

            _expandSuggested.bindItem = SafeBind((visualElement, index) =>
            {
                if (visualElement is not LBSCustomFoldout foldout) return;
                if (index < 0 || index >= expandArray.Length) return;

                foldout.contentContainer.Clear();
                foldout.text = $"Expansion {index + 1}";

                List<string> actions = expandArray[index] ?? new List<string>();

                // Header
                var header = new ExpansionHeader();
                header.ButtonConvert.SetAction(currentQuest.QuestAction, _grammarAssistant.ExpandAction(actions, currentQuest));
                foldout.contentContainer.Add(header);

                // Entries
                for (int j = 0; j < actions.Count; j++)
                {
                    var type =  j == 0 ? QuestNode.ENodeType.Start :
                                j == actions.Count - 1 ? QuestNode.ENodeType.Goal :
                                QuestNode.ENodeType.Middle;

                    var entry = new ActionExpandEntry
                    {
                        style = { flexGrow = 1, flexShrink = 0 }
                    };
                    entry.SetEntryAction(actions[j], type, actions.Count == 1);
                    foldout.contentContainer.Add(entry);
                }
            });

            _expandSuggested.Rebuild();
        }
        #endregion

        #region Actions
        private LBSCustomFoldout MakeExpandFoldout()
        {
            var foldout = new LBSCustomFoldout();
            foldout.contentContainer.style.flexDirection = FlexDirection.Column;
            foldout.style.flexGrow = 1;
            foldout.style.flexShrink = 0;

            foldout.RegisterValueChangedCallback(evt =>
            {
                foldout.contentContainer.style.flexShrink = evt.newValue ? 0 : 1;
                try { _expandSuggested.RefreshItems(); }
                catch
                {
                    Debug.LogError("Assistant failed to refresh the expand foldout");
                }
            });

            return foldout;
        }
        
        #endregion

        public override void OnUnfocus()
        {
            LBSMainWindow.Instance.rootVisualElement.Q<ToolBarMain>().CancelProgress();
        }

        #endregion
    }
}
