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

        /// <summary>
        /// Validates grammar paths through a given node using full paths from roots and branches.
        /// </summary>
        public void ValidateNodeGrammar(QuestNode node)
        {
            var grammar = Quest.Grammar;
            if (grammar == null || grammar.Rules.Count == 0) return;

            var paths = BuildAllPathsThroughNode(node);

            // Mark all as invalid initially
            foreach (var n in paths.SelectMany(p => p))
                n.ValidGrammar = false;

            // Mark nodes part of valid paths
            foreach (var path in paths)
            {
                if (IsValidSequence(path.Select(n => n.QuestAction).ToList(), grammar))
                {
                    foreach (var n in path)
                        n.ValidGrammar = true;
                }
            }
        }

        /// <summary>
        /// Validates grammar paths through a given edge.
        /// </summary>
        public void ValidateEdgeGrammar(QuestEdge edge)
        {
            if (edge == null) return;

            var grammar = Quest.Grammar;
            if (grammar == null || grammar.Rules.Count == 0) return;

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

        /// <summary>
        /// Checks a sequence of actions for grammar correctness.
        /// </summary>
        private bool IsValidSequence(List<string> actions, LBSGrammar grammar)
        {
            for (int i = 0; i < actions.Count - 1; i++)
            {
                var current = actions[i];
                var next = actions[i + 1];

                if (!grammar.RuleDict.TryGetValue(current, out var validNext))
                    return false;

                if (!validNext.Contains(next))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Returns true if all nodes in a list are marked as valid.
        /// </summary>
        public bool FastValidGrammar(List<QuestNode> nodes)
        {
            return nodes.All(n => n.ValidGrammar);
        }

        /// <summary>
        /// Returns all valid next actions from the given node.
        /// </summary>
        public List<string> GetSuggestions(QuestNode node)
        {
            var grammar = Quest.Grammar;

            if (grammar == null)
                return new List<string>();

            if (grammar.RuleDict.TryGetValue(node.QuestAction, out var nextSet))
                return nextSet.ToList();

            // If not in ruleDict, return all terminals (i.e., valid root nodes)
            return grammar.TerminalActions.ToList();
        }

        /// <summary>
        /// Returns all valid actions from a given terminal action.
        /// </summary>
        public List<string> GetAllValidNextActions(string currentAction)
        {
            var grammar = Quest.Grammar;

            if (grammar != null && grammar.RuleDict.TryGetValue(currentAction, out var nextSet))
                return nextSet.ToList();

            return new List<string>();
        }

        public List<string> GetAllValidPrevActions(string getSelectedNode)
        {
            return new List<string>() ;
        }

        public List<List<string>> GetAllExpansions(string getSelectedNode)
        {
            return new List<List<string>>() ;
        }
        
        /// <summary>
        /// Builds all path lines that go through a node.
        /// </summary>
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
                    path.RemoveAt(path.Count - 1); // Avoid node duplication
                    path.AddRange(branch);
                    paths.Add(path);
                }
            }

            return paths;
        }

        /// <summary>
        /// Builds all path lines that go through an edge.
        /// </summary>
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

        /// <summary>
        /// Traverses upward from a node to find all root paths.
        /// </summary>
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

        /// <summary>
        /// Traverses downward from a node to find all branch paths.
        /// </summary>
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
