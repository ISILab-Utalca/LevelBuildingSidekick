using LBS.Graph;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGraphNode : LBSNodeData
{
    string grammarKey;

    public QuestGraphNode(string label, Vector2 position) : base(label, position) 
    {
        this.grammarKey = label;
        Debug.Log(Label);
        Label = label;
    }
}
