using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[System.Serializable]
public class GrammarNonTerminal : GrammarElement
{
    [SerializeField, SerializeReference]
    List<GrammarElement> nodes;
    public List<GrammarElement> Nodes => nodes;

    public GrammarNonTerminal(string id, List<GrammarElement> alternations)
    {
        ID = id;
        nodes = alternations;
    }
    public GrammarNonTerminal(string id)
    {
        ID = id;
        nodes = new List<GrammarElement>();
    }

    public override List<GrammarElement> GetTerminals()
    {
        return nodes[Random.Range(0, nodes.Count - 1)].GetTerminals();
    }

    public override string GetText()
    {
        return nodes[Random.Range(0, nodes.Count - 1)].GetText();
    }
    public void AppendNode(GrammarElement node)
    {
        nodes.Add(node);
    }

    public override List<string> GetExpansionsText()
    {
        if (nodes.Count == 1)
        {
            return new List<string>() { nodes[0].ID };
        }

        var expansions = new  List<string>();

        foreach (var node in Nodes)
        {
            var s = node.ID + ": ";
            node.GetExpansionsText().ForEach(n => s += n + " ");
            expansions.Add(s);
        }
        return expansions;
    }

    public override List<GrammarElement> GetExpansion(int index)
    {
        if (index < 0 || index >= nodes.Count)
        {
            Debug.Log("WTF");
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

    public override object Clone()
    {
        return new GrammarNonTerminal(ID, Nodes.Select(e => e.Clone() as GrammarElement).ToList());
    }
}
