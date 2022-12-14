using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionNode : GrammarNode
{
    List<GrammarNode> nodes;

    public ProductionNode(string id, List<GrammarNode> rhs)
    {
        ID = id;
        nodes = rhs;
    }

    public ProductionNode(string id)
    {
        ID = id;
        nodes = new List<GrammarNode>();
    }

    public ProductionNode()
    {
        nodes = new List<GrammarNode>();
    }

    public override List<GrammarNode> GetTerminals()
    {
        var list = new List<GrammarNode>();
        foreach (var n in nodes)
        {
            list.AddRange(n.GetTerminals());
        }
        return list;
    }

    public override string GetText()
    {
        string text = "";
        foreach(var n in nodes)
        {
            text += n.GetText() + " ";
        }
        return text;
    }

    public void AppendNode(GrammarNode node)
    {
        nodes.Add(node);
    }
}
