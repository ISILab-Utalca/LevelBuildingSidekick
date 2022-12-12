using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonTerminalNode : GrammarNode
{
    List<GrammarNode> nodes;

    public NonTerminalNode(List<GrammarNode> alternations)
    {
        nodes = alternations;
    }

    public override List<GrammarNode> GetTerminals()
    {
        return nodes[Random.Range(0, nodes.Count - 1)].GetTerminals();
    }

    public override string GetText()
    {
        return nodes[Random.Range(0, nodes.Count - 1)].GetText();
    }
}
