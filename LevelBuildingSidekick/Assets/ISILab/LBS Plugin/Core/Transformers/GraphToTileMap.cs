using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.Representation.TileMap;
using LevelBuildingSidekick.Graph;
using System.Linq;
using LevelBuildingSidekick.Schema;

namespace LBS.Transformers
{
    public class GraphToTileMap : Transformer<LBSGraphData, LBSTileMapData>
    {
        public override LBSTileMapData Transform(LBSGraphData graph)
        {
            if (graph.NodeCount() <= 0)
            {
                Debug.LogWarning("[Error]: 'Graph node' have 0 nodes.");
                return null;
            }

            Queue<LBSNodeData> open = new Queue<LBSNodeData>();
            HashSet<LBSNodeData> closed = new HashSet<LBSNodeData>();

            var parent = graph.GetNodes().OrderByDescending((n) => graph.GetNeighbors(n).Count).First() as RoomCharacteristicsData;
            open.Enqueue(parent);
            //Debug.Log("parent: "+parent.Label);

            var tileMap = new LBSTileMapData();
            int h = parent.RangeHeight.Middle;
            int w = parent.RangeWidth.Middle;
            tileMap.AddRoom(Vector2Int.zero, h, w, parent.Label);

            while (open.Count > 0)
            {
                parent = open.Dequeue() as RoomCharacteristicsData;
                Debug.Log("parent: " + parent.Label);

                var childs = graph.GetNeighbors(parent);
                //var childs = graph.GetNeighbors(parent).OrderBy(n => Utility.MathTools.GetAngleD15(parent.Centroid, n.Centroid)).Select( c => c as RoomCharacteristicsData);

                foreach (var child in childs)
                {
                    if (closed.Contains(child) || open.ToHashSet().Contains(child))
                        continue;

                    open.Enqueue(child);

                    var parentH = parent.RangeHeight.Middle; 
                    var parentW = parent.RangeWidth.Middle;
                    var childH = child.RangeHeight.Middle;
                    var childW = child.RangeWidth.Middle;

                    var dir = ((Vector2)(child.Centroid - parent.Centroid)).normalized;
                    var posX = dir.x * ((childW + parentW) / 2f);
                    var posY = dir.y * ((childH + parentH) / 2f);
                    tileMap.AddRoom(new Vector2Int((int)posX, (int)posY), childW, childH, child.Label);
                }

                closed.Add(parent);
            }

            return tileMap;
        }
    }
}
