using Commons.Optimization.Evaluator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MaximizeDistance : IRangedEvaluator
{
    public float MaxValue => 1;

    public float MinValue => 0;

    public float LocalMax => 0.75f;

    public float LocalMin => 0.25f;


    public List<Object> whiteList = new List<Object>();

    public DistanceType distType;

    public float Evaluate(IOptimizable evaluable)
    {
        var ev = evaluable as ChromosomeBase2D;

        if(ev == null)
        {
            return MinValue;
        }

        var fitness = 0f;

        foreach(object o in whiteList)
        {
            var genes = ev.GetGenes();
            var indexes = new List<int>();

            for (int i = 0; i < genes.Length; i++)
            {
                if (genes[i] != null && genes[i].Equals(o))
                    indexes.Add(i);
            }


            if (indexes.Count == 0)
            {
                fitness += MinValue;
                continue;
            }

            fitness += avgMin(indexes, ev);
        }

        return MinValue + ((MaxValue - MinValue) * (fitness / whiteList.Count));

    }

    private float avgMin(List<int> indexes, ChromosomeBase2D chr)
    {
        var side = Mathf.Sqrt(indexes.Count);

        var max = ((Vector2)chr.ToMatrixPosition(chr.Length - 1) / side).Distance(distType);

        var avgMin = indexes.Average(i => indexes.Min(j => ((Vector2)(chr.ToMatrixPosition(i) - chr.ToMatrixPosition(j))).Distance(distType)));
        
        float val = avgMin < max ? avgMin : 1;

        return val;
    }
}
