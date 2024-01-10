using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ISILab/LBS/LBSGrammar")]
[System.Serializable]
public class LBSGrammar : ScriptableObject
{
    [SerializeField]
    GrammarTree grammarTree;

    [SerializeField]
    private List<ActionTargetDepiction> actions;

    public int ActionCount => actions.Count;

    public List<ActionTargetDepiction> Actions => new List<ActionTargetDepiction>(actions);

    public GrammarTree GrammarTree
    {
        get => grammarTree;
        set
        {
            grammarTree = value;
            UpdateActions();
        }
    }

    private void UpdateActions()
    {
        actions.Clear();
        foreach(var terminal in grammarTree.Terminals)
        {
            actions.Add(new ActionTargetDepiction(terminal, new List<string>()));
        }
    }

    public ActionTargetDepiction GetAction(int index)
    {
        return actions[index];
    }

    internal bool Validate(List<string> actions, out List<List<GrammarElement>> candidates)
    {

        var root = grammarTree.Root;

        candidates = new List<List<GrammarElement>>();
        var first = new List<GrammarElement>();
        first.Add(root);
        candidates.Add(first);

        for(int i = 0; i < actions.Count; i++)
        {
            if(candidates.Count == 0)
                return false;

            var newCandidates = new List<List<GrammarElement>>();

            //Partial exploration of grammar tree
            foreach(var c in candidates)
            {
                newCandidates.AddRange(ProcessPhrase(i, i, c));
            }

            candidates.Clear();

            //Prunning non valid phrases
            foreach(var c in newCandidates)
            {
                var text = (c[i] as GrammarTerminal).Text;
                if (text == actions[i])
                {
                    candidates.Add(c);
                }
            }
        }

        return true;
    }

    private List<List<GrammarElement>> ProcessPhrase(int startIndex, int lastIndex, List<GrammarElement> phrase)
    {
        var toProcess = new Queue<List<GrammarElement>>();
        var processed = new List<List<GrammarElement>>();

        toProcess.Enqueue(phrase);

        while(toProcess.Count > 0)
        {
            var raw = toProcess.Dequeue();

            for (int i = startIndex; i <= lastIndex; i++)
            {
                GrammarElement node;
                try
                {
                    node = raw[i];
                }
                catch
                {
                    break;
                }


                if (node is GrammarTerminal)
                {
                    continue;
                }

                if (node is GrammarProduction)
                {
                    //replace for production
                    var production = (node as GrammarProduction).Nodes;
                    raw.RemoveAt(i);
                    raw.InsertRange(i, production);
                    i--;
                    continue;
                }

                if (node is GrammarNonTerminal)
                {
                    var nonTerminal = (node as GrammarNonTerminal).Nodes;

                    for(int j = 1; j < nonTerminal.Count; j++)
                    {
                        var newRaw = new List<GrammarElement>(raw);
                        newRaw.RemoveAt(i);
                        newRaw.Insert(i, nonTerminal[j]);
                        toProcess.Enqueue(newRaw);
                    }

                    raw.RemoveAt(i);
                    raw.Insert(i, nonTerminal[0]);
                    i--;
                    continue;
                }
            }

            if(lastIndex < raw.Count)
            {
                processed.Add(raw);
            }
        }
        
        return processed;
    }

    internal bool Validate(List<string> actions)
    {
        var output = new List<List<GrammarElement>>();
        return Validate(actions, out output);
    }
}

