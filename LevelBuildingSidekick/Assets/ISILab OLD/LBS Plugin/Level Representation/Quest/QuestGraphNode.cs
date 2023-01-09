using LBS.Graph;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGraphNode : LBSNodeData
{
    string grammarKey;
    public string GrammarKey => grammarKey;

    private int expandTo;

    QuestGraphNode parent;
    public QuestGraphNode Parent
    {
        get => parent;
        set => parent = value;
    }

    List<QuestGraphNode> children;

    public List<QuestGraphNode> Children
    {
        get
        {
            if (children == null)
                children = new List<QuestGraphNode>();
            return children;
        }
        set => children = value;
    }

    public int ExpandTo
    {
        get => expandTo;
        set => expandTo = value;
    }

    public QuestGraphNode(string label, Vector2 position, QuestGraphNode parent = null) : base(label, position)
    {
        this.grammarKey = label;
        Label = label;
        expandTo = -1;
        children = new List<QuestGraphNode>();
        Parent = parent;
    }

    public void Expand(int option, GrammarTree grammar)
    {

        /*if (option == expandTo)
            return;*/

        expandTo = option;
        var element = grammar.GetGrammarElement(grammarKey);



        if (element == null)
        {
            return;
        }

        var nodes = element.GetExpansion(option);

        Children.Clear();

        for(int i = 0; i < nodes.Count; i++)
        {
            Children.Add(new QuestGraphNode(nodes[i].ID, new Vector2(64*i*Width*2.5f,0), this));
        }
    }
}
