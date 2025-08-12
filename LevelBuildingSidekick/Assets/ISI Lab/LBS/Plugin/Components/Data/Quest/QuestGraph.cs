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
using TreeEditor;
using UnityEngine;

namespace ISILab.LBS.Modules
{
    [Serializable]
    public class QuestGraph : LBSModule, ICloneable, ISelectable
    {
        #region FIELDS
        [SerializeField, JsonRequired]
        private Vector2Int nodeSize = new(5, 1);

        [SerializeField, SerializeReference, JsonRequired]
        private List<GraphNode> graphNodes = new();

        [SerializeField, SerializeReference, JsonRequired]
        private List<QuestEdge> questEdges = new();

        [SerializeField, SerializeReference, JsonRequired]
        private QuestNode root;
        
        private QuestNode _selectedQuestNode;

        private float _viewNodeWidthOffset = 400f;
        private float _viewNodeHeightOffset = 100f;
        #endregion

        #region PROPERTIES
        [JsonIgnore]
        public Vector2Int NodeSize => nodeSize;

        [JsonIgnore]
        public QuestNode Root => root;

        [JsonIgnore]
        private LBSGrammar _grammar;
        
        /// <summary>
        /// // default value is DefaultGrammar
        /// </summary>
        [SerializeField] 
        private string grammarGuid = "63ab688b53411154db5edd0ec7171c42"; 
        

        [JsonIgnore]
        public LBSGrammar Grammar
        {
            get => _grammar;
            set
            {
                _grammar = value;
                if(_grammar == null) return;
                
                // Updating the GUID as this is how the object is loaded
                grammarGuid = LBSAssetMacro.GetGuidFromAsset(value);
                // if changing grammar, must validate the existing graph with the new grammar
                CheckGraphByGrammar();
            }
        }

        /// <summary>
        /// Calls the assistant to update all the nodes by the new grammar, if the graph structure is valid
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void CheckGraphByGrammar()
        {
            var assistant = OwnerLayer.GetAssistant<GrammarAssistant>();
            if (assistant == null) throw new Exception("No Behavior");
            foreach (var edge in QuestEdges)
            {
                assistant.ValidateEdgeGrammar(edge);
            }

        }

        public List<GraphNode> GraphNodes => graphNodes;
        public List<QuestEdge> QuestEdges => questEdges;

        public QuestNode SelectedQuestNode
        {
            get => _selectedQuestNode;
            set
            {
                var previous = _selectedQuestNode;
                _selectedQuestNode = value;
                _onQuestNodeSelected?.Invoke(_selectedQuestNode);
                
                // If the selection is new, new elements must be drawn
                if (previous != _selectedQuestNode)
                {
                  //  ChangeVisuals();   
                }
            }
        }
        
        private Action<QuestNode> _onQuestNodeSelected;
        public event Action<QuestNode> OnQuestNodeSelected
        {
            add =>
                // a single suscribed function at a time
                _onQuestNodeSelected += value;

            remove => _onQuestNodeSelected = null;
        }
        
        
        [JsonIgnore]
        private Action _onUpdateGraph;
        public event Action OnUpdateGraph
        {
            add =>
                // a single suscribed function at a time
                _onUpdateGraph += value;

            remove => _onUpdateGraph = null;
        }

        #endregion
        
        #region EVENTS
        [JsonIgnore]
        public Action<GraphNode> GoToNode;
        [JsonIgnore]
        public Action<GraphNode> OnAddNode;
        [JsonIgnore]
        public Action<GraphNode> OnRemoveNode;
        [JsonIgnore]
        public Action<QuestEdge> OnAddEdge;
        [JsonIgnore]
        public Action<QuestEdge> OnRemoveEdge;
        
        
        
        #endregion
        
        #region METHODS
        
        public void NodeDataChanged(QuestNode node) {_onQuestNodeSelected?.Invoke(node);}
        
        public void LoadGrammar()
        {
            if (_grammar == null) _grammar = LBSAssetMacro.LoadAssetByGuid<LBSGrammar>(grammarGuid);
        }
        
