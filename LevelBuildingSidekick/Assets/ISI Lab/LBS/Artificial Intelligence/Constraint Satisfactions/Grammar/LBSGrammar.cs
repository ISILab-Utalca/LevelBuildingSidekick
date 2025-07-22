using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ISILab.LBS.Assistants;
using ISILab.LBS.Components;
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
        private List<ActionTargetDepiction> actions = new();

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


        /* DEPRECATED
         -------
        /// <summary>
        /// Validates the passed action order (quest flow: graph of quest nodes). It checks their validity
        /// against the grammar rules in the Grammar file.
        /// </summary>
        /// <param name="actions"></param>
        /// <param name="candidates"></param>
        /// <returns></returns>
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
        */
        
        
        /// <summary>
        /// Validates the given list of quest nodes by checking for all valid phrases they can form,
        /// based on the defined grammar.
        /// Returns a tuple containing a boolean indicating if any valid phrase was found,
        /// and the list of quest nodes that matched valid phrases.
        /// </summary>
        /// <param name="questNodes">The list of quest nodes to validate.</param>
        /// <returns>A tuple: (bool isValid, List of matching quest nodes).</returns>
        internal Tuple<bool, List<QuestNode>> NewValidate(List<QuestNode> questNodes)
        {
            var result = Tuple.Create(false, new List<QuestNode>());
            int index = 0;

            while (index < questNodes.Count)
            {
                bool found = false;

                // Try to match from current index to as long as possible
                for (int length = questNodes.Count - index; length > 0; length--)
                {
                    var nodeSlice = questNodes.GetRange(index, length);
                    var actionSlice = nodeSlice.Select(n => n.QuestAction).ToList();

                    if (!NewValidateSinglePhrase(actionSlice)) continue;
                    
                    index += length;
                    found = true;
                        
                    foreach (var node in nodeSlice)
                    {
                        result.Item2.Add(node);
                    }

                    break;
                }

                if (!found)
                {
                    return result;
                }
            }

            return Tuple.Create(true, result.Item2);
        }


 
        /// <summary>
        /// Attempts to match the given phrase against the grammar.
        /// Iterates through candidate grammar paths and checks if the terminal at the current position
        /// matches the corresponding word in the phrase.
        /// If a valid candidate is found, returns true and outputs the first valid match.
        /// </summary>
        /// <param name="phrase">The list of actions (as strings) to match.</param>
        /// <returns>True if a valid match is found; otherwise, false.</returns>

        private bool NewValidateSinglePhrase(List<string> phrase)
        {
            var root = grammarTree.Root;

            var candidates = new List<List<GrammarElement>> { new() { root } };

            for (var i = 0; i < phrase.Count; i++)
            {
                var newCandidates = new List<List<GrammarElement>>();

                foreach (var c in candidates)
                {
                    newCandidates.AddRange(ProcessPhrase(c, phrase));
                }

                candidates.Clear();

                foreach (var c in newCandidates)
                {
                    if (c.Count != phrase.Count)
                        continue;

                    var terminal = c[i] as GrammarTerminal;
                    if (terminal != null && terminal.Text == phrase[i])
                    {
                        candidates.Add(c);
                    }
                }
            }

            return candidates.Count > 0;
        }

        /// <summary>
        /// Expands a phrase (a list of grammar elements) by recursively processing non-terminals and productions
        /// to generate all possible terminal sequences that could match a given list of actions.
        /// Only complete terminal sequences are kept.
        /// </summary>
        /// <param name="phrase">The initial grammar phrase to process.</param>
        /// <param name="actions">The list of action strings to use as a filter for pruning invalid expansions.</param>
        /// <returns>A list of fully expanded phrases (only terminals), representing all valid sequences derived from</returns>

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
                        }
                    }
                    while (raw[i] is not GrammarTerminal);

                }

                visited.Add(raw);
                if (raw.All(n => n is GrammarTerminal))
                {
                    processed.Add(raw);
                }
            }
            return processed;
        }
        
        public Tuple<bool, List<QuestNode>> Validate(List<QuestNode> nodes)
        {
            return NewValidate(nodes);
        }
        
    }
}

