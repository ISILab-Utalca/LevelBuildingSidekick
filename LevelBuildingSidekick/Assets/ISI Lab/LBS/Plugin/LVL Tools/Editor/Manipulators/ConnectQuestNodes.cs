using LBS;
using LBS.Components;
using LBS.Components.Graph;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class ConnectQuestNodes : LBSManipulator
{
    //QuestBehaviour quest;
    LBSQuestGraph quest;

    public LBSQuestGraph Quest
    {
        get => quest;
        set => quest = value;
    }

    public QuestNode first;

    public ConnectQuestNodes() : base()
    {
        feedback = new ConnectedLine();
    }

    public override void Init(LBSLayer layer, object provider)
    {
        //quest = provider as QuestBehaviour;
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
        Debug.Log(first + " - " + second);
        quest.AddConnection(first, second);

    }

}
