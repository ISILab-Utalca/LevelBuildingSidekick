using ISILab.AI.Optimization.Populations;
using ISILab.LBS.Components;
using ISILab.LBS.Modules;
using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Manipulators
{
    public class SetQuestTargetArea : LBSManipulator
    {
        QuestNode node;
        QuestGraph questGraph;

        public override void Init(LBSLayer layer, object provider)
        {
            base.Init(layer, provider);
        }

        protected override void OnMouseUp(VisualElement target, Vector2Int position, MouseUpEvent e)
        {
            var corners = questGraph.Owner.ToFixedPosition(StartPosition, EndPosition);

            node.Target.Rect = new Rect(corners.Item1, corners.Item2);
        }

    }
}