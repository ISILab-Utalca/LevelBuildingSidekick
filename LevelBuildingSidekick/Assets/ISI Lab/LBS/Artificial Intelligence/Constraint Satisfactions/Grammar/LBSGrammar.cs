using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace ISILab.AI.Grammar
{
    /// <summary>
    /// An expansion wrapper corresponds to a rule definition like:
    /// <ruleref uri="#Goto" /> <ruleref uri="#Get" /> <ruleref uri="#Goto" /> <ruleref uri="#Subquest" /> exchange </item>
    /// </summary>
    [Serializable]
    public class RuleItem
    {
        [FormerlySerializedAs("ruleRefs")] [FormerlySerializedAs("expansionDefinitions")] [FormerlySerializedAs("symbols")] [SerializeField]
        public List<string> items = new();
    }

    [Serializable]
    public class RuleEntry
    {
        [FormerlySerializedAs("rule_id")] [FormerlySerializedAs("ruleName")] [SerializeField]
        public string ruleID;

        [SerializeField]
        public List<RuleItem> expansions = new();
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
                    ruleID = kvp.Key,
                    expansions = new List<RuleItem>()
                };

                foreach (var sequence in kvp.Value.Expansions ?? new List<List<string>>())
                {
                    var wrapper = new RuleItem
                    {
                        items = sequence != null ? new List<string>(sequence) : new List<string>()
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
        public List<string> GetFirstTerminals(string ruleName, HashSet<string> firstTerminals)
        {
            // we need to find the first terminal of each of the rules entries
            foreach (var ruleEntry in RuleEntries)
            {
                // found the rule we need
                if (ruleEntry.ruleID.Equals(ruleName))
                {
                    foreach (var entryItem in ruleEntry.expansions)
                    {
                        var terminal = entryItem.items.FirstOrDefault();
                        if (IsRuleRef(terminal))
                        {
                            // pass termina as it is a rule ref. Called recursively until a terminal is obtained
                            terminal = GetLastTerminals(terminal, firstTerminals).First();
                        }
                        firstTerminals.Add(terminal);
                    }
                }
            }

            return firstTerminals.ToList();
        }

        public List<string> GetLastTerminals(string ruleName, HashSet<string> lastTerminals)
        {
            // we need to find the first terminal of each of the rules entries
            foreach (var ruleEntry in RuleEntries)
            {
                // found the rule we need
                if (ruleEntry.ruleID.Equals(ruleName))
                {
                    foreach (var entryItem in ruleEntry.expansions)
                    {
                        var terminal = entryItem.items.LastOrDefault();
                        if (IsRuleRef(terminal))
                        {
                            // pass termina as it is a rule ref. Called recursively until a terminal is obtained
                            terminal = GetLastTerminals(terminal, lastTerminals).Last();
                        }
                        lastTerminals.Add(terminal);
                    }
                }
            }

            return lastTerminals.ToList();
        }
        
        public List<string> GetNextTerminals(string current, HashSet<string> nextTerminals)
        {
            // we need to find the first terminal of each of the rules entries
            foreach (var ruleEntry in RuleEntries)
            {
                // found the rule we need
                if (ruleEntry.ruleID.Equals(current))
                {
                    // we try to get the next value in the expansion
                    var ruleItem = ruleEntry.expansions.ElementAt(0);
                    if(ruleItem.items.Count < 2) continue;
                    
                    var terminal = ruleItem.items.ElementAt(1);
                    if (IsRuleRef(terminal))
                    {
                        // pass termina as it is a rule ref. Called recursively until a terminal is obtained
                        terminal = GetFirstTerminals(terminal, nextTerminals).First();
                    }
                    nextTerminals.Add(terminal);
                }
            }

            return nextTerminals.ToList();
        }
        
        
        /// <summary>
        /// Checks if a symbol is a non-terminal (starts with #).
        /// </summary>
        public bool IsRuleRef(string symbol) => char.IsUpper(symbol.TrimStart()[0]);
        public bool IsTerminal(string symbol) => !IsRuleRef(symbol);

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
                Debug.Log($"[LBSGrammar] Rule: {entry.ruleID}, Expansions: {entry.expansions?.Count ?? 0}");
                foreach (var expansion in entry.expansions ?? new List<RuleItem>())
                {
                    Debug.Log($"[LBSGrammar]   Expansion: [{string.Join(", ", expansion.items ?? new List<string>())}]");
                }
            }
        }

        // Ensure initialization of serialized fields
        private void OnEnable()
        {
            terminalActions ??= new List<string>();
            ruleEntries ??= new List<RuleEntry>();
        }

    
    }

    [Serializable]
    public class RuleData
    {
        [FormerlySerializedAs("RuleName")] [SerializeField]
        public string ruleName;

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