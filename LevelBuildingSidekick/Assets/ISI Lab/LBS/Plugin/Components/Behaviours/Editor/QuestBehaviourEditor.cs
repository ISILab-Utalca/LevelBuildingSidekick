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
        private AddQuestNode addNode;
        private RemoveQuestNode removeNode;
        private ConnectQuestNodes connectNodes;
        private RemoveQuestConnection removeConnection;

        private QuestHistoryPanel questHistoryPanel;
        
        ObjectField grammarReference;

        VisualElement actionPallete;

        QuestBehaviour behaviour;
        
        public QuestBehaviourEditor(object target) : base(target)
        { 
            SetInfo(target);
        }

        public sealed override void SetInfo(object target)
        {
            Clear();
            
            CreateVisualElement();
            
            behaviour = target as QuestBehaviour;
            if (behaviour == null)
                return;

            var quest = behaviour.OwnerLayer.GetModule<QuestGraph>();
            
            grammarReference.value = quest.Grammar; // Loads grammar
            
            ChangeGrammar(quest.Grammar);
            UpdateContent();
        }

        public void SetTools(ToolKit toolkit)
        {
            behaviour = target as QuestBehaviour;
            
            addNode = new AddQuestNode();
            var t1 = new LBSTool(addNode);
            t1.OnSelect += LBSInspectorPanel.ActivateBehaviourTab;
            
            removeNode = new RemoveQuestNode();
            var t2 = new LBSTool(removeNode);
            t2.OnSelect += LBSInspectorPanel.ActivateBehaviourTab;
            
            connectNodes = new ConnectQuestNodes();
            var t3 = new LBSTool(connectNodes);
            t3.OnSelect += LBSInspectorPanel.ActivateBehaviourTab;
            
            removeConnection = new RemoveQuestConnection();
            var t4 = new LBSTool(removeConnection);
            t4.OnSelect += LBSInspectorPanel.ActivateBehaviourTab;

            
            connectNodes.SetRemover(removeConnection);
            
            toolkit.ActivateTool(t1,behaviour?.OwnerLayer, target);
            toolkit.ActivateTool(t2,behaviour?.OwnerLayer, target);
            toolkit.ActivateTool(t3,behaviour?.OwnerLayer, target);
            toolkit.ActivateTool(t4,behaviour?.OwnerLayer, target);
            
            addNode.OnManipulationEnd += RefreshHistoryPanel;
            removeConnection.OnManipulationEnd += RefreshHistoryPanel;
            connectNodes.OnManipulationEnd += RefreshHistoryPanel;
            removeConnection.OnManipulationEnd += RefreshHistoryPanel;

            behaviour ??= behaviour; // if null, assign
            behaviour!.Graph.GoToNode += GoToQuestNode;
        }

        private void GoToQuestNode(QuestNode Node)
        {
            var nodePos = Node.Position;
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
            behaviour.Graph.UpdateFlow?.Invoke();
            var questBehaviour = target as QuestBehaviour;
            DrawManager.Instance.RedrawLayer(questBehaviour?.OwnerLayer, MainView.Instance);
        }
        
        protected override VisualElement CreateVisualElement()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("GrammarAssistantEditor");
            visualTree.CloneTree(this);

            grammarReference = this.Q<ObjectField>(name: "Grammar");
            grammarReference.objectType = typeof(LBSGrammar);
            
            actionPallete = this.Q<VisualElement>(name: "Content");
            
            grammarReference.RegisterValueChangedCallback(evt => ChangeGrammar(evt.newValue as LBSGrammar));
            return this;
        }

        private void UpdateContent()
        {
            actionPallete.Clear();

           // var behaviour = target as QuestBehaviour;
            if (behaviour == null)
                return;

            var quest = behaviour.OwnerLayer.GetModule<QuestGraph>();
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
                    behaviour.ToSet = a.GrammarElement;
                    behaviour.Graph.UpdateFlow?.Invoke();
                    UpdateContent();
                });
                abL.Add(b);
                actionPallete.Add(b);
            }

            foreach (var b in abL)
            {
                if (behaviour.ToSet?.Text == b.text.text)
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
            
            DrawManager.Instance.RedrawLayer(quest.OwnerLayer, MainView.Instance);
           
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