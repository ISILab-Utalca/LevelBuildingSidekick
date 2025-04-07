using ISILab.AI.Grammar;
using ISILab.AI.Optimization.Populations;
using ISILab.LBS.Editor.Windows;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Components;
using ISILab.LBS.Modules;
using LBS.Components;
using LBS.Components.Graph;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Manipulators
{
    public class CreateQuestNode : LBSManipulator
    {
        QuestGraph quest;
        QuestBehaviour behaviour;
        public GrammarTerminal ActionToSet => behaviour.ToSet;


        private string prefix = "";
        public CreateQuestNode() : base()
        {
        }

        public override void Init(LBSLayer layer, object owner)
        {
            base.Init(layer, owner);
            
            quest = layer.GetModule<QuestGraph>();
            behaviour = layer.GetBehaviour<QuestBehaviour>();
        }

        protected override void OnMouseUp(VisualElement target, Vector2Int endPosition, MouseUpEvent e)
        {
            if (ActionToSet == null)
            {
                LBSMainWindow.MessageNotify("Can't add node. Make sure to select a grammar and a word.", LogType.Error, 5);
                return;
            }

            var name = "";
            var loop = true;
            var v = 0;
            do
            {
                name = prefix + ActionToSet.ID + " (" + v + ")";
                loop = quest.QuestNodes.Any(n => n.ID.Equals(name));
                v++;
            } while (loop);

            quest.AddNode(new QuestNode(name, EndPosition, ActionToSet.ID, quest));
            OnManipulationEnd.Invoke();
        }
    }
}