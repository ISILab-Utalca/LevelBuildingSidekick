using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Speech.Recognition.SrgsGrammar;
using System;

public class GrammarTree
{
    public Dictionary<string, TerminalNode> Terminals = new Dictionary<string, TerminalNode>();
    public Dictionary<string, NonTerminalNode> NonTerminals = new Dictionary<string, NonTerminalNode>();
    public Dictionary<string, ProductionNode> Productions = new Dictionary<string, ProductionNode>();
    public Dictionary<string, GrammarNode> Actions = new Dictionary<string, GrammarNode>();

    private GrammarNode root;
    public GrammarNode Root { 
        get => root;
        set 
        {
            root = value;
            UpdateRoot();
        } 
    }

    public GrammarTree()
    {
    }

    public GrammarTree(GrammarNode root)
    {
        Root = root;
    }

    public void UpdateRoot()
    {
        while (true)
        {
            //Debug.Log(root.GetType().Name);
            if (root is TerminalNode)
                break;
            if (root is ProductionNode)
            {
                var node = (root as ProductionNode);
                if (node.Nodes.Count > 1)
                {
                    //Debug.Log(Root.ID);
                    break;
                }
                root = node.Nodes[0];
                //Debug.Log("Out");
                continue;
            }
            if (root is NonTerminalNode)
            {
                var node = (root as NonTerminalNode);
                if (node.Nodes.Count > 1)
                {
                    //Debug.Log(root.ID);
                    break;
                }
                root = node.Nodes[0];
                //Debug.Log("Out");
                continue;
            }
        }

        Actions.Clear();

        if (root is NonTerminalNode)
        {
            var nodes = (root as NonTerminalNode).Nodes;
            foreach (var node in nodes)
            {
                Actions.Add(node.ID, node);
            }
        }
        else if (root is ProductionNode)
        {
            var nodes = (root as ProductionNode).Nodes;
            foreach (var node in nodes)
            {
                Actions.Add(node.ID, node);
            }
        }
    }

    internal GrammarNode GetGrammarElement(string grammarKey)
    {
        if (Terminals.ContainsKey(grammarKey))
        {
            return Terminals[grammarKey];
        }
        else if (NonTerminals.ContainsKey(grammarKey))
        {
            return NonTerminals[grammarKey];
        }
        else if(Productions.ContainsKey(grammarKey))
        {
            return Productions[grammarKey];
        }
        return null;

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
