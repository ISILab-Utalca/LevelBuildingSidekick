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

        /// <summary>
        /// Validates the passed action order (quest flow: graph of quest nodes). It checks their validity
        /// against the grammar rules in the Grammar file.
        /// </summary>
        /// <param name="actions"></param>
        /// <param name="candidates"></param>
        /// <returns></returns>
        internal bool Validate(List<string> actions, out List<List<GrammarElement>> candidates)
        {
            var root = grammarTree.Root;

            candidates = new List<List<GrammarElement>>();
            var first = new List<GrammarElement>();
            first.Add(root);
            candidates.Add(first);

            for (int i = 0; i < actions.Count; i++)
            {
                var newCandidates = new List<List<GrammarElement>>();

                // Partial exploration of grammar tree
                foreach (var c in candidates)
                {
                    newCandidates.AddRange(ProcessPhrase(c, actions));
                }

                candidates.Clear();

                // Pruning non-valid phrases
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
                List<GrammarElement> raw = toProcess[0];
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
                    var node = raw[i];

                    // If the current node is a production or non-terminal, we expand it
                    if (node is GrammarProduction)
                    {
                        var production = (node as GrammarProduction).Nodes;
                        raw.RemoveAt(i);
                        raw.InsertRange(i, production);
                        continue;
                    }

                    // Non-terminal processing
                    if (node is GrammarNonTerminal)
                    {
                        var nonTerminal = (node as GrammarNonTerminal).Nodes;

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

                visited.Add(raw);
                if (!raw.Any(n => !(n is GrammarTerminal)))
                {
                    processed.Add(raw);
                }
            }

            return processed;
        }

        /// <summary>
        /// Generates and prints all valid permutations based on the grammar.
        /// </summary>
        public void GenerateAndPrintQuestDefinitions()
        {
            var questDefinitions = GenerateQuestDefinitions();
            foreach (var quest in questDefinitions)
            {
                Debug.Log("Generated Quest Definition: " + string.Join(" -> ", quest));
            }
        }

        /// <summary>
        /// Generates all possible quest definitions by recursively expanding the grammar.
        /// </summary>
        public List<List<string>> GenerateQuestDefinitions()
        {
            var questDefinitions = new List<List<string>>();
            GenerateQuestDefinitionsRecursive(questDefinitions, new List<string>(), grammarTree.Root);
            return questDefinitions;
        }

        /// <summary>
        /// Recursively generates quest definitions by traversing the grammar tree.
        /// </summary>
        private void GenerateQuestDefinitionsRecursive(List<List<string>> questDefinitions, List<string> currentDefinition, GrammarElement currentNode)
        {
            if (currentNode == null || questDefinitions.Contains(new List<string>(currentDefinition))) {
                return; 
            }
            
            if (currentNode is GrammarTerminal terminal)
            {
                // Terminal node, add the action text to the current definition
                currentDefinition.Add(terminal.Text);
                questDefinitions.Add(new List<string>(currentDefinition));
                return;
            }

            if (currentNode is GrammarProduction production)
            {
                // If it's a production, explore all the possible expansions
                foreach (var subNode in production.Nodes)
                {
                    GenerateQuestDefinitionsRecursive(questDefinitions, currentDefinition, subNode);
                }
            }

            if (currentNode is GrammarNonTerminal nonTerminal)
            {
                // Non-terminal, recursively explore its options
                foreach (var subNode in nonTerminal.Nodes)
                {
                    GenerateQuestDefinitionsRecursive(questDefinitions, currentDefinition, subNode);
                }
            }
        }

        internal bool Validate(List<string> actions)
        {
            var output = new List<List<GrammarElement>>();
            return Validate(actions, out output);
        }
    }
}
