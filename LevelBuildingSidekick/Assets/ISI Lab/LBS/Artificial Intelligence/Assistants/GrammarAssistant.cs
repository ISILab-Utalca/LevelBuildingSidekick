using System;
using System.Collections.Generic;
using System.Linq;
using ISILab.LBS.Components;
using ISILab.LBS.Modules;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UIElements;
using ISILab.AI.Grammar;
using ISILab.Extensions;
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
            if (grammar == null || !grammar.RuleEntries.Any()) return false;

            bool returnValid = false;

            foreach (var from in edge.From)
            {
                // validate start 
                if (from.NodeType == NodeType.Start)
                {
                    List<string> validNextTerminals = GetAllValidNextActions(from.QuestAction);
                    bool validGrammar = validNextTerminals.Contains(edge.To.QuestAction);
                    from.ValidGrammar = validGrammar;
                    returnValid = validGrammar;
                }
            
                // validate middle
                if (from.NodeType == NodeType.Middle)
                {
                    bool validGrammar = false;
                    bool hasPreviousConnection = false;
                
                    // check that the next terminal is valid
                    List<string> validNextTerminals = GetAllValidNextActions(from.QuestAction);
                    validGrammar = validNextTerminals.Contains(edge.To.QuestAction);
                
                    // also check that this has a previous node connection
                    foreach (var fromEdge in Quest.QuestEdges)
                    {
                        // the from node has a previous connection
                        if (fromEdge.To == from)
                        {
                            hasPreviousConnection = true;
                            break;
                        }
                    }
                
                    from.ValidGrammar = validGrammar && hasPreviousConnection;
                    returnValid = from.ValidGrammar;
                }
            
                // validate goal
                if (edge.To.NodeType == NodeType.Goal)
                {
                    // if the from is valid(so is the goal). Because the "From" gets validated first
                    // by checking that the "To" is a valid terminal
                    edge.To.ValidGrammar = from.ValidGrammar;
                    returnValid = edge.To.ValidGrammar;
                }
            }
           
                
            return returnValid;
            
        }

        public List<string> GetAllValidNextActions(string currentAction)
        {
            var grammar = Quest.Grammar;
            var nextValidTerminals = new HashSet<string>();

            if (grammar == null) return nextValidTerminals.ToList();

            // Step 1: Get rules that can produce currentAction
            List<string> owningRules = GetOwningRules(currentAction);
            foreach (var owningRule in owningRules.ToList())
            {
                owningRules.AddRange(GetRulesWithRule(owningRule));
            }
            owningRules.RemoveDuplicates();

            // Step 2: Collect all relevant expansions
            HashSet<RuleItem> itemsWithRule = new HashSet<RuleItem>();
            foreach (RuleEntry ruleEntry in grammar.RuleEntries)
            {
                foreach (RuleItem wrapper in ruleEntry.expansions)
                {
                    // Include expansions with currentAction or its owning rules
                    if (wrapper.items.Contains(currentAction) || owningRules.Any(rule => wrapper.items.Contains(rule)))
                    {
                        itemsWithRule.Add(wrapper);
                    }
                }
            }

            // Step 3: Find next terminals
            foreach (var ruleItem in itemsWithRule)
            {
                for (int i = 0; i < ruleItem.items.Count - 1; i++)
                {
                    var current = ruleItem.items[i];
                    bool isCurrentAction = false;

                    // Check if current item matches currentAction or can produce it
                    if (grammar.IsRuleRef(current))
                    {
                        if (GetFirstTerminals(current, grammar).Contains(currentAction))
                        {
                            isCurrentAction = true;
                        }
                    }
                    else if (current.Equals(currentAction))
                    {
                        isCurrentAction = true;
                    }

                    if (isCurrentAction)
                    {
                        var next = ruleItem.items[i + 1];
                        if (grammar.IsRuleRef(next))
                        {
                            // Add all first terminals of the next rule
                            nextValidTerminals.UnionWith(GetFirstTerminals(next, grammar));
                        }
                        else
                        {
                            nextValidTerminals.Add(next);
                        }
                    }
                }
            }

            return nextValidTerminals.ToList();
        }
        
        public List<string> GetAllValidPrevActions(string currentAction)
        {
            var grammar = Quest.Grammar;
            var prevValidTerminals = new HashSet<string>();

            if (grammar == null) return prevValidTerminals.ToList();

            var rules = grammar.RuleEntries;

            // Step 1: Check for the next as current
            foreach (var rule in rules)
            {
                foreach (RuleItem expansion in rule.expansions)
                {
                    for (int i = 0; i < expansion.items.Count - 1; i++)
                    {
                        var next = expansion.items[i+1];
                        if (grammar.IsRuleRef(next))
                        {
                            // if the next symbols a ruleRef get the first valid terminal
                            next = GetFirstTerminals(next, grammar).First();
                        }
                        // if the next symbol is the action we are searching for
                        if (next.Equals(currentAction))
                        {
                            var current = expansion.items[i];
                            if (grammar.IsRuleRef(current))
                            {
                                // if the first next a ruleRef get the first valid terminal
                                current = GetFirstTerminals(current, grammar).First();
                            }
                            
                            // assign the current as a valid prev, because the next is the current
                            prevValidTerminals.Add(current);
                        }
                    }
                }
            }

            return prevValidTerminals.ToList();
        }

        public List<List<string>> GetAllExpansions(string currentAction)
        {
            HashSet<List<string>> allExpansions = new HashSet<List<string>>();
            var grammar = Quest.Grammar;
            if (grammar == null) return allExpansions.ToList();

            var expansions = new List<List<string>>();
            // Get all the rules that contain the terminal
            foreach (var rule in GetOwningRules(currentAction))
            {
                foreach (var ruleEntry in grammar.RuleEntries)
                {
                    if (ruleEntry.ruleID.Equals(rule))
                    {
                        foreach (var wrapper in ruleEntry.expansions)
                        {
                            expansions.Add(wrapper.items);
                        }
                    }
                }
            }

            if (expansions is null or { Count: 0 }) return allExpansions.ToList();

            // Use a HashSet to track unique sequence content as strings
            HashSet<string> seenSequences = new HashSet<string>();

            // Get terminals from the quests
            foreach (var expansion in expansions)
            {
                List<string> sequence = new List<string>();

                foreach (var symbol in expansion)
                {
                    if (grammar.IsTerminal(symbol))
                    {
                        sequence.Add(symbol);
                    }
                    else
                    {
                        sequence.Add(GetFirstTerminals(symbol, grammar).First());
                    }
                }
                
                // do not add sequences that return the same action only
                if(sequence.Count == 1 && sequence[0] == currentAction) continue;
                allExpansions.Add(sequence);
            }

            return allExpansions.ToList();
        }

        private List<string> GetRulesWithRule(string rule)
        {
            var grammar = Quest.Grammar;
            if (grammar == null) return new List<string>();
            
            HashSet<string> owningRules = new HashSet<string>();
            foreach (RuleEntry ruleEntry in Quest.Grammar.RuleEntries)
            {
                foreach (RuleItem ruleItem in ruleEntry.expansions)
                {
                    // if the rule we are checking for is within the rule item
                    if (ruleItem.items.Contains(rule))
                    {
                        owningRules.Add(ruleEntry.ruleID);
                    }
                }
            }
            return owningRules.ToList();
        }
        
        public List<string> GetOwningRules(string currentAction)
        {
            var grammar = Quest.Grammar;
            if (grammar == null) return new List<string>();

            HashSet<string> owners = new HashSet<string>();
            
            foreach (RuleEntry ruleEntry in Quest.Grammar.RuleEntries)
            {
                foreach (RuleItem item in ruleEntry.expansions)
                {
                    if(item.items.Contains(currentAction)) owners.Add(ruleEntry.ruleID);
                }
            }
            
            return owners.ToList();
        }
        
        private List<string> GetFirstTerminals(string ruleName, LBSGrammar grammar)
        {
            var firstTerminals = new HashSet<string>();
            return grammar.GetFirstTerminals(ruleName, firstTerminals);
        }
        
        private List<string> GetLastTerminals(string current, LBSGrammar grammar)
        {
            var lastTerminals = new HashSet<string>();
            return grammar.GetLastTerminals(current, lastTerminals);
        }
        
        private List<string> GetNextTerminal(string current, LBSGrammar grammar)
        {
            var nextTerminals = new HashSet<string>();
            return grammar.GetNextTerminals(current, nextTerminals);
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
                foreach (var from in edge.From)
                {
                    from.ValidGrammar = false;
                }
            };
        }

        public override void OnGUI() { }


    }
}