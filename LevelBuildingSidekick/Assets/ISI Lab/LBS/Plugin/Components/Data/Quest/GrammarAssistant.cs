using System;
using System.Collections.Generic;
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

                if (!Quest.Grammar.Validate(q).Item1)
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

        /// <summary>
        /// Assuming that the validation grammar is checked by the user when setting the nodes
        /// i.e assumes all the nodes have valid grammatical connections
        /// </summary>
        /// <param name="nodes">All the nodes that the graph contains</param>
        /// <returns></returns>
        public bool fastValidGrammar(List<QuestNode> nodes)
        {
            return nodes.All(n => n.GrammarCheck);
        }
        
        public void ValidateEdgeGrammar(QuestEdge edge)
        {
            if(edge == null) return;
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

            List<List<QuestNode>> validRoots = new();
            foreach (var list in roots)
            {
                first = list.First();
                var root = Quest.Root;
                
                // Better comparison by ID or custom logic
                if (first.ID == root.ID) // assuming QuestNode has an ID field
                {
                    validRoots.Add(list);
                }
            }

            var questLines = new List<List<QuestNode>>();
            foreach (var r in validRoots.ToList())
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
                
                Tuple<bool, List<QuestNode>> result = Quest.Grammar.Validate(q);
                
                foreach (var qn in result.Item2)
                {
                    qn.GrammarCheck = true;
                }
                
                if (!result.Item1)
                {
                    Debug.LogWarning($"Invalid quest line: {string.Join(", ", q.Select(n => n.QuestAction).ToList())}");
                    continue;
                }
                
                candidates.Add(q);
            }

            foreach (var c in candidates)
            {
                foreach (var n in c)
                {
                   // n.GrammarCheck = true;
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

                if (!Quest.Grammar.Validate(q).Item1)
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
            var first = new List<QuestNode> { node };
            rootLines.Add(first);

            var expanding = true;
            int iterations = 0;
            const int MAX_ITERATIONS = 1000;
            const int MAX_PATHS = 1000;

            while (expanding && iterations++ < MAX_ITERATIONS && rootLines.Count < MAX_PATHS)
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
                        continue;

                    expanding = true;
                    var firstRoot = roots[0].First;
                    if (firstRoot != null && !line.Contains(firstRoot))
                    {
                        line.Insert(0, firstRoot);
                    }

                    for (int j = 1; j < roots.Count && rootLines.Count + newLines.Count < MAX_PATHS; j++)
                    {
                        var nextRoot = roots[j].First;
                        if (nextRoot == null || line.Contains(nextRoot))
                            continue;

                        var newLine = new List<QuestNode>(line);
                        newLine.Insert(0, nextRoot);
                        newLines.Add(newLine);
                    }
                }

                if (newLines.Count > 0)
                {
                    rootLines.AddRange(newLines);
                    expanding = true;
                }
            }

            if (iterations >= MAX_ITERATIONS)
                Debug.LogError($"RootLines exceeded {MAX_ITERATIONS} iterations for node {node.ID}");
            if (rootLines.Count >= MAX_PATHS)
                Debug.LogWarning($"RootLines capped at {MAX_PATHS} paths for node {node.ID}");

            return rootLines;
        }
        
        private List<List<QuestNode>> RootLinesOLD(QuestNode node)
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
            var first = new List<QuestNode> { node };
            branchLines.Add(first);

            var expanding = true;
            int iterations = 0;
            const int MAX_ITERATIONS = 1000; // Prevent runaway loops
            const int MAX_PATHS = 1000; // Limit total paths

            while (expanding && iterations++ < MAX_ITERATIONS && branchLines.Count < MAX_PATHS)
            {
                expanding = false;
                List<List<QuestNode>> newLines = new List<List<QuestNode>>();

                for (int i = 0; i < branchLines.Count; i++)
                {
                    var line = branchLines[i];
                    var lastNode = line[line.Count - 1]; // Fix: Use last node
                    var branches = Quest.GetBranches(lastNode);

                    if (branches.Count == 0)
                        continue;

                    expanding = true;
                    var firstBranch = branches[0].Second;
                    if (firstBranch != null && !line.Contains(firstBranch))
                    {
                        line.Add(firstBranch); // Add only if not already in path
                    }

                    for (int j = 1; j < branches.Count && branchLines.Count + newLines.Count < MAX_PATHS; j++)
                    {
                        var nextBranch = branches[j].Second;
                        if (nextBranch == null || line.Contains(nextBranch))
                            continue;

                        var newLine = new List<QuestNode>(line);
                        newLine.Add(nextBranch);
                        newLines.Add(newLine);
                    }
                }

                if (newLines.Count > 0)
                {
                    branchLines.AddRange(newLines);
                    expanding = true; // Continue if new paths were added
                }
            }

            if (iterations >= MAX_ITERATIONS)
                Debug.LogError($"BranchLines exceeded {MAX_ITERATIONS} iterations for node {node.ID}");
            if (branchLines.Count >= MAX_PATHS)
                Debug.LogWarning($"BranchLines capped at {MAX_PATHS} paths for node {node.ID}");

            return branchLines;
        }
        
        private List<List<QuestNode>> BranchLinesOLD(QuestNode node)
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
                    if (branchLines[i] == null) continue;
                    
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
                
                expanding = false;
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
            Quest.OnRemoveNode += ValidateNodeGrammar;
            Quest.OnRemoveEdge += ValidateEdgeGrammar;

        }

        public override void OnGUI()
        {
        }
    }
}
