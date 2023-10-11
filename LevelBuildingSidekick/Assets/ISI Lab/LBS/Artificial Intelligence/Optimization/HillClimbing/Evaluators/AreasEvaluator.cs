using Commons.Optimization.Evaluator;
using LBS.Components;
using LBS.Components.Graph;
using LBS.Components.Specifics;
using LBS.Components.TileMap;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class AreasEvaluator : IEvaluator
{
    private LBSLayer original;

    Vector2 delta;

    public AreasEvaluator(LBSLayer layer) 
    {
        this.original = layer;
    }

    private float EvaluateBySize(List<LBSModule> modules, Zone zone)
    {
        var zones = modules.GetModule<SectorizedTileMapModule>();
        var constrs = modules.GetModule<ConstrainsZonesModule>();

        var bound = zones.GetBounds(zone);
        var limit = constrs.GetLimits(zone);

        if (bound.width == 0 || bound.height == 0)
            return 0;
        if (limit == null)
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
        var modules = (evaluable as OptimizableModules).Modules;

        var zones = modules.GetModule<SectorizedTileMapModule>();
        var connected = modules.GetModule<ConnectedZonesModule>();

        var value = 0f;


        for (int i = 0; i < zones.ZonesWithTiles.Count; i++)
        {
            var zone = zones.ZonesWithTiles[i];

            value += EvaluateBySize(modules, zone);
        }

        if (zones.ZonesWithTiles.Count <= 0)
            return 0;

        return value / (zones.ZonesWithTiles.Count * 1f);
    }

    public object Clone()
    {
        throw new System.NotImplementedException();
    }
}
