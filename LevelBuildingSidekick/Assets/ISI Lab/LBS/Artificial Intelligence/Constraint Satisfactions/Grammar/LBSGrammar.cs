using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        actions.RemoveAt(0);

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
                //Debug.Log(c[i].ID);
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
        var toProcess = new List<List<GrammarElement>>();
        var processed = new List<List<GrammarElement>>();


        var visited = new List<List<GrammarElement>>();

        toProcess.Insert(0, phrase);

        //Debug.Log(toProcess.Count);

        int k = 0;

        while(toProcess.Count > 0 && k < 100)
        {
            var raw = toProcess[0];
            toProcess.RemoveAt(0);

            bool closed = visited.Any(r =>
            {
                if (r.Count != raw.Count)
                    return false;
                for (int i = 0; i < raw.Count; i++)
                {
                    if (!raw[i].Equals(r[i]))
                        return false;
                }
                return true;
            });

            if (closed)
                continue;

            Debug.Log("Start " + startIndex + " - " + lastIndex);
            for (int i = startIndex; i <= lastIndex; i++)
            {
                do
                {
                    k++;
                    GrammarElement node;
                    try
                    {
                        node = raw[i];
                    }
                    catch
                    {
                        break;
                    }

                    //Debug.Log("R: " + raw[i]);

                    if (raw[i] is GrammarProduction)
                    {
                        //replace for production
                        var production = (raw[i] as GrammarProduction).Nodes;
                        raw.RemoveAt(i);
                        raw.InsertRange(i, production);
                        continue;
                    }


                    if (raw[i] is GrammarNonTerminal)
                    {
                        var nonTerminal = (raw[i] as GrammarNonTerminal).Nodes;

                        for (int j = 1; j < nonTerminal.Count; j++)
                        {
                            var newRaw = new List<GrammarElement>(raw);
                            newRaw.RemoveAt(i);
                            newRaw.Insert(i, nonTerminal[j]);
                            toProcess.Add(newRaw);
                        }

                        raw.RemoveAt(i);
                        raw.Insert(i, nonTerminal[0]);
                        continue;
                    }
                }
                while (i < raw.Count && !(raw[i] is GrammarTerminal) && k < 100) ;

                Debug.Log( i + " Iterations: " + k);

                if (k > 100)
                break;
            }

            visited.Add(raw);
            if(lastIndex < raw.Count)
            {
                processed.Add(raw);
            }

        }
        Debug.Log("VISITED: " + visited.Count);

        foreach (var v in visited)
        {
            var s = "VISITED: ";
            foreach(var g in v)
            {
                s += g.ID + " ; ";
            }
            Debug.Log(s);
        }


        return processed;
    }

    
    internal bool Validate(List<string> actions)
    {
        var output = new List<List<GrammarElement>>();
        return Validate(actions, out output);
    }
}

