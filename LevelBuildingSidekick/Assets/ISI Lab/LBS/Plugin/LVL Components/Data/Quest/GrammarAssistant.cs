using ISILab.LBS.Components;
using ISILab.LBS.Modules;
using LBS.Components;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

namespace ISILab.LBS.Assistants
{
    [System.Serializable]
    [RequieredModule(typeof(QuestGraph))]
    public class GrammarAssistant : LBSAssistant
    {
        [JsonIgnore]
        public QuestGraph Quest => Owner.GetModule<QuestGraph>();

        public GrammarAssistant(Texture2D icon, string name) : base(icon, name) { }

        public void ValidateNodeGrammar(QuestNode node)
        {
            var grammar = Quest.Grammar;

            var root = Quest.Root;

            var roots = RootLines(node);
            var branches = BranchLines(node);

            var questLines = new List<List<QuestNode>>();

            foreach (var r in roots)
            {
                foreach (var n in r)
                {
                    n.GrammarCheck = false;
                }
            }

            foreach (var b in branches)
            {
                foreach (var n in b)
                {
                    n.GrammarCheck = false;
                }
            }

            var validRoots = new List<List<QuestNode>>();
            foreach (var r in roots)
            {
                if (r[0].Equals(Quest.Root))
                {
                    validRoots.Add(r);
                }
            }

            foreach (var r in validRoots)
            {
                foreach (var b in branches)
                {
                    var questLine = new List<QuestNode>();

                    questLine.AddRange(r);
                    questLine.RemoveAt(questLine.Count - 1); // last element of rootLine is same as first of branchLine which is the node;
                    questLine.AddRange(b);
                    questLines.Add(questLine);
                }
            }

            var candidates = new List<List<QuestNode>>();

            foreach (var q in questLines)
            {
                //Check validity of each list
                if (q == null || q.Count == 0) // => ? should not happen
                {
                    Debug.LogError("Validating QuestNode");
                    continue;
                }

                var actions = q.Select(n => n.QuestAction).ToList();

                if (!Quest.Grammar.Validate(actions))
                {
                    continue;
                }

                candidates.Add(q);
            }

            foreach (var c in candidates)
            {
                foreach (var n in c)
                {
                    n.GrammarCheck = true;
                }
            }

        }

        public void ValidateEdgeGrammar(QuestEdge edge)
        {

            var grammar = Quest.Grammar;

            var root = Quest.Root;

            var first = edge.First;
            var second = edge.Second;

            var roots = RootLines(first);
            var branches = BranchLines(second);

            foreach (var r in roots)
            {
                foreach (var n in r)
                {
                    n.GrammarCheck = false;
                }
            }

            foreach (var b in branches)
            {
                foreach (var n in b)
                {
                    n.GrammarCheck = false;
                }
            }


            var validRoots = new List<List<QuestNode>>();
            foreach (var r in roots)
            {
                if (r[0].Equals(Quest.Root))
                {
                    validRoots.Add(r);
                }
            }

            var questLines = new List<List<QuestNode>>();

            foreach (var r in validRoots)
            {
                foreach (var b in branches)
                {
                    var questLine = new List<QuestNode>();

                    questLine.AddRange(r);
                    questLine.AddRange(b);
                    questLines.Add(questLine);
                }
            }


            var candidates = new List<List<QuestNode>>();

            foreach (var q in questLines)
            {
                //Check validity of each list
                if (q == null || q.Count == 0)
                {
                    Debug.LogError("Validating QuestNode");
                    continue;
                }

                var actions = q.Select(n => n.QuestAction).ToList();

                if (!Quest.Grammar.Validate(actions))
                {
                    continue;
                }

                candidates.Add(q);
            }


            foreach (var c in candidates)
            {
                foreach (var n in c)
                {
                    n.GrammarCheck = true;
                }
            }
        }

        private List<List<QuestNode>> RootLines(QuestNode node)
        {
            List<List<QuestNode>> rootLines = new List<List<QuestNode>>();

            var first = new List<QuestNode>();
            first.Add(node);

            rootLines.Add(first);

            var expanding = true;

            while (expanding)
            {
                expanding = false;

                List<List<QuestNode>> newLines = new List<List<QuestNode>>();

                for (int i = 0; i < rootLines.Count; i++)
                {
                    var line = rootLines[i];

                    if (line[0].Equals(Quest.Root))
                        continue;

                    var roots = Quest.GetRoots(line[0]);

                    if (roots.Count == 0)
                    {
                        continue;
                    }

                    expanding = true;

                    line.Insert(0, roots[0].First);

                    for (int j = 1; j < roots.Count; j++)
                    {
                        var newLine = new List<QuestNode>(line);
                        newLine.Insert(0, roots[i].First);
                        newLines.Add(newLine);
                    }
                }
            }

            return rootLines;

        }

        private List<List<QuestNode>> BranchLines(QuestNode node)
        {
            List<List<QuestNode>> branchLines = new List<List<QuestNode>>();

            var first = new List<QuestNode>();
            first.Add(node);

            branchLines.Add(first);

            var expanding = true;

            while (expanding)
            {
                expanding = false;

                List<List<QuestNode>> newLines = new List<List<QuestNode>>();

                for (int i = 0; i < branchLines.Count; i++)
                {
                    var line = branchLines[i];

                    var branches = Quest.GetBranches(line[0]);

                    if (branches.Count == 0)
                    {
                        continue;
                    }

                    expanding = true;

                    line.Add(branches[0].Second);

                    for (int j = 1; j < branches.Count; j++)
                    {
                        var newLine = new List<QuestNode>(line);
                        newLine.Add(branches[i].Second);
                        newLines.Add(newLine);
                    }
                }
            }

            return branchLines;

        }

        public List<string> GetSuggestions(QuestNode node)
        {
            var suggestions = new List<string>();



            return suggestions;
        }

        public void ValidateMap()
        {

        }

        public override object Clone()
        {
            throw new NotImplementedException(); // TODO: Implement this method for GrammarAssistant class
        }

        public void CheckNode()
        {


        }

        public override void OnAttachLayer(LBSLayer layer)
        {
            base.OnAttachLayer(layer);
            Quest.OnAddNode += ValidateNodeGrammar;
            Quest.OnAddEdge += ValidateEdgeGrammar;

        }
    }
}
