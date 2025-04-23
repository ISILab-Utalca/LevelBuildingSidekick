using System;
using System.Collections.Generic;
using System.Linq;
using ISILab.AI.Grammar;
using ISILab.Extensions;
using ISILab.LBS.Components;
using ISILab.LBS.Internal;
using ISILab.LBS.Settings;
using ISILab.Macros;
using Newtonsoft.Json;
using UnityEngine;

namespace ISILab.LBS.Modules
{
    [System.Serializable]
    public class QuestGraph : LBSModule, ICloneable, ISelectable
    {
        [SerializeField, JsonRequired]
        private Vector2Int nodeSize;

        [SerializeField, JsonRequired]
        string grammarName;

        [SerializeField, SerializeReference, JsonRequired]
        List<QuestNode> questNodes = new();

        [SerializeField, SerializeReference, JsonRequired]
        List<QuestEdge> questEdges = new();

        [SerializeField, SerializeReference, JsonRequired]
        QuestNode root;


        [JsonIgnore]
        public Vector2Int NodeSize => nodeSize;

        [JsonIgnore]
        public QuestNode Root => root;

        [JsonIgnore]
        private LBSGrammar grammar;
        
        [SerializeField] private string LBSGrammarGui = "63ab688b53411154db5edd0ec7171c42"; // default value is DefaultGrammar
        
        [JsonIgnore]
        public LBSGrammar Grammar
        {
            get => GetQuestGrammar(); /*
            {
                if (grammar != null && grammarName != null && grammar.name == grammarName)
                    return grammar;
                else if (grammarName != null)
                {
                    grammar = LBSAssetsStorage.Instance.Get<LBSGrammar>().Find(g => g.name == grammarName);
                    return grammar;
                }
                return null;
            }*/
            set
            {
                grammar = value;
                grammarName = value.name;
                LBSGrammarGui = LBSAssetMacro.GetGuidFromAsset(value);
            }
        }

        private LBSGrammar GetQuestGrammar()
        {
            return LBSAssetMacro.LoadAssetByGuid<LBSGrammar>(LBSGrammarGui);
        }

        [JsonIgnore]
        public List<QuestNode> QuestNodes => questNodes;

        [JsonIgnore]
        public List<QuestEdge> QuestEdges => questEdges;

        [JsonIgnore]
        public bool IsVisible { get; set; }
        
        #region EVENTS
        [JsonIgnore]
        public Action<QuestNode> GoToNode;
        [JsonIgnore]
        public Action UpdateFlow;
        [JsonIgnore]
        public Action<QuestNode> OnAddNode;
        [JsonIgnore]
        public Action<QuestNode> OnRemoveNode;
        [JsonIgnore]
        public Action<QuestEdge> OnAddEdge;
        [JsonIgnore]
        public Action<QuestEdge> OnRemoveEdge;
        #endregion

        public QuestGraph()
        {
            IsVisible = true;
            nodeSize = new Vector2Int(5, 1);
        }
        
        public QuestNode GetQuestNode(Vector2 position)
        {
            var size = nodeSize * LBSSettings.Instance.general.TileSize;

            return questNodes.Find(x => (new Rect(x.Position, size)).Contains(position));
        }
        public List<QuestEdge> GetBranches(QuestNode node)
        {
            if (questEdges.Count == 0)
                return new List<QuestEdge>();
            return questEdges.Where(e => e.First.ID == node.ID).ToList();
        }
        public List<QuestEdge> GetRoots(QuestNode node)
        {
            if (questEdges.Count == 0)
                return new List<QuestEdge>();
            return questEdges.Where(e => e.Second == node).ToList();
        }
        private QuestEdge GetEdge(Vector2 position, float delta)
        {
            var size = OwnerLayer.TileSize * LBSSettings.Instance.general.TileSize;
            foreach (var e in questEdges)
            {
                var c1 = new Rect(e.First.Position, size).center;
                var c2 = new Rect(e.Second.Position, size).center;

                var dist = position.DistanceToLine(c1, c2);
                if (dist < delta)
                    return e;
            }
            return null;
        }
        
        
        public void SetRoot(QuestNode node)
        {
            
            if (node == null)
            {
                root = null;
                Debug.LogError("Root set: to NULL ");
                return;
            }
            
            root = node;
            root.NodeType = NodeType.start;
        }
        
        
        public void AddNode(string id, Vector2 position, string action)
        {
            var data = new QuestNode(id, position, action, this);
            questNodes.Add(data);
            OnAddNode?.Invoke(data);
            UpdateFlow?.Invoke();
        }
        public void AddNode(QuestNode node)
        {
            if(root == null) SetRoot(node);
            questNodes.Add(node);
            OnAddNode?.Invoke(node);
            UpdateFlow?.Invoke();
        }
        public void RemoveQuestNode(QuestNode node)
        {
            questNodes.Remove(node);
            
            var edgesToRemove = questEdges.Where(e => e.First.Equals(node) || e.Second.Equals(node)).ToList();
            foreach (var e in edgesToRemove) RemoveEdge(e);
            OnRemoveNode?.Invoke(node);
            UpdateFlow?.Invoke();
        }
        

