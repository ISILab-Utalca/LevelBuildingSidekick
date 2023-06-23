using Commons.Optimization.Evaluator;
using LBS.Components.Graph;
using LBS.Components.Specifics;
using LBS.Components.TileMap;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class RoomCutEvaluator : IEvaluator
{
    private readonly List<Vector2Int> dirs = new List<Vector2Int>()
    {
        Vector2Int.up,
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left
    };

    LBSRoomGraph graph;

    Vector2 delta;

    public RoomCutEvaluator() { }

    public RoomCutEvaluator(LBSRoomGraph graph)
    {
        this.graph = graph;
    }

    public float Evaluate(IOptimizable evaluable)
    {
        var schema = (evaluable as OptimizableSchema).Schema;
        var value = 0f;
        for (int i = 0; i < graph.NodeCount; i++)
        {
            var node = graph.GetNode(i);
            var room = schema.GetArea(node.ID);

            var tiles = room.Tiles;
            var check = new List<LBSTile>();
            var uncheck = new List<LBSTile>();

            if (tiles.Count <= 0)
                continue;

            uncheck.Add(tiles[0]);

            do
            {
                var current = uncheck.First();
                var neis = room.GetTileNeighbors(current, dirs);
                foreach (var nei in neis)
                {
                    if (nei == null)
                        continue;

                    if(!check.Contains(nei) && !uncheck.Contains(nei))
                    {
                        uncheck.Add(nei);
                    }
                }
                uncheck.Remove(current);
                check.Add(current);
            }
            while (uncheck.Count > 0);

            value = (tiles.Count > check.Count) ? 0 : 1;
        }

        return value / (float)graph.NodeCount;

    }



}
