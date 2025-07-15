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
        /// Validates a node by constructing all full paths through it and checking if each path is valid.
        /// </summary>
        public void ValidateNodeGrammar(QuestNode node)
        {
            var grammar = Quest.Grammar;
            if (grammar == null || grammar.Rules.Count == 0) return;

            var roots = RootLines(node);
            var branches = BranchLines(node);
            var questLines = new List<List<QuestNode>>();

            foreach (var r in roots)
            {
                foreach (var b in branches)
                {
                    var line = new List<QuestNode>();
                    line.AddRange(r);
                    line.RemoveAt(line.Count - 1); // Avoid duplication
                    line.AddRange(b);
                    questLines.Add(line);
                }
            }

            foreach (var n in roots.SelectMany(r => r).Concat(branches.SelectMany(b => b)))
            {
                n.ValidGrammar = false;
            }

            foreach (var line in questLines)
            {
                if (IsValidSequence(line.Select(n => n.QuestAction).ToList(), grammar))
                {
                    foreach (var n in line)
                        n.ValidGrammar = true;
                }
            }
        }

        /// <summary>
        /// Validates an edge by testing all paths that flow through it.
        /// </summary>
        public void ValidateEdgeGrammar(QuestEdge edge)
        {
            if (edge == null) return;
            var grammar = Quest.Grammar;
            if (grammar == null || grammar.Rules.Count == 0) return;

            var roots = RootLines(edge.First);
            var branches = BranchLines(edge.Second);
            var questLines = new List<List<QuestNode>>();

            foreach (var r in roots)
            {
                foreach (var b in branches)
                {
                    var line = new List<QuestNode>();
                    line.AddRange(r);
                    line.AddRange(b);
                    questLines.Add(line);
                }
            }

            foreach (var n in roots.SelectMany(r => r).Concat(branches.SelectMany(b => b)))
            {
                n.ValidGrammar = false;
            }

            foreach (var line in questLines)
            {
                if (IsValidSequence(line.Select(n => n.QuestAction).ToList(), grammar))
                {
                    foreach (var n in line)
                        n.ValidGrammar = true;
                }
            }
        }

        /// <summary>
        /// Efficient validator that checks action sequences linearly using the dictionary rules.
        /// </summary>
        private bool IsValidSequence(List<string> actions, LBSGrammar grammar)
        {
            for (int i = 0; i < actions.Count - 1; i++)
            {
                var currentAction = actions[i];
                var nextSet = actions[i + 1];

                if (!grammar.RuleDict.TryGetValue(currentAction, out var expansions))
                    return false;

                if (!nextSet.Contains(nextSet))
                    return false;
            }
            return true;
        }

        public bool FastValidGrammar(List<QuestNode> nodes)
        {
            return nodes.All(n => n.ValidGrammar);
        }

        public List<string> GetSuggestions(QuestNode node)
        {
            var grammar = Quest.Grammar;
            if (grammar == null || !grammar.RuleDict.TryGetValue(node.QuestAction, out var nextSet))
                return new List<string>();

            return nextSet.ToList();
        }

        private List<List<QuestNode>> RootLines(QuestNode node)
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
                        var prev = edge.First;
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

        private List<List<QuestNode>> BranchLines(QuestNode node)
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
                        var next = edge.Second;
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