        public Tuple<string, LogType> AddEdge(QuestNode first, QuestNode second)
        {
            if (first == null || second == null)
                return Tuple.Create("Must select two nodes", LogType.Error);

            if (first == second)
                return Tuple.Create("Cannot connect a node to itself", LogType.Error);

            if (second.Equals(root))
                return Tuple.Create("The start node cannot be the second element of a connection", LogType.Error);
            
            if (!IsValidFirst(first))
                return Tuple.Create("The first node is already connected", LogType.Error);
            
            var reverseEdge = new QuestEdge(second, first);
            var edge = new QuestEdge(first, second);
            
            if (questEdges.Contains(edge))
                return Tuple.Create("The connection already exists", LogType.Error);
            
            if (questEdges.Contains(reverseEdge))
                return Tuple.Create("The reverse connection already exists", LogType.Error);
            
            if (IsLooped(edge))
                return Tuple.Create("Invalid connection, loop detected", LogType.Error);

            questEdges.Add(edge);
            OnAddEdge?.Invoke(edge);
            UpdateFlow?.Invoke();
            
            var connectionInfo = $"Connection: {first.QuestAction} → {second.QuestAction}";
            return Tuple.Create(connectionInfo, LogType.Log);
        }
        /// <summary>
        /// Removes the connection between two nodes
        /// </summary>
        /// <param name="position">the position clicked in the graph</param>
        /// <param name="delta">higher delta easier to catch the line</param>
        public void RemoveEdge(Vector2Int position, float delta)
        {
            RemoveEdge(GetEdge(position, delta));
        }
        private void RemoveEdge(QuestEdge edge)
        {
            if (edge == null) return;
            questEdges.Remove(edge);
            OnRemoveEdge?.Invoke(edge);
            UpdateFlow?.Invoke();
        }
        
        
        private bool IsValidFirst(QuestNode node)
        {
            var found = questEdges.FirstOrDefault(e => e.First.Equals(node));
            return found == null;
        }
        private bool IsValidSecond(QuestNode node)
        {
            var found = questEdges.FirstOrDefault(e => e.Second.Equals(node));
            return found == null;
        }
        private bool IsLooped(QuestEdge edge)
        {
            if (edge == null || edge.First == null || edge.Second == null)
                return false; 
            
            var visited = new HashSet<QuestNode>(); 
            var queue = new Queue<QuestNode>();
            queue.Enqueue(edge.Second);

            int iteration = 0; // Debug limit
            const int MAX_ITERATIONS = 1000;

            while (queue.Count > 0)
            {
                if (iteration++ > MAX_ITERATIONS)
                {
                    Debug.LogError("IsLooped exceeded max iterations; possible graph corruption");
                    return true; 
                }

                var current = queue.Dequeue();

                if (ReferenceEquals(current, edge.First)) // reference check
                    return true;

                if (!visited.Add(current))
                    continue;

                var branches = GetBranches(current);
                if (branches == null) continue;

                foreach (var e in branches)
                {
                    if (e.Second != null && !visited.Contains(e.Second))
                        queue.Enqueue(e.Second);
                }
            }

            return false;
        }
        private bool Looped(QuestEdge edge)
        {
            var visited = new HashSet<QuestNode>();
            var queue = new Queue<QuestNode>();
            queue.Enqueue(edge.Second);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                if (current == edge.First)
                    return true;

                if (visited.Contains(current))
                    continue;

                visited.Add(current);

                foreach (var e in GetBranches(current))
                {
                    // avoid enqueue visited nodes
                    if (!visited.Contains(e.Second)) 
                        queue.Enqueue(e.Second);
                }
            }

