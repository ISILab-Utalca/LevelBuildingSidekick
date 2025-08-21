using ISILab.AI.Grammar;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Components;
using ISILab.LBS.Editor;
using ISILab.LBS.Manipulators;
using ISILab.LBS.Modules;
using LBS;
using LBS.VisualElements;
using System;
using System.Collections.Generic;
using System.Linq;
using ISILab.LBS.Editor.Windows;
using ISILab.LBS.VisualElements.Editor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    [LBSCustomEditor("QuestBehaviour", typeof(QuestBehaviour))]
    public class QuestBehaviourEditor : LBSCustomEditor, IToolProvider
    {
        private AddGraphNode _addNode;
        private RemoveQuestNode _removeNode;
        private ConnectQuestNodes _connectNodes;
        private RemoveQuestConnection _removeConnection;
        private ObjectField _grammarReference;

        private VisualElement _actionPallete;
        private VisualElement _conditionalPallete;
        
        private QuestBehaviour _behaviour;
        
        public QuestBehaviourEditor(object target) : base(target)
        {
            CreateVisualElement();
            SetInfo(target);     
        }

        public sealed override void SetInfo(object paramTarget)
        {
            if (_behaviour != null) return;
            _behaviour = target as QuestBehaviour;

            if (_behaviour == null) return;
            
            // this should only happen on object creation
            var questGraph = _behaviour.OwnerLayer.GetModule<QuestGraph>();
            if (questGraph is null) return;
            
            questGraph.LoadGrammar();
            
            // Manually set both
            _grammarReference.value = questGraph.Grammar;
            ChangeGrammar(questGraph.Grammar);

            questGraph.RedrawGraph += () => DrawManager.Instance.RedrawLayer(questGraph.OwnerLayer);
        }

        public void SetTools(ToolKit toolkit)
        {
            _behaviour = target as QuestBehaviour;
            
            _addNode = new AddGraphNode();
            var t1 = new LBSTool(_addNode);
            t1.OnSelect += LBSInspectorPanel.ActivateBehaviourTab;
            
            _removeNode = new RemoveQuestNode();
            var t2 = new LBSTool(_removeNode);
            t2.OnSelect += LBSInspectorPanel.ActivateBehaviourTab;
            
            _connectNodes = new ConnectQuestNodes();
            var t3 = new LBSTool(_connectNodes);
            t3.OnSelect += LBSInspectorPanel.ActivateBehaviourTab;
            
            _removeConnection = new RemoveQuestConnection();
            var t4 = new LBSTool(_removeConnection);
            t4.OnSelect += LBSInspectorPanel.ActivateBehaviourTab;
            
            //_connectNodes.SetRemover(_removeConnection);
            
            toolkit.ActivateTool(t1,_behaviour?.OwnerLayer, target);
            toolkit.ActivateTool(t2,_behaviour?.OwnerLayer, target);
            toolkit.ActivateTool(t3,_behaviour?.OwnerLayer, target);
            toolkit.ActivateTool(t4,_behaviour?.OwnerLayer, target);
            
            _addNode.OnManipulationEnd += RefreshHistoryPanel;
            _removeNode.OnManipulationEnd += RefreshHistoryPanel;
            _connectNodes.OnManipulationEnd += RefreshHistoryPanel;
            _removeConnection.OnManipulationEnd += RefreshHistoryPanel;

            _behaviour ??= _behaviour; // if null, assign
            _behaviour!.Graph.GoToNode = null;
            _behaviour!.Graph.GoToNode += GoToQuestNode;
        }

        private static void GoToQuestNode(GraphNode node)
        {
            var nodePos = node.Position;
            var scale = MainView.Instance.viewTransform.scale;
            var viewport = MainView.Instance.viewport.layout;
            
            var xOffset = (viewport.width * 0.5f) / scale.x;
            var yOffset = (viewport.height * 0.5f) / scale.y;
            
            var x = nodePos.x - xOffset;
            var y = nodePos.y - yOffset;

            var position = new Vector3(-x * scale.x, -y * scale.y, 0);
            
            MainView.Instance.UpdateViewTransform(position, scale);
        }

        private void RefreshHistoryPanel()
        {
            SetInfo(target);
            
        }
        
        protected sealed override VisualElement CreateVisualElement()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestBehaviourEditor");
            visualTree.CloneTree(this);

            _grammarReference = this.Q<ObjectField>(name: "Grammar");
            _grammarReference.objectType = typeof(LBSGrammar);
            
            _conditionalPallete  = this.Q<VisualElement>(name: "Conditional");
            
            _actionPallete = this.Q<VisualElement>(name: "Content");
            _grammarReference.RegisterValueChangedCallback(evt => ChangeGrammar(evt.newValue as LBSGrammar));
            return this;
        }

        private void UpdateContent()
        {
            _actionPallete.Clear();

            var quest = _behaviour?.OwnerLayer.GetModule<QuestGraph>();
            if (quest == null) return;
            if (quest.Grammar == null || !quest.Grammar.TerminalActions.Any()) return;
            
            // Grammar actions
            List<ActionButton> actionButtons = new();
            foreach (string action in quest.Grammar.TerminalActions)
            {
                ActionButton actionButton = new ActionButton(action, () =>
                {
                    ToolKit.Instance.SetActive(typeof(AddGraphNode));
                    _behaviour.activeGraphNodeType = typeof(QuestNode);
                    _behaviour.ActionToSet = action;
                });
                
                actionButtons.Add(actionButton);
                _actionPallete.Add(actionButton);
            }
            
            // conditional nodes
            ActionButton orButton = new ActionButton("Or", () =>
            {
                ToolKit.Instance.SetActive(typeof(AddGraphNode));
                _behaviour.activeGraphNodeType = typeof(OrNode);
                _behaviour.ActionToSet = string.Empty;
            });
            _conditionalPallete.Add(orButton);
            
            ActionButton andButton = new ActionButton("And", () =>
            {
                ToolKit.Instance.SetActive(typeof(AddGraphNode));
                _behaviour.activeGraphNodeType = typeof(AndNode);
                _behaviour.ActionToSet = string.Empty;
            });
            _conditionalPallete.Add(andButton);

        }

        private void ChangeGrammar(LBSGrammar grammar)
        {
            if (grammar == null)
            {
                LBSMainWindow.MessageNotify("LBS Quest: Must assign a valid grammar in the Quest Behaviour Editor",LogType.Error,5);
                _grammarReference.value = null;
            }
            else
            {
                var quest = _behaviour.OwnerLayer.GetModule<QuestGraph>();
                if (quest == null)  throw new Exception("No Module");
            
                // Check if the new grammar is different at all
                if(quest.Grammar != grammar) quest.Grammar = grammar;
            }
           
            UpdateContent();
            
        }
    }
}