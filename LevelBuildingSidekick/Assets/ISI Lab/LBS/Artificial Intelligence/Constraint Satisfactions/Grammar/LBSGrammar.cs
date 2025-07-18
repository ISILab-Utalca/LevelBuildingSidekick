using System.Collections.Generic;
using UnityEngine;

namespace ISILab.AI.Grammar
{
        [CreateAssetMenu(menuName = "ISILab/LBSGrammar")]
        public class LBSGrammar : ScriptableObject
        {
                [SerializeField] private List<string> terminalActions = new();
                [SerializeField] private List<RuleEntry> ruleEntries = new();

                [System.Serializable]
                public class RuleEntry
                {
                        public string RuleName;
                        public List<string> Expansions;
                }

                private Dictionary<string, List<string>> cachedRules;

                public Dictionary<string, List<string>> GetRules()
                {
                        if (cachedRules != null) return cachedRules;

                        cachedRules = new Dictionary<string, List<string>>();
                        foreach (var entry in ruleEntries)
                        {
                                if (!cachedRules.ContainsKey(entry.RuleName))
                                {
                                        cachedRules[entry.RuleName] = new List<string>();
                                }
                                cachedRules[entry.RuleName].AddRange(entry.Expansions);
                        }

                        return cachedRules;
                }

                public List<string> TerminalActions => terminalActions;

                public void SetGrammarStructure(LBSGrammarReader.GrammarStructure structure)
                {
                        terminalActions = new List<string>(structure.Terminals);
                        ruleEntries.Clear();

                        foreach (var kvp in structure.Rules)
                        {
                                var entry = new RuleEntry
                                {
                                        RuleName = kvp.Key,
                                        Expansions = new List<string>()
                                };

                                foreach (var expansion in kvp.Value.Expansions)
                                {
                                        entry.Expansions.Add(string.Join(" ", expansion));
                                }

                                ruleEntries.Add(entry);
                        }

                        cachedRules = null;
                }
        }
}