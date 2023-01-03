using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionNode : GrammarNode
{
    List<GrammarNode> nodes;
    public List<GrammarNode> Nodes => nodes;

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

    public override List<string> GetExpansionsText()
    {
        if (nodes.Count == 1 && nodes[0] is NonTerminalNode)
        {
            if (nodes[0] is NonTerminalNode)
            {
                return nodes[0].GetExpansionsText();
            }
            else
            {
                return new List<string>() { nodes[0].ID };
            }
        }

        var expansions = new List<string>();
        string expansion = "";
        foreach(var node in nodes)
        {
            expansion += node.ID + " ";
        }

        return new List<string>() { expansion };
    }

    public override List<GrammarNode> GetExpansion(int index)
    {

        if (nodes.Count == 1 && nodes[0] is NonTerminalNode)
        {
            return nodes[0].GetExpansion(index);
        }


        return nodes;
    }
}
