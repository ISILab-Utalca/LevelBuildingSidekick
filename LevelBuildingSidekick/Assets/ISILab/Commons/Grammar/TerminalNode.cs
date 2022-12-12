using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerminalNode : GrammarNode
{
    string terminal;
    public string Text { get { return terminal; } set { terminal = value; } }


    public TerminalNode(string text)
    {
        terminal = text;
    }

    public override string GetText()
    {
        return terminal;
    }

    public override List<GrammarNode> GetTerminals()
    {
        return new List<GrammarNode>() {this};
    }
}
