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

        return schema.Areas.Average((r) => 
        { 
            var rect = r.GetBounds(); 
            return r.TileCount / (float)(rect.width * rect.height); 
        });
    }

}