using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonTerminalNode : GrammarNode
{
    List<GrammarNode> nodes;

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
}
