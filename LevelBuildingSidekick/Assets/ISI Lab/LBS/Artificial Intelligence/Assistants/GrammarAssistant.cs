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
            if (grammar == null || !grammar.RuleEntries.Any()) return false;

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
            var nextValidTerminals = new HashSet<string>();

            if (grammar == null) return nextValidTerminals.ToList();

            List<string> owningRules = new List<string>();
            foreach (var rule in GetOwningRules(currentAction))
            {
                owningRules.Add(rule);
            }
            
            HashSet<RuleItem> itemsWithRule = new HashSet<RuleItem>();
            
            
            // Step 1: Check for direct next terminals in expansions
            foreach (var rule in grammar.RuleEntries)
            {
                foreach (RuleEntry ruleEntry in grammar.RuleEntries)
                {
                    foreach (RuleItem wrapper in ruleEntry.expansions)
                    {
                        foreach (var owningRule in owningRules)
                        {
                            if (wrapper.items.Contains(currentAction) ||
                                wrapper.items.Contains(owningRule))
                            {
                                itemsWithRule.Add(wrapper);
                            }
                        }
                    }
                }
            }
            
            // Get the next valid terminal
            foreach (var rule in owningRules)
            {
                foreach (var ruleItem in itemsWithRule)
                {
                    for (int i = 0; i < ruleItem.items.Count-1; i++)
                    {
                        var current = ruleItem.items[i];
                        
                        // if isrule ref get last if == to currentaction
                        if (grammar.IsRuleRef(current) && current == rule)
                        {
                            // if the rule is in the item, we must retrieve the last terminal
                            // and compare it to the currentaction
                            current = GetLastTerminals(current, grammar).First();
                        }
                        
                            
                        if(current.Equals(currentAction))
                        {
                            var next = ruleItem.items[i+1];
                            if (grammar.IsRuleRef(next))
                            {
                                // if the first next a ruleRef get the first valid terminal
                                next = GetFirstTerminals(next, grammar).First();
                            }
                            
                            // assign the current as a valid prev, because the next is the current
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
                
                allExpansions.Add(sequence);
            }

            return allExpansions.ToList();
        }

        public List<string> GetOwningRules(string currentAction)
        {
            var grammar = Quest.Grammar;
            if (grammar == null) return new List<string>();

            HashSet<string> owners = new HashSet<string>();
            
            foreach (RuleEntry ruleEntry in Quest.Grammar.RuleEntries)
            {
                foreach (var terminals in ruleEntry.expansions)
                {
                    if(terminals.items.Contains(currentAction)) owners.Add(ruleEntry.ruleID);
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


    }
}