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
using ISILab.LBS.Assistants;
using ISILab.LBS.VisualElements.Editor;
using ISILab.Macros;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    [LBSCustomEditor("QuestBehaviour", typeof(QuestBehaviour))]
    public class QuestBehaviourEditor : LBSCustomEditor, IToolProvider
    {
        private AddQuestNode _addNode;
        private RemoveQuestNode _removeNode;
        private ConnectQuestNodes _connectNodes;
        private RemoveQuestConnection _removeConnection;

        private QuestHistoryPanel _questHistoryPanel;

        private ObjectField _grammarReference;

        private VisualElement _actionPallete;

        private QuestBehaviour _behaviour;
        
        public QuestBehaviourEditor(object target) : base(target)
        {
            
            CreateVisualElement();
            SetInfo(target);     
        }

        public sealed override void SetInfo(object paramTarget)
        {
            //Clear();
            _behaviour = target as QuestBehaviour;
            if (_behaviour == null)return;
         

            var quest = _behaviour.OwnerLayer.GetModule<QuestGraph>();
            
            _grammarReference.value = quest.Grammar; // Loads grammar
            
            ChangeGrammar(quest.Grammar);
            UpdateContent();
        }

        public void SetTools(ToolKit toolkit)
        {
            _behaviour = target as QuestBehaviour;
            
            _addNode = new AddQuestNode();
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

            
            _connectNodes.SetRemover(_removeConnection);
            
            toolkit.ActivateTool(t1,_behaviour?.OwnerLayer, target);
            toolkit.ActivateTool(t2,_behaviour?.OwnerLayer, target);
            toolkit.ActivateTool(t3,_behaviour?.OwnerLayer, target);
            toolkit.ActivateTool(t4,_behaviour?.OwnerLayer, target);
            
            _addNode.OnManipulationEnd += RefreshHistoryPanel;
            _removeConnection.OnManipulationEnd += RefreshHistoryPanel;
            _connectNodes.OnManipulationEnd += RefreshHistoryPanel;
            _removeConnection.OnManipulationEnd += RefreshHistoryPanel;

            _behaviour ??= _behaviour; // if null, assign
            _behaviour!.Graph.GoToNode = null;
            _behaviour!.Graph.GoToNode += GoToQuestNode;
        }

        private static void GoToQuestNode(QuestNode node)
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
            _behaviour.Graph.UpdateFlow?.Invoke();
         //   var questBehaviour = target as QuestBehaviour;
          //  DrawManager.Instance.RedrawLayer(questBehaviour?.OwnerLayer, MainView.Instance);
        }
        
        protected sealed override VisualElement CreateVisualElement()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("GrammarAssistantEditor");
            visualTree.CloneTree(this);

            _grammarReference = this.Q<ObjectField>(name: "Grammar");
            _grammarReference.objectType = typeof(LBSGrammar);
            
            _actionPallete = this.Q<VisualElement>(name: "Content");
            
            _grammarReference.RegisterValueChangedCallback(evt => ChangeGrammar(evt.newValue as LBSGrammar));
            return this;
        }

        private void UpdateContent()
        {
            _actionPallete.Clear();

           // var behaviour = target as QuestBehaviour;
            if (_behaviour == null)
                return;

            var quest = _behaviour.OwnerLayer.GetModule<QuestGraph>();
            if (quest == null || quest.OwnerLayer == null)
                return;

            if (quest.Grammar == null)
                return;

            List<ActionButton> abL = new();
            foreach (var a in quest.Grammar.Actions)
            {
                ActionButton b;
                b = new ActionButton(a.GrammarElement.Text, () =>
                {
                    ToolKit.Instance.SetActive(typeof(AddQuestNode));
                    _behaviour.ToSet = a.GrammarElement;
                    _behaviour.Graph.UpdateFlow?.Invoke();
                    UpdateContent();
                });
                abL.Add(b);
                _actionPallete.Add(b);
            }

            foreach (var b in abL)
            {
                if (_behaviour.ToSet?.Text == b.text.text)
                {
                    b.Children().First().AddToClassList("lbs-actionbutton_selected");
                    b.Children().First().RemoveFromClassList("lbs-actionbutton");
                }
                else
                {
                    b.Children().First().RemoveFromClassList("lbs-actionbutton_selected");
                    b.Children().First().AddToClassList("lbs-actionbutton");
                }
            }
            
           // DrawManager.Instance.RedrawLayer(quest.OwnerLayer, MainView.Instance);
           
        }

        private void ChangeGrammar(LBSGrammar grammar)
        {
            if (grammar == null) throw new Exception("No Grammar");
            
            if (target is not QuestBehaviour behaviour) throw new Exception("No Behavior");

            var assistant = behaviour.OwnerLayer.GetAssistant<GrammarAssistant>();
            if (assistant == null) throw new Exception("No Behavior");
            
            var quest = behaviour.OwnerLayer.GetModule<QuestGraph>();
            if (quest == null)  throw new Exception("No Module");
            
            quest.Grammar = grammar;
            // check if the new grammar applies on the graph
            if(quest.QuestEdges.Any()) assistant.ValidateEdgeGrammar(quest.QuestEdges.First());
          
            UpdateContent();
        }
    }
}