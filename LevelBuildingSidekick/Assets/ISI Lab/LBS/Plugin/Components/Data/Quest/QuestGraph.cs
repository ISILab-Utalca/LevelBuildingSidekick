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

namespace ISILab.LBS.Modules
{
    [Serializable]
    public class QuestGraph : LBSModule, ICloneable, ISelectable
    {
        #region FIELDS
        [SerializeField, JsonRequired]
        private Vector2Int nodeSize = new(5, 1);

        [SerializeField, SerializeReference, JsonRequired]
        private List<QuestNode> questNodes = new();

        [SerializeField, SerializeReference, JsonRequired]
        private List<QuestEdge> questEdges = new();

        [SerializeField, SerializeReference, JsonRequired]
        private QuestNode root;
        
        private HashSet<QuestNode> _newNodes = new ();
        private HashSet<QuestNode> _expiredNodes = new ();
        
        private HashSet<QuestEdge> _newEdges = new ();
        private HashSet<QuestEdge> _expiredEdges = new ();
        
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

        public List<QuestNode> QuestNodes => questNodes;
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
        
        #region METHODS
        
        public void DataChanged(QuestNode node) {_onQuestNodeSelected?.Invoke(node);}
        
        public void LoadGrammar()
        {
            if (_grammar == null) _grammar = LBSAssetMacro.LoadAssetByGuid<LBSGrammar>(grammarGuid);
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
            
            List<QuestEdge> edges = new List<QuestEdge>();
            foreach (var edge in questEdges)
            {
                foreach (var from in edge.From)
                {
                    edges.AddRange(questEdges.Where(e => from.ID == node.ID).ToList());
                }
            }
            
            return edges;
        }
        public List<QuestEdge> GetRoots(QuestNode node)
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
            root.NodeType = NodeType.Start;
        }


        /// <summary>
        /// Creates a new node of a given action type. auto assigning its ID by the time the action has been repeated
        /// </summary>
        /// <param name="action"></param>
        /// <param name="position"></param>
        public QuestNode CreateAddNode(string action, Vector2 position)
        {
            int suffix = 0;
            string nodeID;
            do
            {
                nodeID = $"{action} ({suffix++})";
            } while (QuestNodes.Any(n => n.ID == nodeID));

            QuestNode newNode = new QuestNode(nodeID, position, action, this);
            InternalAddNode(newNode);
            
            return newNode;
        }
        
        /// <summary>
        /// Adds the node to the graph
        /// </summary>
        /// <param name="node"></param>
        public void InternalAddNode(QuestNode node)
        {
            if(root == null) SetRoot(node);
            questNodes.Add(node);
            _newNodes.Add(node);
            
            OnAddNode?.Invoke(node);
            
            _selectedQuestNode = node;
            DataChanged(_selectedQuestNode);
        }
        
        /// <summary>
        /// Adds the node to the graph
        /// </summary>
        /// <param name="node"></param>
        public void InternalInsertNode(QuestNode node, int index)
        {
            if(root == null) SetRoot(node);
            questNodes.Insert(index, node);
            _newNodes.Add(node);
            
            OnAddNode?.Invoke(node);
            
            _selectedQuestNode = node;
            DataChanged(_selectedQuestNode);
        }
        
        /// <summary>
        /// Inserts a new node after a specified reference node
        /// </summary>
        /// <param name="action">The action type for the new node</param>
        /// <param name="referenceNode">The node after which the new node will be inserted</param>
        public QuestNode InsertNodeAfter(string action, QuestNode referenceNode)
        {
            var position = Vector2.zero;
            if (referenceNode == null || !questNodes.Contains(referenceNode))
            {
                Debug.LogWarning("Reference node is null or not in the graph. Adding as regular node.");
                return CreateAddNode(action, position);
            }

            int suffix = 0;
            string nodeID;
            do
            {
                nodeID = $"{action} ({suffix++})";
            } while (QuestNodes.Any(n => n.ID == nodeID));

            position = referenceNode.Position;
            position.x += _viewNodeWidthOffset;
            var newNode = new QuestNode(nodeID, position, action, this);
  
            int index = questNodes.IndexOf(referenceNode);
            index = Math.Clamp(index, 0, questNodes.Count - 1);
            InternalInsertNode(newNode, index+1);

            // Find existing edges from the reference node
            var outgoingEdges = GetBranches(referenceNode).ToList();
            foreach (var edge in outgoingEdges)
            {
                InternalRemoveEdge(edge);
                AddEdge(newNode, edge.To);
            }

            // Add edge from reference node to new node
            AddEdge(referenceNode, newNode);

            UpdateQuestNodes();
            UpdateFlow?.Invoke();
            
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
            if (referenceNode == null || !questNodes.Contains(referenceNode))
            {
                Debug.LogWarning("Reference node is null or not in the graph. Adding as regular node.");
                return CreateAddNode(action, position);
            }

            int suffix = 0;
            string nodeID;
            do
            {
                nodeID = $"{action} ({suffix++})";
            } while (QuestNodes.Any(n => n.ID == nodeID));

            position =  referenceNode.Position;
            position.x -= _viewNodeWidthOffset;
            var newNode = new QuestNode(nodeID, position, action, this);

            int index = questNodes.IndexOf(referenceNode);
            index = Math.Clamp(index, 0, questNodes.Count - 1);
            InternalInsertNode(newNode, index);
            
            // to update the types
            UpdateQuestNodes();
            
            // Find existing edges to the reference node
            var incomingEdges = GetRoots(referenceNode).ToList();
            foreach (var edge in incomingEdges)
            {
                InternalRemoveEdge(edge);
            }
            
            // Add edge from new node to reference node
            AddEdge(newNode, referenceNode);
            
            // to update the visuals
            UpdateQuestNodes();
            UpdateFlow?.Invoke();

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
                var newNode = InsertNodeAfter(action, iterationNode);
                if (newNode is null) continue;
                
                iterationNode = newNode;
                newNodes.Add(newNode);
            }
        
