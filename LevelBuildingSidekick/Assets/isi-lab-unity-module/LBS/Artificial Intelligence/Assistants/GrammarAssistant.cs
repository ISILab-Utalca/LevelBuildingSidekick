using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using ISILab.LBS.Components;
using ISILab.LBS.Modules;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UIElements;
using ISILab.AI.Grammar;
using ISILab.Extensions;
using LBS.Components;
using Debug = UnityEngine.Debug;

namespace ISILab.LBS.Assistants
{
    [Serializable]
    [RequieredModule(typeof(QuestGraph))]
    public class GrammarAssistant : LBSAssistant
    {
        [JsonIgnore]
        public QuestGraph _questGraph => OwnerLayer.GetModule<QuestGraph>();

        public GrammarAssistant(VectorImage icon, string name, Color colorTint)
            : base(icon, name, colorTint) { }

        public override object Clone()
        {
            return new GrammarAssistant(Icon, this.Name, this.ColorTint);
        }

        public bool ValidateQuestGraph()
        {
            foreach (var node in _questGraph.GraphNodes)
            {
                if (!node.ValidGrammar) return false;
            }

            return true;
        }
        
        public bool ValidateEdgeGrammar(QuestEdge edge)
        {
            if (edge?.From is null || edge.To is null) return false;
            
            var grammar = _questGraph.Grammar;
            if (grammar == null || !grammar.RuleEntries.Any()) return false;

            bool returnValid = false;

            foreach (var nodeFrom in edge.From)
            {
                if (nodeFrom is QuestNode from)
                {
                     // validate start 
                    if (from.NodeType == QuestNode.ENodeType.Start)
                    {
                        List<string> validNextTerminals = GetAllValidNextActions(from.QuestAction);
                        bool validGrammar = validNextTerminals.Contains(edge.To.ToString());
                        from.ValidGrammar = validGrammar;
                        returnValid = validGrammar;
                    }
                
                    // validate middle
                    if (from.NodeType == QuestNode.ENodeType.Middle)
                    {
                        // check that the next terminal is valid
                        if (edge.To.GetType() == typeof(QuestNode))
                        {
                            List<string> validNextTerminals = GetAllValidNextActions(from.QuestAction);
                            var validGrammar = validNextTerminals.Contains(edge.To.ToString());
                            from.ValidGrammar = validGrammar;
                        }
                        else
                        {
                            returnValid = from.ValidGrammar;
                        }
                        
                    }
                }
                else
                {
                    // branchis are grammarly valid 
                    nodeFrom.ValidGrammar = BranchNodeRootGrammar(nodeFrom);
                }
                
                // goal is unique
                if (edge.To is QuestNode { NodeType: QuestNode.ENodeType.Goal })
                    // validate goal
                {
                    
                    // if the from is valid(so is the goal). Because the "From" gets validated first
                    // by checking that the "To" is a valid terminal
                    edge.To.ValidGrammar = nodeFrom.ValidGrammar;
                    returnValid = edge.To.ValidGrammar;
                }   
            }
                
         
            return returnValid;
            
        }

        // Tries to retrieve from a branch Node the grammar of the immediate quest node root
        private bool BranchNodeRootGrammar(GraphNode nodeFrom)
        {
            foreach (var rootEdge in _questGraph.GetRoots(nodeFrom))
            {
                foreach (var from in rootEdge.From)
                {
                    // a quest node was found as root, get its grammar value
                    if (from.GetType() == typeof(QuestNode) & !from.IsValid())
                    {
                        // atleast one of the roots is not of valid grammar
                        return false;
                    }
                    // a branching node as root, keep searching within 
                    return BranchNodeRootGrammar(from);
                }
            }
            
            //all quest node roots were valid
            return true;
        }

