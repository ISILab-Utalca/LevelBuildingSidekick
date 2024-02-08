using LBS;
using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

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

    protected override void OnMouseDown(VisualElement target, Vector2Int startPosition, MouseDownEvent e)
    {

    }

    protected override void OnMouseMove(VisualElement target, Vector2Int movePosition, MouseMoveEvent e)
    {

    }

    protected override void OnMouseUp(VisualElement target, Vector2Int endPosition, MouseUpEvent e)
    {
        quest.RemoveEdge(endPosition, 20);
    }
}
