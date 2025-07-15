using System;
using System.Collections.Generic;
using System.Linq;
using ISILab.LBS.Components;
using UnityEngine;
using UnityEngine.Serialization;

namespace ISILab.AI.Grammar
{
    
    [CreateAssetMenu(menuName = "ISILab/LBS/LBSGrammar")]
    [Serializable]
    public class LBSGrammar : ScriptableObject
    {
        [SerializeField] 
        private List<RuleEntry> rules = new();
        public List<RuleEntry> Rules => rules;

        private Dictionary<string, HashSet<string>> _ruleDict;

        [SerializeField] 
        private List<string> terminalActions = new();

        public List<string> TerminalActions => terminalActions;
        
        /// <summary>
        /// Returns all actions (rules) defined in the grammar.
        /// </summary>
        public List<string> GetAllActions()
        {
            return RuleDict.Keys.ToList();
        }


        /// <summary>
        /// Checks if a token is a reference to another rule.
        /// </summary>
        private bool IsRuleReference(string token)
        {
            return rules.Any(r => r.action.Equals(token, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Returns a cached dictionary mapping rules to possible expansions.
        /// </summary>
        public Dictionary<string, HashSet<string>> RuleDict => _ruleDict ??= GetRules();

        /// <summary>
        /// Converts the internal rule list into a dictionary of rule -> set of expansions.
        /// </summary>
        public Dictionary<string, HashSet<string>> GetRules()
        {
            return rules.ToDictionary(
                entry => entry.action,
                entry => new HashSet<string>(entry.expansions)
            );
        }

        /// <summary>
        /// Replaces the rule list with new values and resets the cache.
        /// </summary>
        public void SetRules(Dictionary<string, HashSet<string>> newRules)
        {
            rules.Clear();
            foreach (var pair in newRules)
            {
                rules.Add(new RuleEntry
                {
                    action = pair.Key,
                    expansions = new List<string>(pair.Value)
                });
            }

            _ruleDict = null;
        }

        [Serializable]
        public class RuleEntry
        {
            [FormerlySerializedAs("Action")] [SerializeField]
            public string action;
            [FormerlySerializedAs("Expansions")] [SerializeField]
            public List<string> expansions;
        }

        /// <summary>
        /// Validates whether the given list of quest nodes follows the grammar rules.
        /// </summary>
        public Tuple<bool, List<QuestNode>> Validate(List<QuestNode> questNodes)
        {
            var dict = RuleDict;
            for (int i = 0; i < questNodes.Count - 1; i++)
            {
                var current = questNodes[i].QuestAction;
                var next = questNodes[i + 1].QuestAction;

                if (!dict.TryGetValue(current, out var expansions))
                    return Tuple.Create(false, new List<QuestNode>());

                if (!expansions.Contains(next))
                    return Tuple.Create(false, new List<QuestNode>());
            }

            return Tuple.Create(true, new List<QuestNode>(questNodes));
        }

        public void SetTerminalActions(HashSet<string> actions)
        {
            terminalActions = actions.ToList();
        }
    }
}
