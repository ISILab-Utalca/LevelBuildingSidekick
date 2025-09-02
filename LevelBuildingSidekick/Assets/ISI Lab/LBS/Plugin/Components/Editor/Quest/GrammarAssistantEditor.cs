using System;
using System.Collections.Generic;
using System.Linq;
using ISILab.Commons.Utility.Editor;
using ISILab.Extensions;
using ISILab.LBS.Assistants;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Components;
using ISILab.LBS.CustomComponents;
using ISILab.LBS.Manipulators;
using ISILab.LBS.Modules;
using ISILab.LBS.VisualElements;
using ISILab.LBS.VisualElements.Editor;
using LBS.VisualElements;
using ISILab.Macros;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

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
        
        
        public GrammarAssistantEditor()
        {

        }

        public GrammarAssistantEditor(GrammarAssistant target) : base(target)
        {
            CreateVisualElement();
            SetInfo(target);
    
        }

        public sealed override void SetInfo(object paramTarget)
        {
            target = paramTarget as GrammarAssistant;
            _grammarAssistant = LBSLayerHelper.GetObjectFromLayerChild<GrammarAssistant>(paramTarget);
            _questBehaviour = LBSLayerHelper.GetObjectFromLayerChild<QuestBehaviour>(paramTarget);
            if (_questBehaviour != null)
            {
                _questGraph = _questBehaviour.Graph;
                _questGraph.OnQuestNodeSelected += (_) =>
                {
                    UpdatePanel();
                };

                _questGraph.RedrawGraph += () =>
                {
                    // update the graph grammar
                    foreach (var edge in _questGraph.GraphEdges)
                    {
                        _grammarAssistant.ValidateEdgeGrammar(edge);
                    }
                    
                };
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

     private void UpdatePanel()
    {
        if (_questGraph is null) return;
        
        _grammarField.value = _questGraph.Grammar;
        string selectedAction = GetActionToSet();
        _paramActionLabel.text = "none";
        _nodeIDLabel.text = "none";

        if (string.IsNullOrEmpty(selectedAction)) return;

        if (_questGraph.GetNodeAsQuest() is not null)
        {   
            _paramActionLabel.text = _questGraph.GetNodeAsQuest().QuestAction;
            _nodeIDLabel.text = _questGraph.GetNodeAsQuest().ID;
            SetBaseDataValues(_questGraph.GetNodeData());
        }
    

        // Get suggestions from the assistant
        List<string> nextActions = _grammarAssistant.GetAllValidNextActions(selectedAction);
        List<string> prevActions = _grammarAssistant.GetAllValidPrevActions(selectedAction);
        List<List<string>> expandActions = _grammarAssistant.GetAllExpansions(selectedAction);

        // NEXT SUGGESTED
        _nextInvalidPanel.style.display = nextActions.Any() ? DisplayStyle.None : DisplayStyle.Flex;
        _nextSuggested.style.display = !nextActions.Any() ? DisplayStyle.None : DisplayStyle.Flex;
        
        _nextSuggested.itemsSource = nextActions;
        _nextSuggested.makeItem = () => new SuggestionActionButton();
        _nextSuggested.bindItem = (e, i) =>
        {
            if(_questGraph.SelectedQuestNode is null) return;
            
            var button = e as SuggestionActionButton;
            if (nextActions.Count <= i) return;
            string action = nextActions[i];
            button?.SetAction(action, InsertNextAction(action, _questGraph.GetNodeAsQuest()));

        };
        _nextSuggested.Rebuild();

        // PREVIOUS SUGGESTED
        _prevInvalidPanel.style.display = prevActions.Any() ? DisplayStyle.None : DisplayStyle.Flex;
        _prevSuggested.style.display = !prevActions.Any() ? DisplayStyle.None : DisplayStyle.Flex;
            
        _prevSuggested.itemsSource = prevActions;
        _prevSuggested.makeItem = () => new SuggestionActionButton();
        _prevSuggested.bindItem = (e, i) =>
        {
            if(_questGraph.SelectedQuestNode is null) return;
            
            var button = e as SuggestionActionButton;
            if (prevActions.Count <= i) return;
            string action = prevActions[i];
            button?.SetAction(action, InsertPreviousAction(action, _questGraph.GetNodeAsQuest()));
        };
        _prevSuggested.Rebuild();

        // EXPAND SUGGESTED (Groups of steps)
        _expandInvalidPanel.style.display = expandActions.Any() ? DisplayStyle.None : DisplayStyle.Flex;
        _expandSuggested.style.display = !expandActions.Any() ? DisplayStyle.None : DisplayStyle.Flex;
        
        _expandSuggested.itemsSource = expandActions;
        _expandSuggested.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight; // Ensure dynamic height
        _expandSuggested.makeItem = () => 
        {
            var foldout = new LBSCustomFoldout();
            foldout.contentContainer.style.flexDirection = FlexDirection.Column;
            foldout.style.flexGrow = 1;
            foldout.style.flexShrink = 0;
            
            foldout.RegisterValueChangedCallback(evt =>
            {
                foldout.contentContainer.style.flexShrink = evt.newValue ? 0 : 1;
                _expandSuggested.RefreshItems();
     
            });
            
            return foldout;
        };
        _expandSuggested.bindItem = (v, i) =>
        {
            if(_questGraph.SelectedQuestNode is null) return;
            
            if(i > expandActions.Count-1) return;
            if (v is not LBSCustomFoldout foldout) return;
            
            foldout.contentContainer.Clear();

            foldout.text = "Expansion " + (i + 1);

            List<string> actions = expandActions[i];

            ExpansionHeader header = new ExpansionHeader();
            header.ButtonConvert.SetAction(_questGraph.GetNodeAsQuest().QuestAction,
                ExpandAction(actions, _questGraph.GetNodeAsQuest()));
            foldout.contentContainer.Add(header);

            for (int j = 0; j < actions.Count; j++)
            {
                QuestNode.ENodeType type = QuestNode.ENodeType.Middle;
                if (j == 0) type = QuestNode.ENodeType.Start;
                else if (j == actions.Count - 1) type = QuestNode.ENodeType.Goal;

                ActionExpandEntry entry = new ActionExpandEntry
                {
                    style =
                    {
                        flexGrow = 1,
                        flexShrink = 0
                    }
                };
                entry.SetEntryAction(actions[j], type);
                foldout.contentContainer.Add(entry);
            }
        };
        
        _expandSuggested.Rebuild();
    }

    private Action ExpandAction(List<string> expandAction, QuestNode referenceNode)
    {
        return () =>
        {
            _questGraph.ExpandNode(expandAction, referenceNode);
        };
    }


    /// <summary>
        /// Calls as a delegate the CreateAddNode function from the graph to make a new node based on the
        /// parameter action.
        /// </summary>
        /// <param name="action">Action(Grammar Terminal)</param>
        /// <param name="referenceNode">the node from where we got the next action</param>
        /// <returns></returns>
        private Action InsertNextAction(string action, QuestNode referenceNode)
        {
            return () =>
            {
                _questGraph.InsertQuestNodeAfter(action, referenceNode);
            };
        }
        
        /// <summary>
        /// Calls as a delegate that replaces the previous node to the selected node
        /// </summary>
        /// <param name="action">Action(Grammar Terminal) that will be added as previous</param>
        /// <param name="referenceNode">the node from where we got the previous action</param>
        /// <returns></returns>
        private Action InsertPreviousAction(string action, QuestNode referenceNode)
        {
            return () =>
            {
                _questGraph.InsertNodeBefore(action, referenceNode);
                _questGraph.ValidateAllWithGrammar();
            };
        }
        
        public void SetTools(ToolKit toolkit)
        {
            
        }
        
        private string GetActionToSet()
        {
            return _questGraph.GetNodeAsQuest()?.QuestAction;
        }
        
        private void SetBaseDataValues(BaseQuestNodeData data)
        {
            if(data == null) return;
            
            var backgroundColor = data.Color;
            backgroundColor.a = BackgroundOpacity;
            _actionColor.SetBackgroundColor(backgroundColor);
            
            _actionIcon.style.unityBackgroundImageTintColor = data.Color;
            _actionColor.SetBorder(data.Color, ActionBorderThickness);

        }
        
    }
}