        public List<string> GetAllValidNextActions(string currentAction)
        {
            var grammar = _questGraph.Grammar;
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
        
        public List<string> GetAllValidNextActionsInsert(string currentAction, QuestGraph questGraph, Action<float> onProgress = null, CancellationToken token = default)
        {
            // get valid actions out of context
            var nextValidTerminals = GetAllValidNextActions(currentAction);
            
            // Simulate to only get if its valid in the new context for insert
            HashSet<string> nextValidInsert = new HashSet<string>();
            for (var index = 0; index < nextValidTerminals.Count; index++)
            {
                if(token.IsCancellationRequested) return nextValidTerminals.ToList();
                
                var nextValidTerminal = nextValidTerminals[index];
                CloneRefs.Start();
                var clone = questGraph.Clone() as QuestGraph;
                CloneRefs.End();

                onProgress?.Invoke((float)index/nextValidTerminals.Count);

                if (clone is null) break;

                if(token.IsCancellationRequested) return nextValidTerminals.ToList();
                clone.OwnerLayer = questGraph.OwnerLayer;
                var newNode = clone.InsertQuestNodeAfter(nextValidTerminal, clone.GetNodeAsQuest());
                if (newNode.ValidGrammar)
                {
                    nextValidInsert.Add(nextValidTerminal);
                }

                clone.OwnerLayer = null;
            }

            return nextValidInsert.ToList();
        }
        
        public List<string> GetAllValidPrevActions(string currentAction)
        {
            var grammar = _questGraph.Grammar;
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

        public List<string> GetAllValidPrevActionsInsert(string currentAction, QuestGraph questGraph,  Action<float> onProgress = null, CancellationToken token = default)
        {
            // Get all non context prev actions   
            var prevValidTerminals = GetAllValidPrevActions(currentAction);
            // Simulate to only get if its valid in the new context for insert
            HashSet<string> prevValidInsert = new HashSet<string>();
            for (var index = 0; index < prevValidTerminals.Count; index++)
            {
                if(token.IsCancellationRequested) return prevValidTerminals.ToList();
                
                var nextValidTerminal = prevValidTerminals[index];
                CloneRefs.Start();
                var clone = questGraph.Clone() as QuestGraph;
                CloneRefs.End();

                onProgress?.Invoke((float)index/prevValidTerminals.Count);
                
                if (clone is null) break;
                clone.OwnerLayer = questGraph.OwnerLayer;

                if(token.IsCancellationRequested) return prevValidTerminals.ToList();
                
                var newNode = clone.InsertQuestNodeBefore(nextValidTerminal, clone.GetNodeAsQuest());
                if (newNode.ValidGrammar)
                {
                    prevValidInsert.Add(nextValidTerminal);
                }

                clone.OwnerLayer = null;
            }

            return prevValidInsert.ToList();
        }
        
        public List<List<string>> GetAllExpansions(string currentAction,Action<float> onProgress = null, CancellationToken token = default)
        {
            HashSet<List<string>> allExpansions = new HashSet<List<string>>();
            var grammar = _questGraph.Grammar;
            if (grammar == null) return allExpansions.ToList();

            var expansions = new List<List<string>>();
            // Get all the rules that contain the terminal
            foreach (var rule in GetOwningRules(currentAction))
            {
                if(token.IsCancellationRequested) return allExpansions.ToList();
                foreach (var ruleEntry in grammar.RuleEntries)
                {
                    if(token.IsCancellationRequested) return allExpansions.ToList();
                    if (ruleEntry.ruleID.Equals(rule))
                    {
                        if(token.IsCancellationRequested) return allExpansions.ToList();
                        foreach (var wrapper in ruleEntry.expansions)
                        {
                            if(token.IsCancellationRequested) return allExpansions.ToList();
                            expansions.Add(wrapper.items);
                        }
                    }
                }
            }

            if (expansions is null or { Count: 0 }) return allExpansions.ToList();

            // Use a HashSet to track unique sequence content as strings
            HashSet<string> seenSequences = new HashSet<string>();

            // Get terminals from the quests
            for (var index = 0; index < expansions.Count; index++)
            {
                if(token.IsCancellationRequested) return allExpansions.ToList();
                
                var expansion = expansions[index];
                List<string> sequence = new List<string>();

                foreach (var symbol in expansion)
                {
                    if(token.IsCancellationRequested) return allExpansions.ToList();
                    
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
                if (sequence.Count == 1 && sequence[0] == currentAction) continue;
                allExpansions.Add(sequence);
                
                onProgress?.Invoke((float)index/expansions.Count);
            }

            return allExpansions.ToList();
        }

        private List<string> GetRulesWithRule(string rule)
        {
            var grammar = _questGraph.Grammar;
            if (grammar == null) return new List<string>();
            
            HashSet<string> owningRules = new HashSet<string>();
            foreach (RuleEntry ruleEntry in _questGraph.Grammar.RuleEntries)
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
            var grammar = _questGraph.Grammar;
            if (grammar == null) return new List<string>();

            HashSet<string> owners = new HashSet<string>();
            
            foreach (RuleEntry ruleEntry in _questGraph.Grammar.RuleEntries)
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
        
        public Action ExpandAction(List<string> expandAction, QuestNode referenceNode)
        {
            return () =>
            {
                var stopwatch = Stopwatch.StartNew();

                _questGraph.ExpandNode(expandAction, referenceNode);

                stopwatch.Stop();
                Debug.Log($"ExpandAction took {stopwatch.ElapsedMilliseconds} ms");
            };
        }

        public Action InsertNextAction(string action, QuestNode referenceNode)
        {
            return () =>
            {
                _questGraph.InsertQuestNodeAfter(action, referenceNode);
            };
        }

        public Action InsertPreviousAction(string action, QuestNode referenceNode)
        {
            return () =>
            {
                var stopwatch = Stopwatch.StartNew();

                _questGraph.InsertQuestNodeBefore(action, referenceNode);
                _questGraph.ValidateAllWithGrammar();

                stopwatch.Stop();
                Debug.Log($"InsertPreviousAction took {stopwatch.ElapsedMilliseconds} ms");
            };
        }
        
        public override void OnAttachLayer(LBSLayer layer)
        {
            base.OnAttachLayer(layer);
        }

        public override void OnGUI() { }


        public object ExecuteTest(bool b)
        {
            throw new NotImplementedException(); 
            
        }
    }
}