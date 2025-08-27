using System;
using System.Collections.Generic;
using System.Linq;
using ISILab.AI.Grammar;
using ISILab.Extensions;
using ISILab.LBS.Assistants;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Components;
using ISILab.LBS.Settings;
using ISILab.Macros;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;

namespace ISILab.LBS.Modules
{
    [Serializable]
    public class QuestGraph : LBSModule, ICloneable, ISelectable
    {
        #region FIELDS
        [SerializeField, SerializeReference, JsonRequired]
        private List<GraphNode> graphNodes = new();

        [SerializeField, SerializeReference, JsonRequired]
        private List<QuestEdge> graphEdges = new();

        [SerializeField, SerializeReference, JsonRequired]
        private QuestNode root;
        
        private GraphNode _selectedNode;

        private float _viewNodeWidthOffset = 100f;
        private float _viewNodeHeightOffset = 100f;

        [SerializeField]
        private string grammarGuid = "63ab688b53411154db5edd0ec7171c42"; // Default grammar guid

        [JsonIgnore] private LBSGrammar _grammar;
        [JsonIgnore] private Action<GraphNode> _onNodeSelected;
        [JsonIgnore] private Action _onUpdateGraph;
        #endregion

        #region PROPERTIES
        [JsonIgnore] public QuestNode Root => root;
        [JsonIgnore] public List<GraphNode> GraphNodes => graphNodes;
        [JsonIgnore] public List<QuestEdge> GraphEdges => graphEdges;

        public GraphNode SelectedQuestNode
        {
            get => _selectedNode;
            set
            {
                _selectedNode = value;
                _onNodeSelected?.Invoke(_selectedNode);
            }
        }

        public LBSGrammar Grammar
        {
            get => _grammar;
            set
            {
                _grammar = value;
                if (_grammar == null) return;

                grammarGuid = LBSAssetMacro.GetGuidFromAsset(value);
                ValidateAllWithGrammar();
            }
        }

        public event Action<GraphNode> OnQuestNodeSelected
        {
            add => _onNodeSelected += value;
            remove => _onNodeSelected = null;
        }

        public event Action RedrawGraph
        {
            add => _onUpdateGraph += value;
            remove => _onUpdateGraph = null;
        }
        #endregion

        #region EVENTS
        [JsonIgnore] public Action<GraphNode> GoToNode;
        [JsonIgnore] public Action<GraphNode> OnAddNode;
        [JsonIgnore] public Action<GraphNode> OnRemoveNode;
        [JsonIgnore] public Action<QuestEdge> OnAddEdge;
        [JsonIgnore] public Action<QuestEdge> OnRemoveEdge;
        #endregion

        #region CONSTRUCTOR
        public QuestGraph()
        {
            // changing one edge can change the values of all the graph so we recheck all the graph for
            OnAddEdge += edge =>  ValidateGraph();
            OnRemoveEdge += edge =>  ValidateGraph();
        }
        #endregion
        
    #region METHODS
        
        #region Grammar
        public void LoadGrammar()
        {
            if (_grammar == null)
                _grammar = LBSAssetMacro.LoadAssetByGuid<LBSGrammar>(grammarGuid);
        }

        public void ValidateAllWithGrammar()
        {
            var assistant = OwnerLayer.GetAssistant<GrammarAssistant>();
            if (assistant == null) throw new Exception("No GrammarAssistant found");

            foreach (var edge in GraphEdges)
                assistant.ValidateEdgeGrammar(edge);
            
            _onUpdateGraph?.Invoke();
        }
        
