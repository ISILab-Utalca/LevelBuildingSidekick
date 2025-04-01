using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ISILab.LBS.Assistants;
using UnityEngine;

namespace ISILab.AI.Grammar
{
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
            foreach (var terminal in grammarTree.Terminals)
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
            // comment as no longer the first node is an empty start node
           // actions.RemoveAt(0);

            var root = grammarTree.Root;

            candidates = new List<List<GrammarElement>>();
            var first = new List<GrammarElement>();
            first.Add(root);
            candidates.Add(first);

            for (int i = 0; i < actions.Count; i++)
            {
                var newCandidates = new List<List<GrammarElement>>();

                //Partial exploration of grammar tree
                foreach (var c in candidates)
                {
                    newCandidates.AddRange(ProcessPhrase(c, actions));
                }

                candidates.Clear();

                //Prunning non valid phrases
                foreach (var c in newCandidates)
                {
                    if (c.Count != actions.Count)
                        continue;

                    var text = (c[i] as GrammarTerminal).Text;
                    if (text == actions[i])
                    {
                        candidates.Add(c);
                    }
                }
            }

            return candidates.Count > 0;
        }

        private List<List<GrammarElement>> ProcessPhrase(List<GrammarElement> phrase, List<string> actions)
        {
            var toProcess = new List<List<GrammarElement>>();
            var processed = new List<List<GrammarElement>>();


            var visited = new List<List<GrammarElement>>();

            toProcess.Insert(0, phrase);

            while (toProcess.Count > 0)
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

                for (int i = 0; i < raw.Count; i++)
                {

                    //In theory should never enter the catch
                    try
                    {
                        var node = raw[i];
                    }
                    catch
                    {
                        Debug.Log("Raw: " + raw.Count + " - first: " + raw[0] + " - last: " + raw[^1]);
                        break;
                    }


                    do
                    {
                        //if option bigger than current, invalid option
                        if (raw.Count > actions.Count)
                            break;

                        //if option has element that current don't, invalid option
                        bool unregisteredElement = false;
                        foreach (var n in raw)
                        {
                            if (n is GrammarTerminal && !actions.Contains(n.ID))
                            {
                                unregisteredElement = true;
                                break;
                            }
                        }

                        if (unregisteredElement)
                            break;

                        //If production replace node with expansion chain
                        if (raw[i] is GrammarProduction)
                        {
                            //replace for production
                            var production = (raw[i] as GrammarProduction).Nodes;
                            raw.RemoveAt(i);
                            raw.InsertRange(i, production);
                            continue;
                        }

                        //If nonTerminal replace 
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
                    while (!(raw[i] is GrammarTerminal));

                }

                visited.Add(raw);
                if (!raw.Any(n => !(n is GrammarTerminal)))
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
}