        public T GetNodeAtPosition<T>(Vector2 position) where T : GraphNode
        {
            var size = nodeSize * LBSSettings.Instance.general.TileSize;

            var node = graphNodes.Find(x => new Rect(x.Position, size).Contains(position));

            return node as T; // Returns null if the cast fails
        }

        public List<QuestEdge> GetBranches(GraphNode node)
        {
            if (questEdges.Count == 0)
                return new List<QuestEdge>();
            
            List<QuestEdge> edges = new List<QuestEdge>();
            foreach (var edge in questEdges)
            {
                foreach (var from in edge.From)
                {
                    edges.AddRange(questEdges.Where(e => from == node).ToList());
                }
            }
            
            return edges;
        }
        public List<QuestEdge> GetRoots(GraphNode node)
        {
            return questEdges.Count == 0 ? new List<QuestEdge>() : questEdges.Where(e => e.To == node).ToList();
        }
        private QuestEdge GetEdge(Vector2 position, float delta)
        {
            var size = OwnerLayer.TileSize * LBSSettings.Instance.general.TileSize;
            foreach (var e in questEdges)
            {
                foreach (var from in e.From)
                {
                    var c1 = new Rect(from.Position, size).center;
                    var c2 = new Rect(e.To.Position, size).center;

                    var dist = position.DistanceToLine(c1, c2);
                    if (dist < delta)
                        return e;
                }
                
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
            root.NodeType = QuestNode.ENodeType.Start;
        }

        /// <summary>
        ///  Returns all the quest node types within the graph nodes
        /// </summary>
        /// <returns></returns>
        public List<QuestNode> GetQuestNodes()
        {
            List<QuestNode> existingNodes = new List<QuestNode>();
            foreach (GraphNode node in graphNodes)
            {
                if(node.GetType() == typeof(QuestNode)) existingNodes.Add(node as QuestNode);
            }
            return existingNodes;
        }

        /// <summary>
        /// Creates a new node of a given action type. auto assigning its ID by the time the action has been repeated
        /// </summary>
        /// <param name="behaviour">behaviour that owns the graph</param>
        /// <param name="position"></param>
        public GraphNode AddNewNode(QuestBehaviour behaviour, Vector2 position)
        {
            switch (behaviour.activeGraphNodeType)
            {
                case { } t when t == typeof(QuestNode):
                {
                    return AddNewQuestNode(behaviour.ActionToSet, position);
                }

                case { } t when t == typeof(OrNode):
                {
                    var newNode = new OrNode(position, this);
                    AddNodeToGraph(newNode);
                    return newNode;
                }

                case { } t when t == typeof(AndNode):
                {
                    var newNode = new AndNode(position, this);
                    AddNodeToGraph(newNode);
                    return newNode;
                }

                default:
                    return null; // failed generation
            }
        }

        public QuestNode AddNewQuestNode(string action, Vector2 position)
        {
            int suffix = 0;
            string nodeID;
            do
            {
                nodeID = $"{action} ({suffix++})";
            } while (GetQuestNodes().Any(questNode => questNode.ID == nodeID));

            var newNode = new QuestNode(nodeID, position, action, this);
            AddNodeToGraph(newNode);
            return newNode;
        }
        
        /// <summary>
        /// Adds the node to the graph
        /// </summary>
        /// <param name="node"></param>
        public void AddNodeToGraph(GraphNode node)
        {
            graphNodes.Add(node);
            OnAddNode?.Invoke(node);

            if (node is not QuestNode questNode) return;
            
            // if no root try to set the first node as if it is quest node
            if(root == null) SetRoot(questNode);
            // select when added
            _selectedQuestNode = questNode;
            // update UI
            NodeDataChanged(_selectedQuestNode);
        }
        
  
        
        /// <summary>
        /// finds the edge of a referenced node. makes a new action that turns into the "To"
        /// of the connection and makes a new edge from the new action and the original "To"
        /// of the referenced node
        /// </summary>
        /// <param name="action">The action type for the new node</param>
        /// <param name="referenceNode">The node after which the new node will be inserted</param>
        public QuestNode InsertQuestNodeAfter(string action, QuestNode referenceNode)
        {
            var position = Vector2.zero;
            if (referenceNode == null || !graphNodes.Contains(referenceNode))
            {
                Debug.LogWarning("Reference node is null or not in the graph. Adding as regular node.");
                return AddNewQuestNode(action, position);
            }
            
            position = referenceNode.Position;
            position.x += _viewNodeWidthOffset;
            
            // adds the action to the graph
            var newNode = AddNewQuestNode(action, position);
  
            int index = graphNodes.IndexOf(referenceNode);
            index = Math.Clamp(index, 0, graphNodes.Count - 1);

            // FInd existing edges to the reference
            var incomingEdges = GetRoots(referenceNode).ToList();
            foreach (var edge in incomingEdges)
            {
                RemoveEdge(edge);
            }
            
            // Add edge from reference node to new node
            AddEdge(referenceNode, newNode);
            
            // Find existing edges from the reference node
            var outgoingEdges = GetBranches(referenceNode).ToList();
            foreach (var edge in outgoingEdges)
            {
                RemoveEdge(edge);
                AddEdge(newNode, referenceNode);
                AddEdge(newNode, edge.To);
            }
            return newNode;
        }

        /// <summary>
        /// Inserts a new node before a specified reference node
        /// </summary>
        /// <param name="action">The action type for the new node</param>
        /// <param name="referenceNode">The node before which the new node will be inserted</param>
        public QuestNode InsertNodeBefore(string action, QuestNode referenceNode)
        {
            var position = Vector2.zero;
            if (referenceNode == null || !graphNodes.Contains(referenceNode))
            {
                Debug.LogWarning("Reference node is null or not in the graph. Adding quest node without connections.");
                return AddNewQuestNode(action, position);
            }
            
            position =  referenceNode.Position;
            position.x -= _viewNodeWidthOffset;
            var newNode = AddNewQuestNode(action, position);

            int index = graphNodes.IndexOf(referenceNode);
            
            // Find existing edges to the reference node
            var incomingEdges = GetRoots(referenceNode).ToList();
            foreach (var edge in incomingEdges)
            {
                RemoveEdge(edge);
                AddEdge(newNode, referenceNode);
                foreach (GraphNode from in edge.From)
                {
                    AddEdge(newNode, from);
                }
            }
            
            // Add edge from new node to reference node
            AddEdge(newNode, referenceNode);
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
        
        /// <summary>
        /// Removes a quest node and deletes any connections with it
        ///
        /// may deselect selected node
        /// </summary>
        /// <param name="node"></param>
        public void RemoveQuestNode(QuestNode node)
        {
            // undo connections
            var edgesToRemove = GetEdgesWithNode(node);
            foreach (var e in edgesToRemove) RemoveEdge(e);
            
            graphNodes.Remove(node);
            OnRemoveNode?.Invoke(node);
            
            // make sure to delete reference
            if(node==_selectedQuestNode) _selectedQuestNode = null;
            NodeDataChanged(_selectedQuestNode);
        }

        private List<QuestEdge> GetEdgesWithNode(QuestNode node)
        {
            return questEdges.Where(e => e.From.Contains(node) || e.To.Equals(node)).ToList();
        }


        public Tuple<string, LogType> AddEdge(GraphNode from, GraphNode to)
        {
            // Root node restriction
            if (to == root || from == root)
            {
                return Tuple.Create("Root node can only have Single connection type.", LogType.Error);
            }
            
            var newEdge = new QuestEdge(from, to);
            
            if (!QuestGraphHelper.IsValidEdge(newEdge, this,
                    out string message, out LogType logType))
            {
                return Tuple.Create(message, logType);
            }
            
            questEdges.Add(newEdge);
            OnAddEdge?.Invoke(newEdge);
     
            var connectionInfo = $"Connection: {from} → {to}";
            return Tuple.Create(connectionInfo, LogType.Log);
        }

        
        /// <summary>
        /// Removes the connection between two nodes by using the positions of the edge
        /// </summary>
        /// <param name="position">the position clicked in the graph</param>
        /// <param name="delta">higher delta easier to catch the line</param>
        public void RemoveEdgeByPosition(Vector2Int position, float delta)
        {
            var edge = GetEdge(position, delta);
            RemoveEdge(edge);
        }
        
        /// <summary>
        /// Removes the edge from the graph
        /// </summary>
        /// <param name="edge"></param>
        private void RemoveEdge(QuestEdge edge)
        {
            if (edge == null) return;
            questEdges.Remove(edge);
            OnRemoveEdge?.Invoke(edge);
        }
        
        public override bool IsEmpty()
        {
            return graphNodes.Count == 0;
        }
        
        public override object Clone()
        {
            var clone = new QuestGraph
            {
                grammarGuid = grammarGuid
            };

            clone.graphNodes.Clear();

            var nodes = graphNodes.Select(CloneRefs.Get).Cast<QuestNode>();

            foreach (var node in nodes)
            {
                clone.graphNodes.Add(node);
            }

            var edgesClone = questEdges.Select(CloneRefs.Get).Cast<QuestEdge>();
            foreach (var edge in edgesClone)
            {
                clone.questEdges.Add(edge);
            }

            clone.root = CloneRefs.Get(Root) as QuestNode;

            return clone;
        }
        public List<object> GetSelected(Vector2Int position)
        {
            var selected = new List<object>();

            var node = GetGraphNode(position);
            if (node != null)
                selected.Add(node);

            return selected;
        }

        private GraphNode GetGraphNode(Vector2Int position)
        {
            foreach (var graphNode in graphNodes)
            {
                if(graphNode.Position == position) return graphNode;
            }
            return null;
        }

        /* Unused - QuestFlow pertinent
         
         
        /// <summary>
        /// It updates the quest node types as well as removing all the connections
        /// (edges) redoing them, as this function is called from the Quest History' list
        /// reordering
        /// </summary>
        public void UpdateQuestNodes()
        {
            if (!graphNodes.Any() || !questEdges.Any()) return;
     
            foreach (var qe in questEdges)
            {
                qe.To.NodeType = TreeEditorHelper.NodeType.Middle;
                foreach (var from in qe.From)
                {
                    from.NodeType = TreeEditorHelper.NodeType.Middle;
                }
              
            }
            
            // the root must always have a single from(single node type)
            SetRoot(questEdges.First().From.First());
            
            questEdges.Last().To.NodeType = TreeEditorHelper.NodeType.Goal;
                
            _onUpdateGraph?.Invoke();
        }
        
        public void Reorder()
        {
            if (!graphNodes.Any()) return;

            // 1. Reset root and types before doing anything else
            foreach (var qn in graphNodes)
            {
                qn.NodeType = TreeEditorHelper.NodeType.Middle;
            }

            var firstNode = graphNodes.First();
            var lastNode = graphNodes.Last();

            SetRoot(firstNode);
            lastNode.NodeType = TreeEditorHelper.NodeType.Goal;

            // 2. Clear old edges
            questEdges.Clear();

            // 3. Add new sequential edges
            for (int i = 0; i < graphNodes.Count - 1; i++)
            {
                var node1 = graphNodes[i];
                var node2 = graphNodes[i + 1];

                var result = AddEdge(node1, node2);
                if (result.Item2 == LogType.Error)
                {
                    Debug.LogWarning($"[QuestGraph::Reorder] Failed to add edge: {result.Item1}");
                }
            }

            //UpdateFlow?.Invoke(); // Optional: force redraw if needed
        }
    */
        
        public void ChangeConnection(QuestEdge edge, Type graphNodeType)
        {
            throw new NotImplementedException(); 
        }
        
        #endregion
 
        
        #region MODULE FUNCTIONS: These are not used
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
        }        public override void Rewrite(LBSModule other)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

}