        private void ValidateEdgeConnection(QuestEdge newEdge)
        {
            //  Update quest node types (Goal or Middle) by their connections
            foreach (var innerEdge in GraphEdges)
            {
                if (innerEdge.To is QuestNode qn)
                {
                    qn.NodeType = GetBranches(qn).Any()
                        ? QuestNode.ENodeType.Middle
                        : QuestNode.ENodeType.Goal;
                }
            }

            // reset all connections validations
            foreach (var node in GraphNodes)
            {
                node.ValidConnections = false;
            }

            // we must revalidate all edges connections
            foreach (var edge in GraphEdges)
            {
                // destination node validation
                var dest = edge.To;
                dest.ValidConnections = GetRoots(dest).Any();

                // source nodes validation
                foreach (var node in edge.From)
                {
                    node.ValidConnections = GetRoots(node).Any();
                }
            
                if (dest is QuestNode { NodeType: QuestNode.ENodeType.Goal } goalNode)
                {
                    // the goal must not have branches!
                    goalNode.ValidConnections = !GetBranches(goalNode).Any();
                }
            }
            
            RootValidation();
        }

        void ValidateGraph()
        {
            // first validate that the connections are valid
            foreach (var edge in GraphEdges)
            {
                ValidateEdgeConnection(edge);
            }
            
            // validate grammar
            ValidateAllWithGrammar();
           
        }

        #endregion

        #region Nodes
        public void NodeDataChanged(QuestNode node) => _onNodeSelected?.Invoke(node);

        public T GetNodeAtPosition<T>(Vector2 pos) where T : GraphNode
        {
            foreach (var node in graphNodes)
            {
                if (node.NodeViewPosition.Contains(pos) && node is T casted)
                    return casted;
            }
            return null;
        }

        public List<QuestNode> GetQuestNodes() =>
            graphNodes.OfType<QuestNode>().ToList();

        public GraphNode AddNewNode(QuestBehaviour behaviour, Vector2 pos)
        {
            if (behaviour.activeGraphNodeType == typeof(QuestNode))
                return AddNewQuestNode(behaviour.ActionToSet, pos);

            if (behaviour.activeGraphNodeType == typeof(OrNode))
            {
                var node = new OrNode(pos, this);
                AddNodeToGraph(node);
                return node;
            }

            if (behaviour.activeGraphNodeType == typeof(AndNode))
            {
                var node = new AndNode(pos, this);
                AddNodeToGraph(node);
                return node;
            }

            return null;
        }

        public QuestNode AddNewQuestNode(string action, Vector2 pos)
        {
            int suffix = 0;
            string id;
            do { id = $"{action} ({suffix++})"; }
            while (GetQuestNodes().Any(n => n.ID == id));

            var node = new QuestNode(id, pos, action, this);
            AddNodeToGraph(node);
            return node;
        }

        public void AddNodeToGraph(GraphNode node)
        {
            graphNodes.Add(node);
            OnAddNode?.Invoke(node);

            if (node is QuestNode qn)
            {
                if (root == null) SetRoot(qn);
                _selectedNode = qn;
                NodeDataChanged(qn);
            }
        }

        public void RemoveQuestNode(GraphNode node)
        {
            foreach (var e in GetEdgesWithNode(node))
                RemoveEdge(e);

            graphNodes.Remove(node);
            OnRemoveNode?.Invoke(node);

            if (node == root) root = null;
            if (node == _selectedNode) _selectedNode = null;

            NodeDataChanged(_selectedNode as QuestNode);
        }
        #endregion

        #region Edges
        public Tuple<string, LogType> AddEdge(GraphNode from, GraphNode to)
        {
            if (to == null || from == null)
                return Tuple.Create("A connection requires two nodes.", LogType.Error);

            if (from == to)
                return Tuple.Create("A node cannot connect to itself.", LogType.Error);

            // prevent duplicates
            if (graphEdges.Any(e => e.From.Contains(from) && e.To == to))
                return Tuple.Create("This connection already exists.", LogType.Error);

            // check for looping connections
            if (IsLooped(from, to, new HashSet<GraphNode>()))
            {
                return Tuple.Create("The destination is a root of this node.", LogType.Error);
            }
            // only branching nodes can be a To on multiple edges
            if (to is QuestNode && from is QuestNode)
            {
                bool alreadyTarget = graphEdges.Any(e => e.To == to);
                if (alreadyTarget)
                    return Tuple.Create("Action Nodes can only be the destination of one edge. For multiple use Branching nodes", LogType.Error);
            }
            
            var newEdge = new QuestEdge(from, to);
            graphEdges.Add(newEdge);
            OnAddEdge?.Invoke(newEdge);

            return Tuple.Create($"Connection: {from} → {to}", LogType.Log);
        }

