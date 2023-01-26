using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.Representation.TileMap;
using LBS.Graph;
using System.Linq;
using LBS.Schema;

namespace LBS.Transformers
{
    public class GraphToSchema : TransformerOld<GraphicsModule, LBSSchemaData>
    {
        public override LBSSchemaData Transform(GraphicsModule graph)
        {
            if (graph.NodeCount() <= 0)
            {
                Debug.LogWarning("[Error]: 'Graph node' have 0 nodes.");
                return null;
            }

            Queue<LBSNodeDataOld> open = new Queue<LBSNodeDataOld>();
            HashSet<LBSNodeDataOld> closed = new HashSet<LBSNodeDataOld>();

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

                    var posX = (dir.x * ((childW + parentW) / 2f) * Mathf.Sqrt(2)) + pPos.x;
                    var posY = (dir.y * ((childH + parentH) / 2f) * Mathf.Sqrt(2)) + pPos.y;

                    if ((childW == 1 || childW == 2) && ((childH == 1 || childH == 2)))
                    {
                        posX = Mathf.Round(posX);
                        posY = Mathf.Round(posY);
                    }

                    schema.AddRoom(new Vector2Int((int)posX, (int)posY), childW, childH, child.Label);

                    if(schema.CheckTilesRooms()) { schema.RepositionRooms();}
                }
                
                closed.Add(parent);                              
            }

            schema.RecalculateTilePos();
            return schema;
        }
    }
}
