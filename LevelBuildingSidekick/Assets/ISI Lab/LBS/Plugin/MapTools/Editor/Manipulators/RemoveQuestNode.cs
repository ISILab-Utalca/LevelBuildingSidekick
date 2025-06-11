using ISILab.LBS.Modules;
using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Manipulators
{
    public class RemoveQuestNode : LBSManipulator
    {
        QuestGraph quest;

        protected override string IconGuid { get => "ce08b36a396edbf4394f7a4e641f253d"; }
        
        public RemoveQuestNode() : base()
        {
            name = "Remove Quest Node";
            description = "Click on a quest node to remove it.";
        }
        
        public override void Init(LBSLayer layer, object provider)
        {
            base.Init(layer, provider);
            
            quest = layer.GetModule<QuestGraph>();
        }

        protected override void OnMouseUp(VisualElement paramTarget, Vector2Int endPosition, MouseUpEvent e)
        {
            var node = quest.GetQuestNode(endPosition);
            if (node == null) return;
            quest.RemoveQuestNode(node);
            OnManipulationEnd?.Invoke();
        }
    }
}
