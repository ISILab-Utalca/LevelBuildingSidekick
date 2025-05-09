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
        private CreateQuestNode addNode;
        private RemoveQuestNode removeNode;
        private ConnectQuestNodes connectNodes;
        private RemoveQuestConnection removeConnection;

        private QuestHistoryPanel questHistoryPanel;
        
        ObjectField grammarReference;

        VisualElement actionPallete;

        QuestBehaviour behaviour;

        private string defaultGrammarGuid = "63ab688b53411154db5edd0ec7171c42";
        
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
            
            grammarReference.value = quest.Grammar;
            
            ChangeGrammar(quest.Grammar);
            UpdateContent();
        }

        public void SetTools(ToolKit toolkit)
        {
            var questBehaviour = target as QuestBehaviour;

            Texture2D icon;

            icon = Resources.Load<Texture2D>("Icons/Quest_Icon/Add_Node_Quest");
            addNode = new CreateQuestNode();
            var t1 = new LBSTool(icon, "Add Quest Node", "Add Quest Node", addNode);
            t1.OnSelect += LBSInspectorPanel.ActivateBehaviourTab;
            t1.Init(questBehaviour?.OwnerLayer, target);

            icon = Resources.Load<Texture2D>("Icons/Quest_Icon/Delete_Node_Quest");
            removeNode = new RemoveQuestNode();
            var t2 = new LBSTool(icon, "Remove Quest Node", "Remove Quest Node", removeNode);
            t2.OnSelect += LBSInspectorPanel.ActivateBehaviourTab;
            t2.Init(questBehaviour?.OwnerLayer, target);
            
            icon = Resources.Load<Texture2D>("Icons/Quest_Icon/Node_Connection_Quest");
            connectNodes = new ConnectQuestNodes();
            var t3 = new LBSTool(icon, "Connect Quest Node", "Add Node Connection", connectNodes);
            t3.OnSelect += LBSInspectorPanel.ActivateBehaviourTab;
            t3.Init(questBehaviour?.OwnerLayer, target);

            icon = Resources.Load<Texture2D>("Icons/Quest_Icon/Delete_Node_Connection_Quest");
            removeConnection = new RemoveQuestConnection();
            var t4 = new LBSTool(icon, "Remove Quest Connection", "Remove Quest Connection", removeConnection);
            t4.OnSelect += LBSInspectorPanel.ActivateBehaviourTab;
            t4.Init(questBehaviour?.OwnerLayer, target);
            
            connectNodes.SetRemover(removeConnection);
            
            toolkit.AddTool(t1);
            toolkit.AddTool(t2);
            toolkit.AddTool(t3);
            toolkit.AddTool(t4);
            
            addNode.OnManipulationEnd += RefreshHistoryPanel;
            removeConnection.OnManipulationEnd += RefreshHistoryPanel;
            connectNodes.OnManipulationEnd += RefreshHistoryPanel;
            removeConnection.OnManipulationEnd += RefreshHistoryPanel;

            behaviour ??= questBehaviour; // if null, assign
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
            if (quest == null)
                return;

            if (quest.Grammar == null)
                return;

            List<ActionButton> abL = new();
            foreach (var a in quest.Grammar.Actions)
            {
                ActionButton b;
                b = new ActionButton(a.GrammarElement.Text, () =>
                {
                    ToolKit.Instance.SetActive("Add Quest Node");
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