using LBS;
using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class RemoveNodeGrammar : LBSManipulator
{
    QuestBehaviour quest;
    LBSGraph graph;

    public RemoveNodeGrammar() : base()
    {

    }

    public override void Init(LBSLayer layer, object provider)
    {
        quest = provider as QuestBehaviour;
        graph = layer.GetModule<LBSGraph>();
    }

    protected override void OnMouseDown(VisualElement target, Vector2Int startPosition, MouseDownEvent e)
    {

    }

    protected override void OnMouseMove(VisualElement target, Vector2Int movePosition, MouseMoveEvent e)
    {

    }

    protected override void OnMouseUp(VisualElement target, Vector2Int endPosition, MouseUpEvent e)
    {
        var pair = quest.GetNodes().OrderBy(n => (n.Node.Position - endPosition).magnitude).First();

        quest.RemoveNode(pair);

    }
}