        private bool IsLooped(GraphNode origin, GraphNode current, HashSet<GraphNode> visited)
        {
            if (origin == current)
                return true;

            if (!visited.Add(current))
                return false;

            // Traverse *forward only* (branches)
            foreach (var branch in GetBranches(current))
            {
                if (IsLooped(origin, branch.To, visited))
                    return true;
            }
            
            /* Code for single direction 
            if (origin == current)
                return true;

            if (!visited.Add(current)) // returns false if already in visited
                return false;

            // Check roots
            foreach (var rootEdge in GetRoots(current))
            {
                foreach (var fromNode in rootEdge.From)
                {
                    if (IsLooped(origin, fromNode, visited))
                        return true;
                }
            }

            // Check branches
            foreach (var branch in GetBranches(current))
            {
                if (IsLooped(origin, branch.To, visited))
                    return true;
            }
*/
            return false;
        }


        private void RemoveEdge(QuestEdge edge)
        {
            if (edge == null) return;
            graphEdges.Remove(edge);
            OnRemoveEdge?.Invoke(edge);
        }

        private QuestEdge GetEdge(Vector2 pos, float delta)
        {
            var size = OwnerLayer.TileSize * LBSSettings.Instance.general.TileSize;
            foreach (var e in graphEdges)
            {
                foreach (var from in e.From)
                {
                    var c1 = new Rect(from.NodeViewPosition).center;
                    var c2 = new Rect(e.To.NodeViewPosition).center;
                    if (pos.DistanceToLine(c1, c2) < delta)
                        return e;
                }
            }
            return null;
        }

        public void RemoveEdgeByPosition(Vector2Int pos, float delta)
        {
            var edge = GetEdge(pos, delta);
            RemoveEdge(edge);
        }

        private List<QuestEdge> GetEdgesWithNode(GraphNode node) =>
            graphEdges.Where(e => e.From.Contains(node) || e.To.Equals(node)).ToList();

        public List<QuestEdge> GetBranches(GraphNode node)
        {
            var list = new List<QuestEdge>();
            foreach (var edge in graphEdges)
                if (edge.From.Contains(node))
                    list.Add(edge);
            return list;
        }

        public List<QuestEdge> GetRoots(GraphNode node) =>
            graphEdges.Where(e => e.To == node).ToList();
        #endregion

        
        #region AssistantCalls
        
         /// <summary>
        /// finds the edge of a referenced node. makes a new action that turns into the "To"
        /// of the connection and makes a new edge from the new action and the original "To"
        /// of the referenced node
        /// </summary>
        /// <param name="action">The action type for the new node</param>
        /// <param name="referenceNode">The node after which the new node will be inserted</param>
        public QuestNode InsertQuestNodeAfter(string action, QuestNode referenceNode)
        {
            if (referenceNode == null || !graphNodes.Contains(referenceNode))
            {
                Debug.LogWarning("Reference node is null or not in the graph. Adding as regular node.");
                return AddNewQuestNode(action, Vector2.zero);
            }

            // Position new node next to reference
            var position = referenceNode.NodeViewPosition.position;
            position.x += (int)_viewNodeWidthOffset;

            var newNode = AddNewQuestNode(action, position);

            // Move all outgoing edges of reference so they start at new node
            foreach (var edge in GetBranches(referenceNode).ToList())
            {
                RemoveEdge(edge);
                AddEdge(newNode, edge.To);
            }
            
            // Add edge from reference → new node
            AddEdge(referenceNode, newNode);
            _onUpdateGraph?.Invoke();

            return newNode;
        }



