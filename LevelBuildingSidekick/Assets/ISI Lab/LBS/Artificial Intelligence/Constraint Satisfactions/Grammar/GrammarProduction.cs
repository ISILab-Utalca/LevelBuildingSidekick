using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[System.Serializable]
public class GrammarProduction : GrammarElement
{
    [SerializeField, SerializeReference]
    List<GrammarElement> nodes;
    public List<GrammarElement> Nodes => nodes;

    public GrammarProduction(string id, List<GrammarElement> rhs)
    {
        ID = id;
        nodes = rhs;
    }

    public GrammarProduction(string id)
    {
        ID = id;
        nodes = new List<GrammarElement>();
    }

    public GrammarProduction()
    {
        nodes = new List<GrammarElement>();
    }

    public override List<GrammarElement> GetTerminals()
    {
        var list = new List<GrammarElement>();
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

    public void AppendNode(GrammarElement node)
    {
        nodes.Add(node);
    }

    public override List<string> GetExpansionsText()
    {
        if (nodes.Count == 1 && nodes[0] is GrammarNonTerminal)
        {
            if (nodes[0] is GrammarNonTerminal)
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

    public override List<GrammarElement> GetExpansion(int index)
    {

        if (nodes.Count == 1 && nodes[0] is GrammarNonTerminal)
        {
            return nodes[0].GetExpansion(index);
        }


        return nodes;
    }

    public override object Clone()
    {
        return new GrammarProduction(ID, nodes.Select(e => e.Clone() as GrammarElement).ToList());
    }
}
