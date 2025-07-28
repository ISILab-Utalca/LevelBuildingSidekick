using System;
using System.Collections.Generic;
using System.Linq;
using ISILab.LBS.Components;
using ISILab.LBS.Modules;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UIElements;
using ISILab.AI.Grammar;
using LBS.Components;

namespace ISILab.LBS.Assistants
{
    [Serializable]
    [RequieredModule(typeof(QuestGraph))]
    public class GrammarAssistant : LBSAssistant
    {
        [JsonIgnore]
        public QuestGraph Quest => OwnerLayer.GetModule<QuestGraph>();

        public GrammarAssistant(VectorImage icon, string name, Color colorTint)
            : base(icon, name, colorTint) { }

        public override object Clone()
        {
            return new GrammarAssistant(Icon, this.Name, this.ColorTint);
        }

        public bool ValidateQuestGraph()
        {
            foreach (var node in Quest.QuestNodes)
            {
                if (!node.ValidGrammar) return false;
            }

            return true;
        }
        
        public bool ValidateEdgeGrammar(QuestEdge edge)
        {
            if (edge?.From is null || edge.To is null) return false;

            var grammar = Quest.Grammar;
            if (grammar == null || grammar.GetRules().Count == 0) return false;

            List<string> validTerminals = GetAllValidNextActions(edge.From.QuestAction);
            bool valid = validTerminals.Contains(edge.To.QuestAction);
            // Update the nodes grammar State
            edge.From.ValidGrammar = valid;
            
            // If the To is the goal and this one's grammar is valid so is the next one
            if(valid && edge.To.NodeType == NodeType.Goal) edge.To.ValidGrammar = true;
            return validTerminals.Contains(edge.To.QuestAction);
        }

        public List<string> GetAllValidNextActions(string currentAction)
        {
            var grammar = Quest.Grammar;
            var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (grammar == null) return result.ToList();

            var rules = grammar.GetRules();

            // Step 1: Check for direct next terminals in expansions
            foreach (var kvp in rules)
            {
                foreach (var expansion in kvp.Value)
                {
                    for (int i = 0; i < expansion.Count - 1; i++)
                    {
                        var current = expansion[i];
                        var next = expansion[i + 1];

                        if (current.Equals(currentAction, StringComparison.OrdinalIgnoreCase))
                        {
                            if (!IsNonTerminal(next))
                            {
                                result.Add(next);
                            }
                            else
                            {
                                result.UnionWith(GetFirstTerminals(next.TrimStart('#'), grammar));
                            }
                        }
                    }
                }
            }

            // Step 2: Check if currentAction is the last terminal in any non-terminal's expansion
            var parentNonTerminals = new HashSet<string>();
            foreach (var kvp in rules)
            {
                foreach (var expansion in kvp.Value)
                {
                    if (expansion.Count > 0 &&
                        expansion[^1].Equals(currentAction, StringComparison.OrdinalIgnoreCase) &&
                        !IsNonTerminal(expansion[^1]))
                    {
                        parentNonTerminals.Add(kvp.Key);
                    }
                }
            }

            // Step 3: Find where these non-terminals appear and collect following terminals
            foreach (var kvp in rules)
            {
                foreach (var expansion in kvp.Value)
                {
                    for (int i = 0; i < expansion.Count - 1; i++)
                    {
                        var current = expansion[i];
                        var next = expansion[i + 1];

                        if (IsNonTerminal(current) &&
                            parentNonTerminals.Contains(current.TrimStart('#')))
                        {
                            if (!IsNonTerminal(next))
                            {
                                result.Add(next);
                            }
                            else
                            {
                                result.UnionWith(GetFirstTerminals(next.TrimStart('#'), grammar));
                            }
                        }
                    }
                }
            }

            return result.ToList();
        }