            RemoveQuestNode(referenceNode);
            
            // to update the visuals
            UpdateQuestNodes();
            UpdateFlow?.Invoke();
        }
        
        public void RemoveQuestNode(QuestNode node)
        {
            questNodes.Remove(node);
            _expiredNodes.Add(node);
            
            var edgesToRemove = questEdges.Where(e => e.From.Equals(node) || e.To.Equals(node)).ToList();
            foreach (var e in edgesToRemove) InternalRemoveEdge(e);
            OnRemoveNode?.Invoke(node);
            _selectedQuestNode = null;
            DataChanged(_selectedQuestNode);
        }
        

        public Tuple<string, LogType> AddEdge(QuestNode from, QuestNode to, ConnectionType connectionType = ConnectionType.Single)
        {
            // Root node restriction
            if (to == root ||
                from == root && connectionType != ConnectionType.Single)
            {
                return Tuple.Create("Root node can only have Single connection type.", LogType.Error);
            }
            
            if (!QuestGraphHelper.IsValidEdge(from, to, questEdges, root, this,
                    out string message, out LogType logType))
            {
                return Tuple.Create(message, logType);
            }

            var edge = new QuestEdge(from, to);
            questEdges.Add(edge);
            _newEdges.Add(edge);

            OnAddEdge?.Invoke(edge);
            UpdateFlow?.Invoke();

            UpdateQuestNodes();
            CheckGraphByGrammar();
            
            var connectionInfo = $"Connection: {from.QuestAction} → {to.QuestAction}";
            return Tuple.Create(connectionInfo, LogType.Log);
        }

        
        /// <summary>
        /// Removes the connection between two nodes by using the positions of the edge
        /// </summary>
        /// <param name="position">the position clicked in the graph</param>
        /// <param name="delta">higher delta easier to catch the line</param>
        public void RemoveEdge(Vector2Int position, float delta)
        {
            var edge = GetEdge(position, delta);
            _expiredEdges.Add(edge);
            InternalRemoveEdge(edge);
        }
        
        /// <summary>
        /// Removes the edge from the graph
        /// </summary>
        /// <param name="edge"></param>
        private void InternalRemoveEdge(QuestEdge edge)
        {
            if (edge == null) return;
            questEdges.Remove(edge);
            _expiredEdges.Add(edge);
            
            OnRemoveEdge?.Invoke(edge);
         
        }
        
        public override bool IsEmpty()
        {
            return questNodes.Count == 0;
        }
        
        public override object Clone()
        {
            var clone = new QuestGraph
            {
                grammarGuid = grammarGuid
            };

            clone.questNodes.Clear();

            var nodes = questNodes.Select(CloneRefs.Get).Cast<QuestNode>();

            foreach (var node in nodes)
            {
                clone.questNodes.Add(node);
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

            var node = GetQuestNode(position);
            if (node != null)
                selected.Add(node);

            return selected;
        }

        
        
        /// <summary>
        /// It updates the quest node types as well as removing all the connections
        /// (edges) redoing them, as this function is called from the Quest History' list
        /// reordering
        /// </summary>
        public void UpdateQuestNodes()
        {
            if (!questNodes.Any() || !questEdges.Any()) return;
     
            foreach (var qe in questEdges)
            {
                qe.To.NodeType = NodeType.Middle;
                foreach (var from in qe.From)
                {
                    from.NodeType = NodeType.Middle;
                }
              
            }
            
            // the root must always have a single from(single node type)
            SetRoot(questEdges.First().From.First());
            
            questEdges.Last().To.NodeType = NodeType.Goal;
                
            _onUpdateGraph?.Invoke();
        }
        
        public void Reorder()
        {
            if (!questNodes.Any()) return;

            // 1. Reset root and types before doing anything else
            foreach (var qn in questNodes)
            {
                qn.NodeType = NodeType.Middle;
            }

            var firstNode = questNodes.First();
            var lastNode = questNodes.Last();

            SetRoot(firstNode);
            lastNode.NodeType = NodeType.Goal;

            // 2. Clear old edges
            questEdges.Clear();

            // 3. Add new sequential edges
            for (int i = 0; i < questNodes.Count - 1; i++)
            {
                var node1 = questNodes[i];
                var node2 = questNodes[i + 1];

                var result = AddEdge(node1, node2);
                if (result.Item2 == LogType.Error)
                {
                    Debug.LogWarning($"[QuestGraph::Reorder] Failed to add edge: {result.Item1}");
                }
            }

            //UpdateFlow?.Invoke(); // Optional: force redraw if needed
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