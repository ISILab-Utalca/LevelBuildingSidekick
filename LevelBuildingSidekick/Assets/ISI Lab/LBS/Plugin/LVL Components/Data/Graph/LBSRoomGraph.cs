using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.Components.Specifics;
using System.Linq;

namespace LBS.Components.Graph
{
    [System.Serializable]
    public class LBSRoomGraph : GraphModule<RoomNode>
    {
        public LBSRoomGraph() : base() { Key = GetType().Name; }
        public LBSRoomGraph(List<RoomNode> nodes, List<LBSEdge> edges, string key) : base(nodes, edges, key) { }

        public override object Clone()
        {
            return new LBSRoomGraph(nodes.Select(n => n as RoomNode).ToList(), edges.Select(e => e).ToList(), key);
        }
    }
}

