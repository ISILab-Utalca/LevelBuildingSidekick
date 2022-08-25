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
    //[CreateAssetMenu(menuName = "LevelBuildingSidekick/Level Represetation/Graph Representation/Level")]
    public class LBSGraphData : LBSRepesentationData
    {
        [SerializeField]
        public List<LBSNodeData> nodes = new List<LBSNodeData>(); // private

        [SerializeField]
        public List<LBSEdgeData> edges = new List<LBSEdgeData>(); // private
        public int cellSize = 32; //Add to View parameters (??)

        [JsonIgnore]
        public override Type ControllerType => typeof(LBSGraphController);


        internal void RemoveNode(LBSNodeData node)
        {
            nodes.Remove(node);
        }

        internal LBSNodeData GetNodeByPosition(Vector2 currentPos)
        {
            return nodes.Find(n => n.Rect.Contains(currentPos));
        }

        internal LBSNodeData GetNodeByLabel(string value)   // by ID (??)
        {
            return nodes.Find(n => n.label == value);
        }

        internal object GetEdge(LBSNodeController firstNode, List<LBSNodeData> nodes)
        {
            throw new NotImplementedException();
        }

        internal void AddNode(LBSNodeData node)
        {
            int index = nodes.Count;
            do
            {
                node.label = "Node: " + index.ToString();
                index++;
            }
            while (!nodes.Any(n => n.label == node.label));

            nodes.Add(node);
        }

        internal void AddEdge(LBSEdgeData edge)
        {
            edges.Add(edge);
        }

        public List<LBSNodeData> GetNeighbors(LBSNodeData node)
        {
            var conects =  edges.Where(e => e.Contains(node.label)).ToList();
            var neigs = new List<LBSNodeData>();
            conects.ForEach((e) =>
            {
                if (e.FirstNodeLabel != node.label)
                    neigs.Add(GetNodeByLabel(e.FirstNodeLabel));
                if (e.SecondNodeLabel != node.label)
                    neigs.Add(GetNodeByLabel(e.SecondNodeLabel));
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

