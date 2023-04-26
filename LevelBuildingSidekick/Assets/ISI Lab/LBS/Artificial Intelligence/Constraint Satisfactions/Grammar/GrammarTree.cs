using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Speech.Recognition.SrgsGrammar;
using System;
using System.Linq;

[System.Serializable]
public class GrammarTree
{
    [SerializeField]
    public List<GrammarTerminal> Terminals = new List<GrammarTerminal>();
    [SerializeField]
    public List<GrammarNonTerminal> NonTerminals = new List<GrammarNonTerminal>();
    [SerializeField]
    public List<GrammarProduction> Productions = new List<GrammarProduction>();

    [SerializeField, SerializeReference]
    private GrammarElement root;
    public GrammarElement Root { 
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

    public GrammarTree(GrammarElement root)
    {
        Root = root;
    }

    public void UpdateRoot()
    {
        while (true)
        {
            if (root is GrammarTerminal)
                break;
            if (root is GrammarProduction)
            {
                var node = (root as GrammarProduction);
                if (node.Nodes.Count > 1)
                {
                    break;
                }
                root = node.Nodes[0];
                continue;
            }
            if (root is GrammarNonTerminal)
            {
                var node = (root as GrammarNonTerminal);
                if (node.Nodes.Count > 1)
                {
                    break;
                }
                root = node.Nodes[0];
                continue;
            }
        }
    }

    public List<GrammarElement> GetActions()
    {
        var actions = new List<GrammarElement>();
        if (root is GrammarNonTerminal)
        {
            var nodes = (root as GrammarNonTerminal).Nodes;
            foreach (var node in nodes)
            {
                actions.Add(node);
            }
        }
        else if (root is GrammarProduction)
        {
            var nodes = (root as GrammarProduction).Nodes;
            foreach (var node in nodes)
            {
                actions.Add(node);
            }
        }
        return actions;
    }

    internal GrammarElement GetGrammarElement(string grammarKey)
    {
        if (Terminals.Any(g => g.ID == grammarKey))
        {
            return Terminals.Find(g => g.ID == grammarKey);
        }
        else if (NonTerminals.Any(g => g.ID == grammarKey))
        {
            return NonTerminals.Find(g => g.ID == grammarKey);
        }
        else if(Productions.Any(g => g.ID == grammarKey))
        {
            return Productions.Find(g => g.ID == grammarKey);
        }
        return null;

    }

    public string TextSentence()
    {
        return Root.GetText();
    }

    public List<GrammarElement> NodeSentence()
    {
        return Root.GetTerminals();
    }
}
