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

        public List<LBSNodeData> GetNeighbors(LBSNodeData node)
        {
            var nIDs = node.room.neighbors;
            List<LBSNodeData> neighbors = new List<LBSNodeData>();
            foreach(var id in nIDs)
            {
                var n = nodes.Find(n => n.label == id);
                if(n != null)
                    neighbors.Add(n);
            }
            return neighbors;
        }
    }
}

