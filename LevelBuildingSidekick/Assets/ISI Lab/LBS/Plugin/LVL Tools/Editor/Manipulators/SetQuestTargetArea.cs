using ISILab.AI.Optimization.Populations;
using LBS;
using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SetQuestTargetArea : LBSManipulator
{
    QuestNode node;
    QuestGraph questGraph;


    public override void Init(LBSLayer layer, object provider)
    {

    }

    protected override void OnMouseDown(VisualElement target, Vector2Int position, MouseDownEvent e)
    {

    }

    protected override void OnMouseMove(VisualElement target, Vector2Int position, MouseMoveEvent e)
    {
        //throw new System.NotImplementedException();
    }

    protected override void OnMouseUp(VisualElement target, Vector2Int position, MouseUpEvent e)
    {
        var corners = questGraph.Owner.ToFixedPosition(StartPosition, EndPosition);

        node.Target.Rect = new Rect(corners.Item1, corners.Item2);
    }

}
