using System.Collections.Generic;
using System.Linq;
using Commons.Optimization.Evaluator;
using ISILab.AI.Wrappers;
using ISILab.Commons;
using ISILab.LBS.Modules;
using LBS.Components;
using LBS.Components.TileMap;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.AI.Optimization
{
    public class RoomCutEvaluator : IEvaluator
    {
        LBSLayer original;

        private readonly List<Vector2Int> dirs = Directions.Bidimencional.Edges;

        Vector2 delta;

        public string Tooltip => "Room Cut Evaluator";

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
                var uncheck = new Queue<LBSTile>();

                if (tiles.Count <= 0)
                    continue;

                uncheck.Enqueue(tiles[0]);

                var clock = new System.Diagnostics.Stopwatch();
                clock.Start();

                var x = 0;
                do
                {
                    var current = uncheck.First();
                    var neis = tilesModule.GetTileNeighbors(current, dirs);
                    foreach (var nei in neis)
                    {
                        if (nei == null)
                            continue;

                        if (!check.Contains(nei) && !uncheck.Contains(nei))
                        {
                            uncheck.Enqueue(nei);
                        }
                    }
                    uncheck.Dequeue();
                    check.Add(current);
                    x++;
                }
                while (uncheck.Count > 0);

                clock.Stop();
                Debug.Log("B," + i + ": " + clock.ElapsedMilliseconds / 1000f + "s. (" + x + ")");

                value = (tiles.Count > check.Count) ? 0 : 1;
            }

            if (zones.ZonesWithTiles.Count <= 0)
                return 0;

            return value / (float)zones.ZonesWithTiles.Count;

        }

        public object Clone()
        {
            throw new System.NotImplementedException(); // TODO: Implement Clone method 
        }

        public void InitializeDefault()
        {
            throw new System.NotImplementedException();
        }
    }
}