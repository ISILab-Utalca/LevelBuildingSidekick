using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Speech.Recognition.SrgsGrammar;

public class GrammarTree
{
    public Dictionary<string, TerminalNode> Terminals = new Dictionary<string, TerminalNode>();
    public Dictionary<string, NonTerminalNode> NonTerminals = new Dictionary<string, NonTerminalNode>();
    public Dictionary<string, ProductionNode> Productions = new Dictionary<string, ProductionNode>();

    public GrammarNode Root { get; set; }

    public GrammarTree()
    {
    }

    public GrammarTree(GrammarNode root)
    {
        Root = root;
    }

    public string TextSentence()
    {
        return Root.GetText();
    }

    public List<GrammarNode> NodeSentence()
    {
        return Root.GetTerminals();
    }
}
