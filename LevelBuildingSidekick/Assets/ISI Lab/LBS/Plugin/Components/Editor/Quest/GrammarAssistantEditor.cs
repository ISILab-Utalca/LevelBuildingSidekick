using System.Collections.Generic;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Assistants;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Manipulators;
using ISILab.LBS.Modules;
using LBS.VisualElements;
using ISILab.Macros;
using UnityEditor.UIElements;
using UnityEngine;
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
        #endregion
        
        
        #region VIEW
        private ObjectField _grammarField;
        
        private VisualElement _nextInvalidPanel;
        private VisualElement _prevInvalidPanel;
        private VisualElement _expandInvalidPanel;

        private ListView _nextSuggested;
        private ListView _prevSuggested;
        private ListView _expandSuggested;
        #endregion
        
        
        public GrammarAssistantEditor()
        {

        }

        public GrammarAssistantEditor(GrammarAssistant target) : base(target)
        {
            SetInfo(target);
            CreateVisualElement();
        }

        public sealed override void SetInfo(object paramTarget)
        {
            target = paramTarget as GrammarAssistant;
            _grammarAssistant = LBSLayerHelper.GetObjectFromLayerChild<GrammarAssistant>(paramTarget);
            _questBehaviour = LBSLayerHelper.GetObjectFromLayerChild<QuestBehaviour>(paramTarget);
            if (_questBehaviour != null)
            {
                _questGraph = _questBehaviour.Graph;
            }
            
            UpdatePanel();
        }

        private void UpdatePanel()
        {
            if (string.IsNullOrEmpty(GetSelectedNode())) return;

            _nextSuggested.Clear();
            _prevSuggested.Clear();
            _expandSuggested.Clear();

            // Get suggestions from the assistant
            List<string> nextActions = _grammarAssistant.GetAllValidNextActions(GetSelectedNode());
            List<string> prevActions = _grammarAssistant.GetAllValidPrevActions(GetSelectedNode());
            List<List<string>> expandActions = _grammarAssistant.GetAllExpansions(GetSelectedNode());

            // Create ActionButtons for next actions
            foreach (var action in nextActions)
            {
                var button = CreateActionButton(action);
                _nextSuggested.Add(button);
            }

            // Create ActionButtons for previous actions
            foreach (var action in prevActions)
            {
                var button = CreateActionButton(action);
                _prevSuggested.Add(button);
            }

            // Create ActionButtons for expansions (grouped paths)
            foreach (var expansionPath in expandActions)
            {
                var container = new VisualElement();
                container.style.flexDirection = FlexDirection.Row;

                foreach (var step in expansionPath)
                {
                    var button = CreateActionButton(step);
                    container.Add(button);
                }

                _expandSuggested.Add(container);
            }
        }

        private Button CreateActionButton(string action)
        {
            var button = new Button(() =>
            {
                Debug.Log($"Action selected: {action}");
                // Optionally, assign this action to a node or do something with it
            })
            {
                text = action
            };

            button.style.marginRight = 4;
            button.style.marginBottom = 4;
            return button;
        }
        
        public void SetTools(ToolKit toolkit)
        {
            
        }

        protected sealed override VisualElement CreateVisualElement()
        {
            Clear();
  
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("GrammarAssistantEditor");
            visualTree.CloneTree(this);
            
            _grammarField = this.Q<ObjectField>("Grammar");
            if(_questGraph is not null) _grammarField.value = _questGraph.Grammar;
            
            _nextInvalidPanel = this.Q<VisualElement>("NextInvalidPanel");
            _prevInvalidPanel = this.Q<VisualElement>("PrevInvalidPanel");
            _expandInvalidPanel = this.Q<VisualElement>("ExpandInvalidPanel");
            
            _nextSuggested = this.Q<ListView>("NextSuggested");
            _prevSuggested = this.Q<ListView>("PrevSuggested"); 
            _expandSuggested = this.Q<ListView>("ExpandSuggested");
            
            return this;
        }

        private string GetSelectedNode()
        {
            return _questBehaviour.ActionToSet;
        }

        private void PrintValidActions()
        {
            List<string> actions = _grammarAssistant.GetAllValidNextActions(GetSelectedNode());
            foreach (var action in actions)
            {
                Debug.Log(action);
            }
        }
        
    }
}