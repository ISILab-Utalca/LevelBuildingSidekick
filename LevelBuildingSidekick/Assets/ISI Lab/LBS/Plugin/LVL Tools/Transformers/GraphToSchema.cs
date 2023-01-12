using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.Representation.TileMap;
using LBS.Graph;
using System.Linq;
using LBS.Schema;

namespace LBS.Transformers
{
    public class GraphToSchema : TransformerOld<LBSGraphData, LBSSchemaData>
    {
        public override LBSSchemaData Transform(LBSGraphData graph)
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

            var schema = new LBSSchemaData();
            //int h = parent.RangeHeight.max;
            //int w = parent.RangeWidth.max;
            int h = Random.Range(parent.RangeHeight.min, parent.RangeHeight.max);
            int w = Random.Range(parent.RangeWidth.min, parent.RangeWidth.max);
            schema.AddRoom(Vector2Int.zero, w, h, parent.Label);

            while (open.Count > 0)
            {
                parent = open.Dequeue() as RoomCharacteristicsData;

                var childs = graph.GetNeighbors(parent);

                foreach (var child in childs)
                {
                    if (closed.Contains(child) || open.ToHashSet().Contains(child))
                        continue;

                    open.Enqueue(child);

                    var parentH = Random.Range(parent.RangeHeight.min, parent.RangeHeight.max); 
                    var parentW = Random.Range(parent.RangeWidth.min, parent.RangeWidth.max);
                    var childH = Random.Range(child.RangeHeight.min, child.RangeHeight.max);
                    var childW = Random.Range(child.RangeWidth.min, child.RangeWidth.max);

                    var pSchema = schema.GetRoom(parent.Label);
                    var pPos = (pSchema != null) ? pSchema.Centroid : Vector2Int.zero;

                    var dir = -((Vector2)(parent.Centroid - child.Centroid)).normalized;
                    var posX = (dir.x * ((childW + parentW) / 2f) * 1.41f) + pPos.x;
                    var posY = (dir.y * ((childH + parentH) / 2f) * 1.41f) + pPos.y;
                    schema.AddRoom(new Vector2Int((int)posX, (int)posY), childW, childH, child.Label);
                }

                closed.Add(parent);
            }

            schema.RecalculateTilePos();
            return schema;
        }
    }
}