        public List<string> GetNextNonTerminalFirsts(string currentAction, string nonTerminal)
        {
            var grammar = Quest.Grammar;
            var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (grammar == null) return result.ToList();

            var targetNonTerminal = nonTerminal.StartsWith("#") ? nonTerminal : "#" + nonTerminal;
            var rules = grammar.GetRules();

            foreach (var kvp in rules)
            {
                foreach (var expansion in kvp.Value)
                {
                    for (int i = 0; i < expansion.Count - 1; i++)
                    {
                        if (expansion[i].Equals(currentAction, StringComparison.OrdinalIgnoreCase) &&
                            expansion[i + 1].Equals(targetNonTerminal, StringComparison.OrdinalIgnoreCase))
                        {
                            if (IsNonTerminal(expansion[i + 1]))
                            {
                                result.UnionWith(GetFirstTerminals(targetNonTerminal.TrimStart('#'), grammar));
                            }
                        }
                    }
                }
            }

            return result.ToList();
        }

        private List<string> GetFirstTerminals(string ruleName, LBSGrammar grammar)
        {
            var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var rules = grammar.GetRules();

            if (!rules.TryGetValue(ruleName, out var expansions))
                return result.ToList();

            var visited = new HashSet<string>();
            CollectFirstTerminalsRecursive(expansions, rules, result, visited);

            return result.ToList();
        }

        private void CollectFirstTerminalsRecursive(List<List<string>> expansions, Dictionary<string, List<List<string>>> rules, HashSet<string> result, HashSet<string> visited)
        {
            foreach (var expansion in expansions)
            {
                if (expansion.Count == 0) continue;

                var first = expansion[0];
                if (!IsNonTerminal(first))
                {
                    result.Add(first);
                }
                else
                {
                    string ruleName = first.TrimStart('#');
                    if (visited.Contains(ruleName)) continue;

                    visited.Add(ruleName);
                    if (rules.TryGetValue(ruleName, out var nestedExpansions))
                    {
                        CollectFirstTerminalsRecursive(nestedExpansions, rules, result, visited);
                    }
                }
            }
        }

        public List<string> GetAllValidPrevActions(string currentAction)
        {
            var grammar = Quest.Grammar;
            var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (grammar == null) return result.ToList();

            var rules = grammar.GetRules();

            foreach (var kvp in rules)
            {
                foreach (var expansion in kvp.Value)
                {
                    for (int i = 1; i < expansion.Count; i++)
                    {
                        var current = expansion[i];
                        var prev = expansion[i - 1];

                        if (current.Equals(currentAction, StringComparison.OrdinalIgnoreCase))
                        {
                            if (!IsNonTerminal(prev))
                            {
                                result.Add(prev);
                            }
                            else
                            {
                                result.UnionWith(GetFirstTerminals(prev.TrimStart('#'), grammar));
                            }
                        }
                    }
                }
            }

            return result.ToList();
        }

        public List<List<string>> GetAllExpansions(string currentAction)
        {
            var grammar = Quest.Grammar;
            if (grammar == null || !grammar.GetRules().TryGetValue(currentAction, out var expansions))
                return new List<List<string>>();

            var result = new List<List<string>>();
            foreach (var expansion in expansions)
            {
                var sequence = new List<string>();
                foreach (var symbol in expansion)
                {
                    if (!IsNonTerminal(symbol))
                    {
                        sequence.Add(symbol);
                    }
                    else
                    {
                        sequence.AddRange(GetFirstTerminals(symbol.TrimStart('#'), grammar));
                    }
                }
                if (sequence.Count > 0)
                    result.Add(sequence);
            }

            return result;
        }
        
        public override void OnAttachLayer(LBSLayer layer)
        {
            base.OnAttachLayer(layer);
         
            Quest.OnAddEdge += (edge)=>
            {
                ValidateEdgeGrammar(edge);
            };
            
            // Removing an edge changes the nodes grammar to false
            Quest.OnRemoveEdge += (edge)=>
            {
                if (edge.To is not null) edge.To.ValidGrammar = false;
                if (edge.From is not null) edge.From.ValidGrammar = false;
            };
        }

        public override void OnGUI() { }

        private bool IsNonTerminal(string symbol) => symbol.Trim().StartsWith("#");
    }
}