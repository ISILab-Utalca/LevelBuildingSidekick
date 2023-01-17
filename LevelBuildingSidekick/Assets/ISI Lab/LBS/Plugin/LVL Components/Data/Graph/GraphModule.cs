using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.Graph;
using Newtonsoft.Json;
using System.Linq;

namespace LBS.Components.Graph
{
    [System.Serializable]
    public class GraphModule<T> : LBSModule where T : LBSNode //REVISAR CLASE PARA IMPLEMENTAR METODOS Y TIPO T (!!!)
    {
        //FIELDS

        [SerializeField, JsonRequired, SerializeReference]
        List<T> nodes = new List<T>();

        [SerializeField, JsonRequired, SerializeReference]
        List<LBSEdge> edges = new List<LBSEdge>();

        //PROPERTIES

        [JsonIgnore]
        public int NodeCount => nodes.Count;


        [JsonIgnore]
        public int EdgeCount => edges.Count;

        //COSNTRUCTORS

        /// <summary>
        /// Default base class constructor of LBSGraphData.  
        /// </summary>
        public GraphModule() : base() { }

        //METHODS

        /// <summary>
        /// Clear the edges and nodes.
        /// </summary>
        public override void Clear()
        {
            edges = new List<LBSEdge>();
            nodes = new List<T>();
        }

        /// <summary>
        /// Remove node by ID/Label.
        /// </summary>
        /// <param name="ID">ID given to remove the node.</param>
        public void RemoveNode(string ID)
        {
            var r = nodes.Find(n => n.ID == ID);
            if (r != null)
                RemoveNode(r);
        }

        /// <summary>
        /// Remove node by reference and remove conection, 
        /// also removes the edges connected to the node.
        /// </summary>
        /// <param name="node">Node data given to find the node.</param>
        public void RemoveNode(T node)
        {
            RemoveEdges(node);
            nodes.Remove(node);
        }

        /// <summary>
        /// Return the first node whit the ID/Label.
        /// </summary>
        /// <param name="id">Label given to get the node.</param>
        /// <returns>Return the node found by label or null if no such exists.</returns>
        public T GetNode(string id)
        {
            return nodes.Find(n => n.ID == id);
        }

        /// <summary>
        /// Return node by index.
        /// </summary>
        /// <param name="index">Index to get the node.</param>
        /// <returns>Return the node found by index.</returns>
        public T GetNode(int index)
        {
            return nodes[index];
        }

        /// <summary>
        /// Return a list of nodes.
        /// </summary>
        /// <returns> List of nodes.</returns>
        public List<T> GetNodes()
        {
            return new List<T>(nodes);
        }

        /// <summary>
        /// Adds a new node to the list in the graph.
        /// If the label of the new node is already in use, the method will 
        /// append an index to the label until it is unique.
        /// </summary>
        /// <param name="node">Data node to add.</param>
        public void AddNode(T node)
        {
            //node.OnChange += (n) => Debug.Log("[implementar]"); // <------------------------------------- !!!!!

            int index = nodes.Count;
            if (!nodes.Any(n => n.ID == node.ID))
            {
                nodes.Add(node);
                return;
            }

            var tempName = "";
            do
            {
                tempName = node.ID + " " + index;
                index++;
            } while (nodes.Any(n => n.ID == tempName));

            node.ID = tempName;
            nodes.Add(node);
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
            //Debug.Log(Label);
            /*foreach(var n in nodes)
            {
                Debug.Log(n.Label);
            }*/
            //Debug.Log(n1);
            //Debug.Log(n2);
            /*if (n1.Label == n2.Label)
            {
                Debug.LogWarning("Cannot connect a node to itself.");
                return;
            }*/

            if (edges.Any(e => e.Contains(n1) && e.Contains(n2)))
            {
                Debug.LogWarning("Nodes '" + n1.ID + "' and '" + n2.ID + "' are already connected.");
                return;
            }
            edges.Add(edge);
        }

        /// <summary>
        /// Returns the neighboring nodes of the node delivered by parameters.
        /// </summary>
        /// <param name="node"> Node to find neighbors. </param>
        /// <returns> List of neighbors.</returns>
        public List<T> GetNeighbors(T node)
        {
            var conects = edges.Where(e => e.Contains(node)).ToList();

            var neigs = conects.Where(e => e.FirstNode != node).Select(e => (T)e.FirstNode);
            neigs.Concat(conects.Where(e => e.SecondNode != node).Select(e => (T)e.SecondNode));
            /*
            var neigs = new List<T>();
            conects.ForEach((e) =>
            {
                if (e.FirstNode != node)
                {
                    var n = e.FirstNode;
                    neigs.Add((T)n);
                }

                if (e.SecondNode != node)
                {
                    var n = e.SecondNode;
                    neigs.Add((T)n);
                }

            });*/

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

        public override object Clone()
        {
            var module = new GraphModule<T>();
            module.nodes = this.nodes.Select(n => n.Clone() as T).ToList();
            module.edges = this.edges.Select(e => e.Clone() as LBSEdge).ToList();
            return module;
        }
    }

    [System.Serializable]
    public class LBSBaseGraph : GraphModule<LBSNode> { }
}

