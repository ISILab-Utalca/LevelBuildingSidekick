using Commons.Optimization.Evaluator;
using ISILab.AI.Optimization;
using ISILab.AI.Wrappers;
using ISILab.Extensions;
using ISILab.LBS.Modules;
using LBS.Components;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.AI.Optimization
{
    public class EmptySpaceEvaluator : IEvaluator
    {
        private LBSLayer original;

        public EmptySpaceEvaluator(LBSLayer layer)
        {
            this.original = layer;
        }

        public object Clone()
        {
            throw new System.NotImplementedException(); // TODO: Implement this
        }

        public float Evaluate(IOptimizable evaluable)
        {
            var layer = (evaluable as OptimizableModules).Modules;
            var zones = layer.GetModule<SectorizedTileMapModule>();

            if (zones.ZonesWithTiles.Count <= 0)
            {
                return 0;
            }

            var avg = zones.ZonesWithTiles.Average((z) =>
            {
                var tiles = zones.GetTiles(z);
                var rect = tiles.GetBounds();
                if (rect.width <= 0 || rect.height <= 0)
                    return float.NegativeInfinity;

                return tiles.Count / (float)(rect.width * rect.height);
            });
            return avg;
        }
    }
}