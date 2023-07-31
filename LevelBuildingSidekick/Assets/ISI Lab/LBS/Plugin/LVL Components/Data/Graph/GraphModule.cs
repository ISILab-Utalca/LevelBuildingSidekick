using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;
using LBS.Components.Specifics;
using LBS.Components.TileMap;

namespace LBS.Components.Graph
{
    [System.Serializable]
    public class GraphModule<T> : LBSModule where T : LBSNode //REVISAR CLASE PARA IMPLEMENTAR METODOS Y TIPO T (!!!)
    {
        #region FIELDS

        [SerializeField, JsonRequired, SerializeReference]
        protected List<LBSNode> nodes;

        [SerializeField, JsonRequired, SerializeReference]
        protected List<LBSEdge> edges;

        #endregion

        #region PROPERTIES

        [JsonIgnore]
        public int NodeCount => nodes.Count;

        [JsonIgnore]
        public int EdgeCount => edges.Count;

        #endregion

        #region COSNTRUCTORS

        /// <summary>
        /// Default base class constructor of LBSGraphData.  
        /// </summary>
        public GraphModule() : base() 
        { 
            ID = GetType().Name;
            nodes = new List<LBSNode>();
            edges = new List<LBSEdge>();
        }

        public GraphModule(List<T> nodes, List<LBSEdge> edges, string key) : base(key) 
        {
            this.nodes = new List<LBSNode>(nodes);
            this.edges = new List<LBSEdge>(edges);
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Remove node by ID/Label.
        /// </summary>
        /// <param name="ID">ID given to remove the node.</param>
        public T RemoveNode(string ID)
        {
            var r = nodes.Find(n => n.ID == ID) as T;
            if (r != null)
            {
                RemoveNode(r);
                return r;
            }
            return null;
        }

        /// <summary>
        /// Remove node by reference and remove conection, 
        /// also removes the edges connected to the node.
        /// </summary>
        /// <param name="node">Node data given to find the node.</param>
        public bool RemoveNode(T node)
        {
            if (node != null && nodes.Contains(node))
            {
                RemoveEdges(node);
                nodes.Remove(node);
                OnChanged?.Invoke(this);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Return the first node whit the ID/Label.
        /// </summary>
        /// <param name="id">Label given to get the node.</param>
        /// <returns>Return the node found by label or null if no such exists.</returns>
        public T GetNode(string id)
        {
            return nodes.Find(n => n.ID == id) as T;
        }

        /// <summary>
        /// Return node by index.
        /// </summary>
        /// <param name="index">Index to get the node.</param>
        /// <returns>Return the node found by index.</returns>
        public T GetNode(int index)
        {
            return nodes[index] as T;
        }

        public T GetNode(Vector2Int position)
        {
            return nodes.Find(n => (new Rect(n.Position - Vector2Int.one*60, Vector2Int.one*120)).Contains(position)) as T;
        }

        /// <summary>
        /// Return a list of nodes.
        /// </summary>
        /// <returns> List of nodes.</returns>
        public List<T> GetNodes()
        {
            return new List<T>(nodes.Select(n => n as T));
        }

        /// <summary>
        /// Adds a new node to the list in the graph.
        /// If the label of the new node is already in use, the method will 
        /// append an index to the label until it is unique.
        /// </summary>
        /// <param name="node">Data node to add.</param>
        public bool AddNode(T node)
        {
            int index = nodes.Count;
            if (!nodes.Any(n => n.ID == node.ID))
            {
                nodes.Add(node);
                OnChanged?.Invoke(this);
                return true;
            }

            var tempName = "";
            do
            {
                tempName = node.ID + " " + index;
                index++;
            } while (nodes.Any(n => n.ID == tempName));

            node.ID = tempName;
            nodes.Add(node);

            OnChanged?.Invoke(this);
            return true;
        }

        /// <summary>
        /// Returns all edges that are connected to the node 
        /// delivered by parameters.
        /// </summary>
        /// <param name="node"> Node data to get the node.</param>
        /// <returns> A enumerable collection of edges containing the given node.</returns>
        public IEnumerable<LBSEdge> GetEdges(T node)
        {
            return edges.Where(e => e.Contains(node));
        }

        /// <summary>
        /// Return edge by index.
        /// </summary>
        /// <param name="index"> int variable that indicate edge position in array edges</param>
        /// <returns> edge of edges array</returns>
        public LBSEdge GetEdge(int index)
        {
            return edges[index];
        }

        public LBSEdge GetEdge(Vector2 position, float delta)
        {
            foreach(var e in edges)
            {
                var dist = position.DistanceToLine(e.FirstNode.Position, e.SecondNode.Position);
                if (dist < delta)
                    return e;
            }
            return null;
        }

        /// <summary>
        /// Get a list of edges of type LBSEdgeData
        /// </summary>
        /// <returns> List of edges</returns>
        public List<LBSEdge> GetEdges()
        {
            return new List<LBSEdge>(edges);
        }

        /// <summary>
        /// Remove al edge conected to the node delivered by parameters. 
        /// </summary>
        /// <param name="node"> Data node to remove edges. </param>
        public void RemoveEdges(T node)
        {
            var rs = edges.FindAll(e => e.Contains(node));
            rs.ForEach(r => edges.Remove(r));
        }

        /// <summary>
        /// Remove the edge connected to the nodes delivered by parameters.
        /// </summary>
        /// <param name="n1"> First node to disconnect edge.</param>
        /// <param name="n2"> Second node to disconnect edge.</param>
        public void RemoveEdge(T n1, T n2)
        {
            var e = edges.Find(e => e.Contains(n1) && e.Contains(n2));
            RemoveEdge(e);
        }

        /// <summary>
        /// Remove edge.
        /// </summary>
        /// <param name="edge"> Data to edge to remove. </param>
        public void RemoveEdge(LBSEdge edge)
        {
            edges.Remove(edge);
        }

        /// <summary>
        /// Add new edge.
        /// </summary>
        /// <param name="n1"> First node to add a edge.</param>
        /// <param name="n2"> Second node to add a edge.</param>
        public void AddEdge(T n1, T n2)
        {
            var e = new LBSEdge(n1, n2);
            AddEdge(e);
        }

        /// <summary>
        /// Add edge.
        /// </summary>
        /// <param name="edge"> Edge to add. </param>
        /// <returns> If exists a connection between the two nodes before show a warning
        /// message.</returns>
        public void AddEdge(LBSEdge edge)
        {
            //edge.OnChange += (n) => Debug.Log("[implementar]"); // <------------------------------------- !!!!!
            var n1 = edge.FirstNode;
            var n2 = edge.SecondNode;

            if(!(nodes.Contains(n1) && nodes.Contains(n2)))
            {
                return;
            }

            if (edges.Any(e => e.Contains(n1) && e.Contains(n2)))
            {
                Debug.LogWarning("Nodes '" + n1.ID + "' and '" + n2.ID + "' are already connected.");
                return;
            }
            edges.Add(edge);
            OnChanged?.Invoke(this);
        }

        /// <summary>
        /// Returns the neighboring nodes of the node delivered by parameters.
        /// </summary>
        /// <param name="node"> Node to find neighbors. </param>
        /// <returns> List of neighbors.</returns>
        public List<T> GetNeighbors(T node)
        {
            var conects = edges.Where(e => e.Contains(node)).ToList();

            var firsts = conects.Where(e => e.FirstNode != node).Select(e => (T)e.FirstNode);
            var seconds = conects.Where(e => e.SecondNode != node).Select(e => (T)e.SecondNode);
            var neigs = firsts.ToList();
            neigs.AddRange(seconds.ToList());
            return neigs.ToList();
        }

        /// <summary>
        /// Print the nodes and edges amount by console.
        /// </summary>
        public override void Print()
        {
            var msg = "";
            msg += "<b>Room char. (step 1)</b>" + "\n";
            msg += "Node amount: " + this.nodes.Count + "\n";
            msg += "Edge amount: " + this.edges.Count + "\n";
            Debug.Log(msg);
        }

        /// <summary>
        /// Clear the edges and nodes.
        /// </summary>
        public override void Clear()
        {
            edges.Clear();
            nodes.Clear();
            OnChanged?.Invoke(this);
        }


        public override bool IsEmpty()
        {
            return (nodes.Count <= 0 && edges.Count <= 0);
        }

        public override object Clone()
        {
            var module = new GraphModule<T>();
            module.nodes = nodes.Select(n => n.Clone() as LBSNode).ToList(); // new List<LBSNode>(nodes);
            module.edges = edges.Select(n => n.Clone() as LBSEdge).ToList(); //new List<LBSEdge>(edges);
            return module;
        }

        public override void OnAttach(LBSLayer layer)
        {

        }

        public override void OnDetach(LBSLayer layer)
        {

        }

        public override Rect GetBounds()
        {
            throw new System.NotImplementedException();
        }

        public override void Rewrite(LBSModule module)
        {
            throw new System.NotImplementedException();
        }

        public override void OnReload(LBSLayer layer)
        {

        }

        #endregion
    }

    public class LBSGraph : GraphModule<LBSNode>
    {
        public LBSGraph() : base() {}

        public LBSGraph(List<LBSNode> nodes, List<LBSEdge> edges, string key) : base(nodes, edges, key) {}

        public override object Clone()
        {
            return new LBSGraph(nodes.Select(n => n.Clone() as LBSNode).ToList(), edges.Select(e => e.Clone() as LBSEdge).ToList(), id);
        }
    }
}

