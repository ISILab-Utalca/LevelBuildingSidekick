using System;
using ISILab.LBS.Editor.Windows;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Components;
using ISILab.LBS.Modules;
using LBS.Components;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Manipulators
{
    public class AddQuestNode : LBSManipulator
    {
        private QuestGraph _questGraph;
        private QuestBehaviour _behaviour;
        private string ActionToSet => _behaviour.ActionToSet;
        protected override string IconGuid => "3d0b251f4a09bce4b9224787cfa08d49";

        public AddQuestNode()
        {
            Name = "Add Quest Node";
            Description = "Pick a quest word from the inspector panel, then Click on the graph.";
        }

        public override void Init(LBSLayer layer, object provider = null)
        {
            base.Init(layer, provider);
            
            _questGraph = layer.GetModule<QuestGraph>();
            _behaviour = layer.GetBehaviour<QuestBehaviour>();
        }

        protected override void OnMouseUp(VisualElement element, Vector2Int endPosition, MouseUpEvent e)
        {
            if (string.IsNullOrEmpty(ActionToSet))
            {
                LBSMainWindow.MessageNotify("Can't add node. Make sure to select a grammar and a word.", LogType.Error, 5);
                return;
            }

            _questGraph.CreateAddNode(ActionToSet, endPosition);
            OnManipulationEnd.Invoke();
        }
    }
}