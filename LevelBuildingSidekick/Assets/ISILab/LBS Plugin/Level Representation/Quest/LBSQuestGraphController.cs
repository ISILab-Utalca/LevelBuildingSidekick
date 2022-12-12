using LBS.Graph;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LBSQuestGraphController : LBSGraphController
{
    public LBSQuestGraphController(LBSGraphView view, LBSGraphData data) : base(view, data)
    {

    }

    internal override LBSNodeData NewNode(Vector2 position)
    {
        QuestGraphNode g = new QuestGraphNode("Undefined", position);
        return g;
    }

    public LBSNodeData NewNode(Vector2 position, GrammarNode grammarElement)
    {
        QuestGraphNode g = new QuestGraphNode(grammarElement, position);
        return g;
    }
}
