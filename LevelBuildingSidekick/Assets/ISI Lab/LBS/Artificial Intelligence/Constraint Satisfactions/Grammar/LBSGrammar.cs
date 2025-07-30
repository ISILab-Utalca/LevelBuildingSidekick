using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ISILab.AI.Grammar
{
    [Serializable]
    public class ExpansionWrapper
    {
        [SerializeField]
        public List<string> symbols = new();
    }

    [Serializable]
    public class RuleEntry
    {
        [SerializeField]
        public string ruleName;

        [SerializeField]
        public List<ExpansionWrapper> expansions = new();
    }

    [CreateAssetMenu(menuName = "ISILab/LBSGrammar")]
    public class LBSGrammar : ScriptableObject
    {
        [SerializeField]
        private List<string> terminalActions = new();

        [SerializeField]
        private List<RuleEntry> ruleEntries = new();

        public List<string> TerminalActions => terminalActions;
        public List<RuleEntry> RuleEntries => ruleEntries;

        /// <summary>
        /// Gets rules as a dictionary for easy lookup, converting ExpansionWrapper to List<string>.
        /// </summary>
        public Dictionary<string, List<List<string>>> GetRules()
        {
            var result = new Dictionary<string, List<List<string>>>();
            foreach (var entry in ruleEntries)
            {
                if (string.IsNullOrEmpty(entry.ruleName))
                {
                    Debug.LogWarning($"[LBSGrammar] Found RuleEntry with null or empty ruleName. Skipping.");
                    continue;
                }
                if (entry.expansions == null)
                {
                    Debug.LogWarning($"[LBSGrammar] Rule '{entry.ruleName}' has null expansions. Initializing empty list.");
                    entry.expansions = new List<ExpansionWrapper>();
                }
                var expansions = new List<List<string>>();
                foreach (var wrapper in entry.expansions)
                {
                    expansions.Add(wrapper.symbols ?? new List<string>());
                }
                result[entry.ruleName] = expansions;
            }
            return result;
        }

        /// <summary>
        /// Sets grammar structure after parsing.
        /// </summary>
        public void SetGrammarStructure(GrammarStructure structure)
        {
            if (structure == null)
            {
                Debug.LogError("[LBSGrammar] GrammarStructure is null.");
                return;
            }

            Debug.Log($"[LBSGrammar] Setting grammar structure. Terminals: {structure.terminals?.Count ?? 0}, Rules: {structure.Rules?.Count ?? 0}");

            terminalActions = structure.terminals != null ? new List<string>(structure.terminals) : new List<string>();
            ruleEntries = new List<RuleEntry>();

            foreach (var kvp in structure.Rules ?? new Dictionary<string, RuleData>())
            {
                if (kvp.Value == null || string.IsNullOrEmpty(kvp.Key))
                {
                    Debug.LogWarning($"[LBSGrammar] Skipping invalid rule: Key={kvp.Key}, Value={kvp.Value}");
                    continue;
                }

                var entry = new RuleEntry
                {
                    ruleName = kvp.Key,
                    expansions = new List<ExpansionWrapper>()
                };

                foreach (var sequence in kvp.Value.Expansions ?? new List<List<string>>())
                {
                    var wrapper = new ExpansionWrapper
                    {
                        symbols = sequence != null ? new List<string>(sequence) : new List<string>()
                    };
                    entry.expansions.Add(wrapper);
                }

                ruleEntries.Add(entry);
            }

            Debug.Log($"[LBSGrammar] Populated {ruleEntries.Count} rule entries.");

            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            #endif
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
                if (!IsNonTerminal(first))
                {
                    result.Add(first);
                }
                else
                {
                    string nested = first.TrimStart('#');
                    result.AddRange(GetFirstTerminals(nested, visited));
                }
            }

            return result;
        }

        /// <summary>
        /// Checks if a symbol is a non-terminal (starts with #).
        /// </summary>
        private bool IsNonTerminal(string symbol) => !string.IsNullOrEmpty(symbol) && symbol.Trim().StartsWith("#");

        /// <summary>
        /// Debug method to inspect serialized data.
        /// </summary>
        [ContextMenu("Debug Grammar")]
        private void DebugGrammar()
        {
            Debug.Log($"[LBSGrammar] Terminal Actions Count: {terminalActions?.Count ?? 0}");
            foreach (var action in terminalActions ?? new List<string>())
            {
                Debug.Log($"[LBSGrammar] Terminal: {action}");
            }

            Debug.Log($"[LBSGrammar] Rule Entries Count: {ruleEntries?.Count ?? 0}");
            foreach (var entry in ruleEntries ?? new List<RuleEntry>())
            {
                Debug.Log($"[LBSGrammar] Rule: {entry.ruleName}, Expansions: {entry.expansions?.Count ?? 0}");
                foreach (var expansion in entry.expansions ?? new List<ExpansionWrapper>())
                {
                    Debug.Log($"[LBSGrammar]   Expansion: [{string.Join(", ", expansion.symbols ?? new List<string>())}]");
                }
            }
        }

        // Ensure initialization of serialized fields
        private void OnEnable()
        {
            if (terminalActions == null)
                terminalActions = new List<string>();
            if (ruleEntries == null)
                ruleEntries = new List<RuleEntry>();
        }
    }

    [Serializable]
    public class RuleData
    {
        [SerializeField]
        public string RuleName;

        [SerializeField]
        public List<List<string>> Expansions = new();
    }

    [Serializable]
    public class GrammarStructure
    {
        [SerializeField]
        public Dictionary<string, RuleData> Rules = new();

        [SerializeField]
        public List<string> terminals = new();
    }
}