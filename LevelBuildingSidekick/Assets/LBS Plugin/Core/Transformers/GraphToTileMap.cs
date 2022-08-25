using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.Representation.TileMap;
using LevelBuildingSidekick.Graph;
using System.Linq;

namespace LBS.Transformers
{
    public class GraphToTileMap : Transformer<LBSGraphData, LBSTileMapData>
    {
        public override LBSTileMapData Transform(LBSGraphData graph)
        {
            if (graph.nodes.Count <= 0)
            {
                Debug.LogWarning("[Error]: Graph node have 0 nodes.");
                return null;
            }

            Queue<LBSNodeData> open = new Queue<LBSNodeData>();
            HashSet<LBSNodeData> closed = new HashSet<LBSNodeData>();

            var parent = graph.nodes.OrderByDescending((n) => graph.GetNeighbors(n).Count).First();
            graph.nodes.ForEach(n => Debug.Log(n.label +": "+ graph.GetNeighbors(n).Count));
            open.Enqueue(parent);

            var tileMap = new LBSTileMapData();
            int h = (int)((parent.room.maxHeight + parent.room.minHeight) / 2f);
            int w = (int)((parent.room.maxWidth + parent.room.minWidth) / 2f);
            tileMap.AddRoom(Vector2Int.zero, h, w, parent.label);

            while (open.Count > 0)
            {
                parent = open.Dequeue();

                var childs = graph.GetNeighbors(parent).OrderBy(n => Utility.MathTools.GetAngleD15(parent.Centroid, n.Centroid));

                foreach (var child in childs)
                {
                    if (closed.Contains(child) || open.ToHashSet().Contains(child))
                        continue;

                    var room = child.room;
                    open.Enqueue(child);

                    var parentH = (int)((room.maxHeight + room.minHeight) / 2f);
                    var parentW = (int)((room.maxWidth + room.minWidth) / 2f);
                    var childH = (int)((room.maxHeight + room.minHeight) / 2f);
                    var childW = (int)((room.maxWidth + room.minWidth) / 2f);

                    var dir = ((Vector2)(child.Centroid - parent.Centroid)).normalized;
                    var posX = dir.x * ((childW + parentW) / 2f);
                    var posY = dir.y + ((childH + parentH) / 2f);
                    tileMap.AddRoom(new Vector2Int((int)posX, (int)posY), childW, childH, child.label);
                }

                closed.Add(parent);
            }

            return tileMap;
        }
    }
}
