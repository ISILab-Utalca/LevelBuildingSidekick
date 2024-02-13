using ISILab.LBS.Manipulators;
using ISILab.LBS.Modules;
using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class ConnectQuestNodes : LBSManipulator
{
    QuestGraph quest;

    public QuestNode first;

    public ConnectQuestNodes() : base()
    {
        feedback = new ConnectedLine();
    }

    public override void Init(LBSLayer layer, object provider)
    {
        quest = layer.GetModule<QuestGraph>();
    }

    protected override void OnMouseDown(VisualElement target, Vector2Int startPosition, MouseDownEvent e)
    {
        first = quest.GetQuesNode(startPosition);
    }

    protected override void OnMouseMove(VisualElement target, Vector2Int movePosition, MouseMoveEvent e)
    {

    }

    protected override void OnMouseUp(VisualElement target, Vector2Int endPosition, MouseUpEvent e)
    {
        var second = quest.GetQuesNode(endPosition);
        quest.AddConnection(first, second);

    }

}
