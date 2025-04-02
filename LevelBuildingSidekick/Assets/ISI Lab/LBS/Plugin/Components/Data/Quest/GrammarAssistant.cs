using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ISILab.LBS.Components;
using ISILab.LBS.Modules;
using LBS.Components;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Assistants
{
    [System.Serializable]
    [RequieredModule(typeof(QuestGraph))]
    public class GrammarAssistant : LBSAssistant
    {
        [JsonIgnore]
        public QuestGraph Quest => OwnerLayer.GetModule<QuestGraph>();

        public GrammarAssistant(VectorImage icon, string name, Color colorTint) : base(icon, name, colorTint) { }

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
            if(edge == null) return;
            var grammar = Quest.Grammar;

            var root = Quest.Root;
            
            var first = edge.First;
            var second = edge.Second;

            var roots = RootLines(first);
            var branches = BranchLines(second);

            foreach (var n in roots.SelectMany(r => r))
            {
                n.GrammarCheck = false;
            }

            foreach (var n in branches.SelectMany(b => b))
            {
                n.GrammarCheck = false;
            }
    
            var validRoots = roots.Where(r => r[0].Equals(Quest.Root)).ToList();

            var questLines = new List<List<QuestNode>>();
            foreach (var r in validRoots)
            {
                foreach (var b in branches)
                {
                    var questLine = new List<QuestNode>(r);
                    questLine.AddRange(b);
                    questLines.Add(questLine);
                }
            }

            var candidates = new List<List<QuestNode>>();
            foreach (var q in questLines)
            {
                if (q == null || q.Count == 0)
                {
                    Debug.LogError($"Invalid quest line found. Null or empty.");
                    continue;
                }

                var actions = q.Select(n => n.QuestAction).ToList();
                if (!Quest.Grammar.Validate(actions))
                {
                    Debug.LogWarning($"Invalid quest line: {string.Join(", ", actions)}");
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
                
                Debug.Log($"GrammarCheck set to TRUE for: {string.Join(", ", c.Select(n => n.QuestAction))}");
            }
        }

        public void ValidateEdgeGrammarOLD(QuestEdge edge)
        {
            if(edge == null) return;
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

                    if(branches[0].Second == null) continue;
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
            return new GrammarAssistant(this.Icon, this.Name, this.ColorTint);
            //throw new NotImplementedException(); // TODO: Implement this method for GrammarAssistant class
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

        public override void OnGUI()
        {
        }
    }
}