        /// <summary>
        /// Inserts a new node before a specified reference node
        /// </summary>
        /// <param name="action">The action type for the new node</param>
        /// <param name="referenceNode">The node before which the new node will be inserted</param>
        public QuestNode InsertNodeBefore(string action, QuestNode referenceNode)
        {
            if (referenceNode == null || !graphNodes.Contains(referenceNode))
            {
                Debug.LogWarning("Reference node is null or not in the graph. Adding as regular node.");
                return AddNewQuestNode(action, Vector2.zero);
            }

            // Position new node next to reference
            var position = referenceNode.NodeViewPosition.position;
            position.x -= (int)_viewNodeWidthOffset;

            var newNode = AddNewQuestNode(action, position);

            // Move all incoming edges of reference so they start at new node
            foreach (var edge in GetRoots(referenceNode).ToList())
            {
                RemoveEdge(edge);
                foreach (var from in edge.From)
                {
                    AddEdge(from, newNode);
                }          
     
            }
            
            // Add edge from new node →reference
            AddEdge(newNode, referenceNode);
            _onUpdateGraph?.Invoke();

            return newNode;
        }
        
        /// <summary>
        /// Inserts all the nodes to replace the reference node
        /// </summary>
        /// <param name="expandActions">all the actions that correspond to a new node</param>
        /// <param name="referenceNode">the node that will be expanded(replaced)</param>
        public void ExpandNode(List<string> expandActions, QuestNode referenceNode)
        {
            if(!expandActions.Any()) return;
            
            List<QuestNode> newNodes = new List<QuestNode>();
            QuestNode iterationNode = referenceNode;
            
            // cant' redo connections with a root already in use
            if(referenceNode == Root) SetRoot(null);
            
            // add from the previous index position to add the new ones
            foreach (var action in expandActions)
            {
                var newNode = InsertQuestNodeAfter(action, iterationNode);
                iterationNode = newNode;
            }
        
            
            RemoveQuestNode(referenceNode);
        }
        
        #endregion
        
        #region Root
        public void SetRoot(QuestNode node)
        {
            if (root != null)
            {
                root.NodeType = QuestNode.ENodeType.Middle;
            }
            
            root = node;
            // set a null root
            if (root == null) return;
            
            root.NodeType = QuestNode.ENodeType.Start;
            
            RootValidation();
            
            ValidateAllWithGrammar();
        }

        private void RootValidation()
        {
            root.ValidConnections = !GetRoots(root).Any() && GetBranches(root).Any();
        }

        #endregion

        #region Clone & Utils

        public BaseQuestNodeData GetNodeData()
        {
            QuestNode node = SelectedQuestNode as QuestNode;
            return node?.NodeData;
        }
        
        public QuestNode GetNodeAsQuest()
        {
            return SelectedQuestNode as QuestNode;
        }
        
        public override bool IsEmpty() => graphNodes.Count == 0;

        public override object Clone()
        {
            var clone = new QuestGraph { grammarGuid = grammarGuid };

            var nodes = graphNodes.Select(CloneRefs.Get).Cast<GraphNode>();
            foreach (var n in nodes)
            {
                clone.graphNodes.Add(n);
                n.Graph = clone;
            }

            var edges = graphEdges.Select(CloneRefs.Get).Cast<QuestEdge>();
            foreach (var e in edges) clone.graphEdges.Add(e);

            clone.root = CloneRefs.Get(Root) as QuestNode;
            return clone;
        }

        public List<object> GetSelected(Vector2Int pos)
        {
            var list = new List<object>();
            var node = GetGraphNode(pos);
            if (node != null) list.Add(node);
            return list;
        }

        private GraphNode GetGraphNode(Vector2Int pos) =>
            graphNodes.FirstOrDefault(n => n.Position == pos);
        #endregion

        #region Unused
        public void ChangeConnection(QuestEdge edge, Type graphNodeType) =>
            throw new NotImplementedException();

        public override void Print() => throw new NotImplementedException();
        public override void Clear() => throw new NotImplementedException();
        public override Rect GetBounds() => throw new NotImplementedException();
        public override void Rewrite(LBSModule other) => throw new NotImplementedException();
        #endregion
        
        #endregion
        
    }
}
