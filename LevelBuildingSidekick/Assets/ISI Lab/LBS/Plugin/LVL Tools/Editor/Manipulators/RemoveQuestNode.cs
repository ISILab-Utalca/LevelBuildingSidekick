using LBS;
using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class RemoveQuestNode : LBSManipulator
{
    //QuestBehaviour quest;
    LBSQuestGraph quest;

    public RemoveQuestNode() : base()
    {

    }

    public override void Init(LBSLayer layer, object provider)
    {
        quest = layer.GetModule<LBSQuestGraph>();
    }

    protected override void OnMouseDown(VisualElement target, Vector2Int startPosition, MouseDownEvent e)
    {

    }

    protected override void OnMouseMove(VisualElement target, Vector2Int movePosition, MouseMoveEvent e)
    {

    }

    protected override void OnMouseUp(VisualElement target, Vector2Int endPosition, MouseUpEvent e)
    {
        var node = quest.GetQuesNode(endPosition);

        quest.RemoveQuestNode(node);

    }
}
