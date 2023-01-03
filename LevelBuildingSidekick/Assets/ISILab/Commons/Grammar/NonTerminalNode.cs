using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonTerminalNode : GrammarNode
{
    List<GrammarNode> nodes;
    public List<GrammarNode> Nodes => nodes;

    public NonTerminalNode(string id, List<GrammarNode> alternations)
    {
        ID = id;
        nodes = alternations;
    }
    public NonTerminalNode(string id)
    {
        ID = id;
        nodes = new List<GrammarNode>();
    }

    public override List<GrammarNode> GetTerminals()
    {
        return nodes[Random.Range(0, nodes.Count - 1)].GetTerminals();
    }

    public override string GetText()
    {
        return nodes[Random.Range(0, nodes.Count - 1)].GetText();
    }
    public void AppendNode(GrammarNode node)
    {
        nodes.Add(node);
    }

    public override List<string> GetExpansionsText()
    {/*
        if (nodes.Count == 1)
        {
            return  nodes[0].GetExpansionsText();
        }*/

        var expansions = new  List<string>();

        foreach (var node in Nodes)
        {
            var s = "";
            node.GetExpansionsText().ForEach(n => s += n + " ");
            expansions.Add(s);
        }
        return expansions;
    }

    public override List<GrammarNode> GetExpansion(int index)
    {
        if (index < 0 || index >= nodes.Count)
        {
            foreach(var n in nodes)
            {
                Debug.Log(n.ID);
            }
            return null;
        }


        return nodes[index].GetExpansion(index);
    }

    public override string ToString()
    {
        return base.ToString();
    }
}
