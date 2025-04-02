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

        public RemoveQuestNode() : base()
        {

        }

        public override void Init(LBSLayer layer, object provider)
        {
            base.Init(layer, provider);
            
            quest = layer.GetModule<QuestGraph>();
        }

        protected override void OnMouseUp(VisualElement target, Vector2Int endPosition, MouseUpEvent e)
        {
            var node = quest.GetQuestNode(endPosition);
            if (node == null) return;
            quest.RemoveQuestNode(node);
            OnManipulationEnd.Invoke();
        }
    }
}
