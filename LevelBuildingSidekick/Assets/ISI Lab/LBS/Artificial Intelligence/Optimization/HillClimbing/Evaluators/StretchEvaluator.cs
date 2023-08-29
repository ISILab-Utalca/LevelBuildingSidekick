using Commons.Optimization.Evaluator;
using LBS.Components.TileMap;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
public class StretchEvaluator : IEvaluator
{
    public float Evaluate(IOptimizable evaluable)
    {
        var schema = (evaluable as OptimizableModules).Modules;

        TiledArea totalArea = new TiledArea();
        var borderMax = 0f;
        foreach (var area in schema.Areas)
        {
            foreach(var t in area.Tiles)
                totalArea.AddTile(t);

            var areaWalls = area.GetHorizontalWalls().Concat(area.GetVerticalWalls()).ToList();
            foreach (var wall in areaWalls)
            {
                borderMax += wall.Length;
            }
            borderMax -= area.GetConcaveCorners().Count() * 2;
        }

        var walls = totalArea.GetHorizontalWalls().Concat(totalArea.GetVerticalWalls()).ToList();

        var borderSize = 0f;
        foreach (var wall in walls)
        {
            borderSize += wall.Length;
        }
        borderSize -= totalArea.GetConcaveCorners().Count() * 2;

        return 1 - (borderSize/borderMax);
    }

}
*/
