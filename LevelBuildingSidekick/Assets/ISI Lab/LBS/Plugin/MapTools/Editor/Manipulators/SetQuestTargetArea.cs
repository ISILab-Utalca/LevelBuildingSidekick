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

        protected override string IconGuid { get => "8636ef426c1415343a9f2f806cb42b28"; }

        public override void Init(LBSLayer layer, object provider)
        {
            base.Init(layer, provider);
        }

        protected override void OnMouseUp(VisualElement target, Vector2Int position, MouseUpEvent e)
        {
            var corners = questGraph.OwnerLayer.ToFixedPosition(StartPosition, EndPosition);

            node.Target.Rect = new Rect(corners.Item1, corners.Item2);
        }

    }
}