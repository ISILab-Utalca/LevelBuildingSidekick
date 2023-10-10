using Commons.Optimization.Evaluator;
using LBS.Components;
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
    LBSLayer original;

    private readonly List<Vector2Int> dirs = Directions.Bidimencional.Edges;

    Vector2 delta;

    public RoomCutEvaluator(LBSLayer layer)
    {
        this.original = layer;
    }

    public float Evaluate(IOptimizable evaluable)
    {
        var layer = (evaluable as OptimizableModules).Modules;
        var zones = layer.GetModule<SectorizedTileMapModule>();
        var tilesModule = layer.GetModule<TileMapModule>();

        var value = 0f;
        for (int i = 0; i < zones.ZonesWithTiles.Count; i++)
        {
            var zone = zones.ZonesWithTiles[i];

            var tiles = zones.GetTiles(zone);
            var check = new List<LBSTile>();
            var uncheck = new List<LBSTile>();

            if (tiles.Count <= 0)
                continue;

            uncheck.Add(tiles[0]);

            do
            {
                var current = uncheck.First();
                var neis = tilesModule.GetTileNeighbors(current,dirs); //room.Tiles;// GetTileNeighbors(current, dirs);
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

        if (zones.ZonesWithTiles.Count <= 0)
            return 0;

        return value / (float)zones.ZonesWithTiles.Count;

    }

    public object Clone()
    {
        throw new System.NotImplementedException();
    }
}
