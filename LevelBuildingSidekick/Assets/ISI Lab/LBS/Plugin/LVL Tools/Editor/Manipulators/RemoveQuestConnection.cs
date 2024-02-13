using ISILab.LBS.Modules;
using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Manipulators
{
    public class RemoveQuestConnection : LBSManipulator
    {
        //QuestBehaviour quest;
        QuestGraph quest;

        public RemoveQuestConnection() : base()
        {

        }

        public override void Init(LBSLayer layer, object provider)
        {
            quest = layer.GetModule<QuestGraph>();
        }


        protected override void OnMouseUp(VisualElement target, Vector2Int endPosition, MouseUpEvent e)
        {
            quest.RemoveEdge(endPosition, 20);
        }
    }
}