using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick;
using System;
using Newtonsoft.Json;
using System.Linq;

namespace LevelBuildingSidekick.Graph
{
    [System.Serializable]
    public class LBSGraphData : LBSRepesentationData
    {
        [SerializeField, JsonRequired, SerializeReference]
        private List<LBSNodeData> nodes = new List<LBSNodeData>();

        [SerializeField, JsonRequired, SerializeReference]
        private List<LBSEdgeData> edges = new List<LBSEdgeData>();
        
        //public int cellSize = 32; //Add to View parameters (??)

        [JsonIgnore]
        public override Type ControllerType => typeof(LBSGraphController);

        internal void Clear()
        {
            edges = new List<LBSEdgeData>();
            nodes = new List<LBSNodeData>();
        }

        /// <summary>
        /// Remove node by ID/Label.
        /// </summary>
        /// <param name="ID"></param>
        internal void RemoveNode(string ID)
        {
            var r = nodes.Find(n => n.Label == ID);
            if (r != null)
                RemoveNode(r);
        }

        /// <summary>
        /// Remove node by refrence and remove conection, 
        /// also removes the edges connected to the node.
        /// </summary>
        /// <param name="node"></param>
        internal void RemoveNode(LBSNodeData node)
        {
            RemoveEdges(node);
            nodes.Remove(node);
        }

        /// <summary>
        /// Returns the first node that is in position.
        /// </summary>
        /// <param name="currentPos"></param>
        /// <returns></returns>
        [Obsolete]
        internal LBSNodeData GetNode(Vector2 currentPos)
        {
            return null; // nodes.Find(n => n.Rect.Contains(currentPos));
        }

        /// <summary>
        ///  Return the first node whit the ID/Label.
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        internal LBSNodeData GetNode(string label)   // by ID (??)
        {
            return nodes.Find(n => n.Label == label);
        }

        /// <summary>
        /// Return node by index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        internal LBSNodeData GetNode(int index)
        {
            return nodes[index];
        }

        internal List<LBSNodeData> GetNodes()
        {
            return new List<LBSNodeData>(nodes);
        }

        /// <summary>
        /// Add new node.
        /// </summary>
        /// <param name="node"></param>
        internal void AddNode(LBSNodeData node)
        {
            node.OnChange += (n) => Debug.Log("[implementar]"); // <------------------------------------- !!!!!

            int index = nodes.Count;
            if(!nodes.Any(n => n.Label == node.Label))
            {
                nodes.Add(node);
                return;
            }

            var tempName = "";
            do
            {
                tempName = node.Label + " " + index;
                index++;
            } while (!nodes.Any(n => n.Label == tempName));

            node.Label = tempName;
            nodes.Add(node);
        }

        /// <summary>
        /// Get node count.
        /// </summary>
        /// <returns></returns>
        internal int NodeCount()
        {
            return nodes.Count;
        }

        /// <summary>
        /// Returns all edges that are connected to the node 
        /// delivered by parameters.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        internal IEnumerable<LBSEdgeData> GetEdges(LBSNodeData node)
        {
            return edges.Where(e => e.Contains(node.Label));
        }

        /// <summary>
        /// Return edge by index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        internal LBSEdgeData GetEdge(int index)
        {
            return edges[index];
        }

        internal List<LBSEdgeData> GetEdges()
        {
            return edges;
        }

        /// <summary>
        /// Get edge Conut.
        /// </summary>
        /// <returns></returns>
        internal int EdgeCount()
        {
            return edges.Count;
        }

        /// <summary>
        /// Remove al edge conected to the node delivered by parameters. 
        /// </summary>
        /// <param name="node"></param>
        internal void RemoveEdges(LBSNodeData node)
        {
            var rs = edges.FindAll(e => e.Contains(node.Label));
            rs.ForEach(r => edges.Remove(r));
        }

        /// <summary>
        /// Remove the edge connected to the nodes delivered by parameters.
        /// </summary>
        internal void RemoveEdge(LBSNodeData n1, LBSNodeData n2)
        {
            var e = edges.Find(e => e.Contains(n1.Label) && e.Contains(n2.Label));
            RemoveEdge(e);
        }

        /// <summary>
        /// Remove edge.
        /// </summary>
        /// <param name="edge"></param>
        internal void RemoveEdge(LBSEdgeData edge)
        {
            edges.Remove(edge);
        }

        /// <summary>
        /// Add new edge.
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        internal void AddEdge(LBSNodeData n1,LBSNodeData n2)
        {
            var e = new LBSEdgeData(n1, n2);
            AddEdge(e);
        }

        /// <summary>
        /// Add edge.
        /// </summary>
        /// <param name="edge"></param>
        internal void AddEdge(LBSEdgeData edge)
        {
            //edge.OnChange += (n) => Debug.Log("[implementar]"); // <------------------------------------- !!!!!
            var n1 = edge.firstNode;
            var n2 = edge.secondNode;
            if (n1.Label == n2.Label)
            {
                Debug.LogWarning("Cannot connect a node to itself.");
                return;
            }

            if(edges.Any(e => e.Contains(n1.Label) && e.Contains(n2.Label)))
            {
                Debug.LogWarning("Nodes '" + n1.Label + "' and '" + n2.Label + "' are already connected.");
                return;
            }
            edges.Add(edge);
        }

        /// <summary>
        /// Returns the neighboring nodes of the node delivered by parameters.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public List<LBSNodeData> GetNeighbors(LBSNodeData node)
        {
            var conects =  edges.Where(e => e.Contains(node.Label)).ToList();
            var neigs = new List<LBSNodeData>();
            conects.ForEach((e) =>
            {
                if (e.FirstNodeLabel != node.Label)
                    neigs.Add(GetNode(e.FirstNodeLabel));
                if (e.SecondNodeLabel != node.Label)
                    neigs.Add(GetNode(e.SecondNodeLabel));
            });

            return neigs;
        }


        public override void Print()
        {
            var msg = "";
            msg += "<b>Room char. (step 1)</b>" + "\n";
            msg += "Node amount: " + this.nodes.Count + "\n";
            msg += "Edge amount: " + this.edges.Count + "\n";
            Debug.Log(msg);
        }
    }
}

