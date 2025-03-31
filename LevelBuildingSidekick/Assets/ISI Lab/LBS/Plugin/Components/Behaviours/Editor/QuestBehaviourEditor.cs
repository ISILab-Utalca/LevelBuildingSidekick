using ISILab.AI.Grammar;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Components;
using ISILab.LBS.Editor;
using ISILab.LBS.Internal;
using ISILab.LBS.Manipulators;
using ISILab.LBS.Modules;
using LBS;
using LBS.VisualElements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;
using ISILab.Extensions;
using ISILab.LBS.Editor.Windows;
using ISILab.LBS.VisualElements.Editor;
using ISILab.Macros;
using UnityEditor;
using UnityEditor.Graphs;
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
        
        DropdownField grammarDropdown;

        VisualElement actionPallete;

        List<LBSGrammar> grammars = new();

        QuestBehaviour behaviour;

        private string defaultGrammarGuid = "63ab688b53411154db5edd0ec7171c42";
        
        public QuestBehaviourEditor(object target) : base(target)
        {
            SetInfo(target);
            var defaultGrammar = LBSAssetMacro.LoadAssetByGuid<LBSGrammar>(defaultGrammarGuid);
            ChangeGrammar(defaultGrammar.name);
            
        }

        public override void SetInfo(object target)
        {
            Clear();
            
            CreateVisualElement();
            
            behaviour = target as QuestBehaviour;
            if (behaviour == null)
                return;

            var quest = behaviour.Owner.GetModule<QuestGraph>();

            UpdateDropdown();

            if (quest.Grammar != null)
            {
                grammarDropdown.value = quest.Grammar.name;
            }

            UpdateContent();
        }

        public void SetTools(ToolKit toolkit)
        {
            var ass = target as QuestBehaviour;

            Texture2D icon;

            icon = Resources.Load<Texture2D>("Icons/Quest_Icon/Add_Node_Quest");
            addNode = new CreateQuestNode();
            var t1 = new LBSTool(icon, "Add Quest Node", "Add a quest node activated!", addNode);
            t1.OnSelect += () => LBSInspectorPanel.ShowInspector("Behaviours");
            t1.Init(ass.Owner, target);

            icon = Resources.Load<Texture2D>("Icons/Quest_Icon/Delete_Node_Quest");
            removeNode = new RemoveQuestNode();
            var t2 = new LBSTool(icon, "Remove Quest Node", "Remove a quest node activated!", removeNode);
            t2.Init(ass.Owner, target);
            
            icon = Resources.Load<Texture2D>("Icons/Quest_Icon/Node_Connection_Quest");
            connectNodes = new ConnectQuestNodes();
            var t3 = new LBSTool(icon, "Connect Quest Node", "Connect quest nodes activated!", connectNodes);
            t3.Init(ass.Owner, target);

            icon = Resources.Load<Texture2D>("Icons/Quest_Icon/Delete_Node_Connection_Quest");
            removeConnection = new RemoveQuestConnection();
            var t4 = new LBSTool(icon, "Remove Quest Connection", "Remove quest connection activated!", removeConnection);
            t4.Init(ass.Owner, target);

            //addNode.SetAddRemoveConnection(removeNode); - right click assigns main root - no remover
            connectNodes.SetRemover(removeConnection);
            
            toolkit.AddTool(t1);
            toolkit.AddTool(t2);
            toolkit.AddTool(t3);
            toolkit.AddTool(t4);

            addNode.OnManipulationEnd = null;
            removeNode.OnManipulationEnd = null;
            connectNodes.OnManipulationEnd = null;
            removeConnection.OnManipulationEnd = null;
            
            addNode.OnManipulationEnd += RefreshHistoryPanel;
            removeConnection.OnManipulationEnd += RefreshHistoryPanel;
            connectNodes.OnManipulationEnd += RefreshHistoryPanel;
            removeConnection.OnManipulationEnd += RefreshHistoryPanel;

            behaviour.Graph.GoToNode += GoToQuestNode;
        }

        private void GoToQuestNode(QuestNode Node)
        {
            var nodePos = Node.Position;
            var scale = MainView.Instance.viewTransform.scale;
            var viewport = MainView.Instance.viewport.layout;
            
            var xOffset = (viewport.width * 0.5f) / scale.x;
            var yOffset = (viewport.height * 0.5f) / scale.y;
            
            //(viewport.width * 0.5f - viewport.width / behaviour.Graph.NodeSize.x) / scale.x;
            //var xOffset = (viewport.width * 0.5f)/ scale.x - ((viewport.width / behaviour.Graph.NodeSize.x) *1.5f)/ scale.x;
            //var yOffset = (viewport.height * 0.5f)/ scale.y - ((viewport.height / behaviour.Graph.NodeSize.y) *1.5f)/ scale.y;

            var x = nodePos.x - xOffset;
            var y = nodePos.y - yOffset;

            var position = new Vector3(-x * scale.x, -y * scale.y, 0);
            
            MainView.Instance.UpdateViewTransform(position, scale);
        }

        private void RefreshHistoryPanel()
        {
            SetInfo(target);
            LBSInspectorPanel.ReDraw();
            behaviour.Graph.UpdateFlow?.Invoke();
        }
        
        protected override VisualElement CreateVisualElement()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("GrammarAssistantEditor");
            visualTree.CloneTree(this);

            grammarDropdown = this.Q<DropdownField>(name: "Grammar");
            actionPallete = this.Q<VisualElement>(name: "Content");
            
            grammarDropdown.RegisterValueChangedCallback(evt => ChangeGrammar(evt.newValue));
  
            return this;
        }

        private void UpdateDropdown()
        {
            grammarDropdown.choices.Clear();
            grammars.Clear();
            var options = LBSAssetsStorage.Instance.Get<LBSGrammar>();
            if (options == null || options.Count == 0)
                return;
            grammars = options;
            grammarDropdown.choices = options.Select(s => s.name).ToList();
        }

        private void UpdateContent()
        {
            actionPallete.Clear();

           // var behaviour = target as QuestBehaviour;
            if (behaviour == null)
                return;

            var quest = behaviour.Owner.GetModule<QuestGraph>();
            if (quest == null)
                return;

            if (quest.Grammar == null)
                return;

            foreach (var a in quest.Grammar.Actions)
            {
                var b = new ActionButton(a.GrammarElement.Text, () =>
                {
                    behaviour.ToSet = a.GrammarElement;
                    ToolKit.Instance.SetActive("Add Quest Node");
                    behaviour.Graph.UpdateFlow?.Invoke();
                });
                actionPallete.Add(b);
            }
            behaviour.Graph.UpdateFlow?.Invoke();
           
        }

        private void ChangeGrammar(string value)
        {
            var behaviour = target as QuestBehaviour;
            if (behaviour == null)
                throw new Exception("No Assistant");

            var quest = behaviour.Owner.GetModule<QuestGraph>();
            if (quest == null)
                throw new Exception("No Module");

            var grammar = grammars.Find(s => s.name == value);
            if (grammar == null)
                throw new Exception("No Grammar");

            quest.Grammar = grammar;

            UpdateContent();
        }
    }
}