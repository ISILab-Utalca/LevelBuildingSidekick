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

        public void ValidateNodeGrammar(QuestNode node)
        {
            var grammar = Quest.Grammar;
            if (grammar == null || grammar.GetRules().Count == 0) return;

            var paths = BuildAllPathsThroughNode(node);
            foreach (var n in paths.SelectMany(p => p))
                n.ValidGrammar = false;

            foreach (var path in paths)
            {
                if (IsValidSequence(path.Select(n => n.QuestAction).ToList(), grammar))
                {
                    foreach (var n in path)
                        n.ValidGrammar = true;
                }
            }
        }

        public void ValidateEdgeGrammar(QuestEdge edge)
        {
            if (edge == null) return;

            var grammar = Quest.Grammar;
            if (grammar == null || grammar.GetRules().Count == 0) return;

            var paths = BuildAllPathsThroughEdge(edge);
            foreach (var n in paths.SelectMany(p => p))
                n.ValidGrammar = false;

            foreach (var path in paths)
            {
                if (IsValidSequence(path.Select(n => n.QuestAction).ToList(), grammar))
                {
                    foreach (var n in path)
                        n.ValidGrammar = true;
                }
            }
        }

        private bool IsValidSequence(List<string> actions, LBSGrammar grammar)
        {
            if (actions.Count < 2) return true;

            for (int i = 0; i < actions.Count - 1; i++)
            {
                var current = actions[i];
                var next = actions[i + 1];

                if (!grammar.GetRules().TryGetValue(current, out var expansions))
                    return false;

                bool valid = false;
                foreach (var expansion in expansions)
                {
                    if (expansion.Count == 0) continue;

                    var firstGrammar = expansion[0];
                    if (firstGrammar.name.Equals(next, StringComparison.OrdinalIgnoreCase))
                    {
                        valid = true;
                        break;
                    }

                    if (firstGrammar is NonTerminal nt)
                    {
                        var firsts = GetFirstTerminals(nt, grammar);
                        if (firsts.Contains(next, StringComparer.OrdinalIgnoreCase))
                        {
                            valid = true;
                            break;
                        }
                    }
                }

                if (!valid) return false;
            }

            return true;
        }

        public bool FastValidGrammar(List<QuestNode> nodes)
        {
            return nodes.All(n => n.ValidGrammar);
        }

        public List<string> GetSuggestions(QuestNode node)
        {
            var suggestions = new List<string>();
            var grammar = Quest.Grammar;

            if (grammar == null || !grammar.GetRules().TryGetValue(node.QuestAction, out var expansions))
                return suggestions;

            foreach (var expansion in expansions)
            {
                if (expansion.Count > 0)
                {
                    var first = expansion[0];
                    if (first is TerminalGrammar terminal)
                    {
                        suggestions.Add(terminal.name);
                    }
                    else if (first is NonTerminal nonTerminal)
                    {
                        suggestions.AddRange(GetFirstTerminals(nonTerminal, grammar));
                    }
                }
            }

            return suggestions.Distinct().ToList();
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

                        if (current.name.Equals(currentAction, StringComparison.OrdinalIgnoreCase))
                        {
                            if (next is TerminalGrammar terminal)
                            {
                                result.Add(terminal.name);
                            }
                            else if (next is NonTerminal nonTerminal)
                            {
                                result.UnionWith(GetFirstTerminals(nonTerminal, grammar));
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
                        expansion[^1].name.Equals(currentAction, StringComparison.OrdinalIgnoreCase) &&
                        expansion[^1] is TerminalGrammar)
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

                        if (current is NonTerminal nt && 
                            parentNonTerminals.Contains(nt.name.TrimStart('#')))
                        {
                            if (next is TerminalGrammar terminal)
                            {
                                result.Add(terminal.name);
                            }
                            else if (next is NonTerminal nonTerminal)
                            {
                                result.UnionWith(GetFirstTerminals(nonTerminal, grammar));
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
                        if (expansion[i].name.Equals(currentAction, StringComparison.OrdinalIgnoreCase) &&
                            expansion[i + 1].name.Equals(targetNonTerminal, StringComparison.OrdinalIgnoreCase))
                        {
                            if (expansion[i + 1] is NonTerminal nt)
                            {
                                result.UnionWith(GetFirstTerminals(nt, grammar));
                            }
                        }
                    }
                }
            }

            return result.ToList();
        }

        private List<string> GetFirstTerminals(NonTerminal nonTerminal, LBSGrammar grammar)
        {
            var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var rules = grammar.GetRules();
            string ruleName = nonTerminal.name.TrimStart('#');

            if (!rules.TryGetValue(ruleName, out var expansions))
                return result.ToList();

            var visited = new HashSet<string>();
            CollectFirstTerminalsRecursive(expansions, rules, result, visited);

            return result.ToList();
        }

        private void CollectFirstTerminalsRecursive(List<List<Grammar>> expansions, Dictionary<string, List<List<Grammar>>> rules, HashSet<string> result, HashSet<string> visited)
        {
            foreach (var expansion in expansions)
            {
                if (expansion.Count == 0) continue;

                var first = expansion[0];
                if (first is TerminalGrammar terminal)
                {
                    result.Add(terminal.name);
                }
                else if (first is NonTerminal nt)
                {
                    string ruleName = nt.name.TrimStart('#');
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

                        if (current.name.Equals(currentAction, StringComparison.OrdinalIgnoreCase))
                        {
                            if (prev is TerminalGrammar terminal)
                            {
                                result.Add(terminal.name);
                            }
                            else if (prev is NonTerminal nonTerminal)
                            {
                                result.UnionWith(GetFirstTerminals(nonTerminal, grammar));
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
                foreach (var grammarItem in expansion)
                {
                    if (grammarItem is TerminalGrammar terminal)
                    {
                        sequence.Add(terminal.name);
                    }
                    else if (grammarItem is NonTerminal nonTerminal)
                    {
                        sequence.AddRange(GetFirstTerminals(nonTerminal, grammar));
                    }
                }
                if (sequence.Count > 0)
                    result.Add(sequence);
            }

            return result;
        }

        private List<List<QuestNode>> BuildAllPathsThroughNode(QuestNode node)
        {
            var roots = ExpandToRoots(node);
            var branches = ExpandToBranches(node);
            var paths = new List<List<QuestNode>>();

            foreach (var root in roots)
            {
                foreach (var branch in branches)
                {
                    var path = new List<QuestNode>(root);
                    path.RemoveAt(path.Count - 1);
                    path.AddRange(branch);
                    paths.Add(path);
                }
            }

            return paths;
        }

        private List<List<QuestNode>> BuildAllPathsThroughEdge(QuestEdge edge)
        {
            var roots = ExpandToRoots(edge.From);
            var branches = ExpandToBranches(edge.To);
            var paths = new List<List<QuestNode>>();

            foreach (var root in roots)
            {
                foreach (var branch in branches)
                {
                    var path = new List<QuestNode>(root);
                    path.AddRange(branch);
                    paths.Add(path);
                }
            }

            return paths;
        }

        private List<List<QuestNode>> ExpandToRoots(QuestNode node)
        {
            var rootLines = new List<List<QuestNode>> { new() { node } };
            var expanding = true;

            while (expanding)
            {
                expanding = false;
                var newLines = new List<List<QuestNode>>();

                foreach (var line in rootLines.ToList())
                {
                    var roots = Quest.GetRoots(line[0]);
                    foreach (var edge in roots)
                    {
                        var prev = edge.From;
                        if (prev != null && !line.Contains(prev))
                        {
                            var newLine = new List<QuestNode>(line);
                            newLine.Insert(0, prev);
                            newLines.Add(newLine);
                            expanding = true;
                        }
                    }
                }

                rootLines.AddRange(newLines);
            }

            return rootLines;
        }

        private List<List<QuestNode>> ExpandToBranches(QuestNode node)
        {
            var branchLines = new List<List<QuestNode>> { new() { node } };
            var expanding = true;

            while (expanding)
            {
                expanding = false;
                var newLines = new List<List<QuestNode>>();

                foreach (var line in branchLines.ToList())
                {
                    var branches = Quest.GetBranches(line[^1]);
                    foreach (var edge in branches)
                    {
                        var next = edge.To;
                        if (next != null && !line.Contains(next))
                        {
                            var newLine = new List<QuestNode>(line);
                            newLine.Add(next);
                            newLines.Add(newLine);
                            expanding = true;
                        }
                    }
                }

                branchLines.AddRange(newLines);
            }

            return branchLines;
        }

        public override void OnAttachLayer(LBSLayer layer)
        {
            base.OnAttachLayer(layer);
            Quest.OnAddNode += ValidateNodeGrammar;
            Quest.OnAddEdge += ValidateEdgeGrammar;
            Quest.OnRemoveNode += ValidateNodeGrammar;
            Quest.OnRemoveEdge += ValidateEdgeGrammar;
        }

        public override void OnGUI() { }
    }
}