            return false;
            
            /*
            var list = new List<QuestNode>();
            list.Add(edge.Second);
            while (list.Count > 0)
            {
                if (list.Contains(edge.First))
                    return true;

                var candidates = new List<QuestNode>();
                foreach (var node in list)
                {
                    var edges = GetBranches(node);
                    if (edges.Count == 0)
                        continue;
                    candidates.AddRange(edges.Select(e => e.Second));
                }

                list = candidates;
            }
            
            return false;
            */
        }
        public bool HasRequiredConnection(QuestNode questNode)
        {
            if (!questNodes.Any()) return false;
            var valid = false;
            
            bool hasNext = questEdges.Any(qe => qe.First == questNode && qe.Second != null);
            bool hasPrevious = questEdges.Any(qe => qe.Second == questNode && qe.First != null);
          
            if (questNode.NodeType == NodeType.start)
            {
                // if first node check for next connection
                valid = hasNext & !hasPrevious;
                //   Debug.Log($"start ({questNode.ID}) - edges: {valid} ");
                return valid;
            }
            
            if (questNode.NodeType == NodeType.goal)
            {
                // if last node check for previous connection
                valid = hasPrevious & !hasNext;
                //    Debug.Log($"goal ({questNode.ID}) - edges: {valid} ");
                return valid;
            }

            // mid node must have both connetions
            valid = hasNext && hasPrevious;
            
            // Debug.Log($"mid ({questNode.ID}) - edges: {valid} ");
            
            return valid;
        }
        
        public override void Print()
        {
            throw new NotImplementedException();
        }
        public override void Clear()
        {
            throw new NotImplementedException();
        }
        public override Rect GetBounds()
        {
            throw new NotImplementedException();
        }

        
        public override bool IsEmpty()
        {
            return questNodes.Count == 0;
        }
        public override object Clone()
        {
            var clone = new QuestGraph();

            clone.questNodes.Clear();

            var nodes = questNodes.Select(n => CloneRefs.Get(n)).Cast<QuestNode>();

            foreach (var node in nodes)
            {
                clone.questNodes.Add(node);
            }

            var edgesClone = questEdges.Select(e => CloneRefs.Get(e)).Cast<QuestEdge>();
            foreach (var edge in edgesClone)
            {
                clone.questEdges.Add(edge);
            }

            clone.root = CloneRefs.Get(Root) as QuestNode;

            return clone;
        }
        public List<object> GetSelected(Vector2Int position)
        {
            var selecteds = new List<object>();

            var node = GetQuestNode(position);
            if (node != null)
                selecteds.Add(node);

            return selecteds;
        }
        public override void Rewrite(LBSModule other)
        {
            throw new NotImplementedException();
        }
        
        
        /// <summary>
        /// It updates the quest node types as well as removing all the connections
        /// (edges) redoing them, as this function is called from the Quest History' list
        /// reordering
        /// </summary>
        public void UpdateQuestNodes()
        {
            if (!questNodes.Any()) return;
            
            foreach (var qn in questNodes)
            {
                qn.NodeType = NodeType.middle;
            }
            
            questNodes.Last().NodeType = NodeType.goal;
            SetRoot(questNodes.First());
        }
        public void Reorder()
        {
            questEdges.Clear(); 
            for (int i = 0; i < questNodes.Count - 1; i++)
            {
                var node1 = questNodes[i];
                var node2 = questNodes[i + 1];
                AddEdge(node1, node2);
            }
        }
        
    }

    [Serializable]
    public class QuestEdge : ICloneable
    {
        [SerializeField, SerializeReference, JsonRequired]
        QuestNode first;
        [SerializeField, SerializeReference, JsonRequired]
        QuestNode second;

        [JsonIgnore]
        public QuestNode First
        {
            get => first;
            set => first = value;
        }
        [JsonIgnore]
        public QuestNode Second
        {
            get => second;
            set => second = value;
        }

        public QuestEdge()
        {

        }

        public QuestEdge(QuestNode first, QuestNode second)
        {
            this.first = first;
            this.second = second;
        }

        public object Clone()
        {
            return new QuestEdge(CloneRefs.Get(first) as QuestNode, CloneRefs.Get(second) as QuestNode);
        }

   
    }
}
