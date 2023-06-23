using Commons.Optimization.Evaluator;
using LBS.Components.Graph;
using LBS.Components.Specifics;
using LBS.Components.TileMap;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class EmptySpaceEvaluator : IEvaluator
{

    public EmptySpaceEvaluator() { }

    public float Evaluate(IOptimizable evaluable)
    {
        var schema = (evaluable as OptimizableSchema).Schema;

        if(schema.AreaCount <= 0)
        {
            return 0;
        }

        /*var value = 0f;
        foreach (var room in schema.Areas)
        {
            var rect = room.GetBounds();
            var rectArea = rect.width * rect.height;
            var tc = room.TileCount;
            //value += 1 - (MathF.Abs(rectArea - tc) / (tc * 1f));
            value += tc / rectArea;
        }

        return value / (schema.AreaCount * 1f);*/

        return schema.Areas.Average((r) => 
        { 
            var rect = r.GetBounds(); 
            return r.TileCount / (float)(rect.width * rect.height); 
        });
    }

}