using System.Collections.Generic;
using Commons.Optimization.Evaluator;
using ISILab.AI.Optimization;
using ISILab.AI.Wrappers;
using ISILab.LBS.Components;
using ISILab.LBS.Modules;
using LBS.Components;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.AI.Optimization
{
    public class AreasEvaluator : IEvaluator
    {
        private LBSLayer _original;

        public string Tooltip => "Areas Evaluator";

        public AreasEvaluator(LBSLayer layer)
        {
            this._original = layer;
        }

        private float EvaluateBySize(List<LBSModule> modules, Zone zone)
        {
            var zones = modules.GetModule<SectorizedTileMapModule>();
            var constrs = modules.GetModule<ConstrainsZonesModule>();

            var limit = constrs.GetLimits(zone);
            if (limit == null)
                return 0;

            var bound = zones.GetBounds(zone);
            if (bound.width == 0 || bound.height == 0)
                return 0;

            var vw = 1f;
            if (bound.width > limit.maxWidth || bound.width < limit.minWidth)
            {
                vw = bound.width / (float)limit.WidthMid;
                if (vw > 1)
                    vw = 1 / vw;
            }

            var vh = 1f;
            if (bound.height > limit.maxHeight || bound.height < limit.minHeight)
            {
                vh = bound.height / (float)limit.WidthMid;
                if (vh > 1)
                    vh = 1 / vh;
            }

            return (vw + vh) / 2f;
        }

        public float Evaluate(IOptimizable evaluable)
        {
            //Debug.Log(Tooltip);

            var modules = (evaluable as OptimizableModules).Modules;

            var zones = _original.GetModule<SectorizedTileMapModule>();
            //var connected = modules.GetModule<ConnectedZonesModule>();

            float value = 0f;

            List<Zone> zonesWithTiles = zones.ZonesWithTiles;

            if (zonesWithTiles.Count <= 0)
            {
                return 0;
            }

            for (int i = 0; i < zonesWithTiles.Count; i++)
            {
                Zone zone = zonesWithTiles[i];

                value += EvaluateBySize(modules, zone);
            }

            return value / (zonesWithTiles.Count * 1f);
        }

        public object Clone()
        {
            throw new System.NotImplementedException(); // TODO: Implement clone method
        }

        public void InitializeDefault()
        {
            throw new System.NotImplementedException();
        }
    }
}