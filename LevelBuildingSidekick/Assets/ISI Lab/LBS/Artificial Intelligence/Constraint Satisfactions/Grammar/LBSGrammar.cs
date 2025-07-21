using System;
using System.Collections.Generic;
using UnityEngine;

namespace ISILab.AI.Grammar
{
    [Serializable]
    public abstract class Grammar
    {
        public string name;

        public bool IsNonTerminal() => GetTrimmed().StartsWith("#");

        public bool IsTerminal() => !IsNonTerminal();

        public string GetTrimmed() => name.Trim();
    }

    [Serializable]
    public class TerminalGrammar : Grammar
    {
    }

    [Serializable]
    public class NonTerminal : Grammar
    {
        [SerializeField] private List<List<Grammar>> _expansions = new();

        public List<List<Grammar>> Expansions
        {
            get => _expansions;
            set => _expansions = value;
        }

        public List<string> GetTerminals()
        {
            List<string> terminals = new();
            foreach (var sequence in _expansions)
            {
                foreach (var grammar in sequence)
                {
                    if (grammar is NonTerminal nonTerminal)
                    {
                        terminals.AddRange(nonTerminal.GetTerminals());
                    }
                    else if (grammar is TerminalGrammar terminal)
                    {
                        terminals.Add(terminal.name);
                    }
                }
            }
            return terminals;
        }
    }

    [Serializable]
    public class RuleEntry
    {
        public string RuleName;
        public List<List<Grammar>> Expansions = new();
    }

    [CreateAssetMenu(menuName = "ISILab/LBSGrammar")]
    public class LBSGrammar : ScriptableObject
    {
        [SerializeField] private List<string> terminalActions = new();
        [SerializeField] private List<RuleEntry> ruleEntries = new();

        public List<string> TerminalActions => terminalActions;
        public List<RuleEntry> RuleEntries => ruleEntries;

        /// <summary>
        /// Gets rules as a dictionary for easy lookup.
        /// </summary>
        public Dictionary<string, List<List<Grammar>>> GetRules()
        {
            var result = new Dictionary<string, List<List<Grammar>>>();
            foreach (var entry in ruleEntries)
            {
                result[entry.RuleName] = entry.Expansions;
            }
            return result;
        }

        /// <summary>
        /// Sets grammar structure after parsing.
        /// </summary>
        public void SetGrammarStructure(GrammarStructure structure)
        {
            terminalActions = new List<string>(structure.Terminals);
            ruleEntries.Clear();

            foreach (var kvp in structure.Rules)
            {
                var entry = new RuleEntry
                {
                    RuleName = kvp.Key,
                    Expansions = new List<List<Grammar>>()
                };

                foreach (var sequence in kvp.Value.Expansions)
                {
                    var grammarSequence = new List<Grammar>();
                    foreach (var symbol in sequence)
                    {
                        Grammar g = symbol.StartsWith("#")
                            ? new NonTerminal { name = symbol }
                            : new TerminalGrammar { name = symbol };
                        grammarSequence.Add(g);
                    }
                    entry.Expansions.Add(grammarSequence);
                }

                ruleEntries.Add(entry);
            }
        }

        /// <summary>
        /// Gets all valid next terminal actions that follow the given terminal.
        /// </summary>
        public List<string> GetAllValidNextActions(string currentAction)
        {
            var result = new HashSet<string>();
            var visited = new HashSet<string>();
            var rules = GetRules();

            foreach (var kvp in rules)
            {
                foreach (var expansion in kvp.Value)
                {
                    for (int i = 0; i < expansion.Count - 1; i++)
                    {
                        if (expansion[i].name == currentAction)
                        {
                            var next = expansion[i + 1];
                            if (next is TerminalGrammar terminal)
                            {
                                result.Add(terminal.name);
                            }
                            else if (next is NonTerminal nonTerminal)
                            {
                                string ruleName = nonTerminal.name.TrimStart('#');
                                var terminals = GetFirstTerminals(ruleName, new HashSet<string>());
                                foreach (var t in terminals)
                                    result.Add(t);
                            }
                        }
                    }
                }
            }

            return new List<string>(result);
        }

        /// <summary>
        /// Recursively collects first terminal(s) from a rule.
        /// </summary>
        private List<string> GetFirstTerminals(string ruleName, HashSet<string> visited)
        {
            var result = new List<string>();
            var rules = GetRules();

            if (!rules.ContainsKey(ruleName) || visited.Contains(ruleName))
                return result;

            visited.Add(ruleName);

            foreach (var expansion in rules[ruleName])
            {
                if (expansion.Count == 0) continue;

                var first = expansion[0];
                if (first is TerminalGrammar terminal)
                {
                    result.Add(terminal.name);
                }
                else if (first is NonTerminal nonTerminal)
                {
                    string nested = nonTerminal.name.TrimStart('#');
                    result.AddRange(GetFirstTerminals(nested, visited));
                }
            }

            return result;
        }
    }

    public class RuleData
    {
        public string RuleName;
        public List<List<string>> Expansions = new(); // List of sequences
    }

    public class GrammarStructure
    {
        public Dictionary<string, RuleData> Rules = new();
        public HashSet<string> Terminals = new();
    }
}
