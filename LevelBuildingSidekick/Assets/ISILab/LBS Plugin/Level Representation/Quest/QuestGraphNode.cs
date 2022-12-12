using LBS.Graph;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGraphNode : LBSNodeData
{
    GrammarNode grammarElement;

    public QuestGraphNode(string label, Vector2 position) : base(label, position) { }

    public QuestGraphNode(GrammarNode grammarElement, Vector2 position) : base(grammarElement.ID, position)
    {
        this.grammarElement = grammarElement;
    }
}
