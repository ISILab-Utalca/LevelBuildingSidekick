using Commons.Optimization.Evaluator;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MinimizeDistance : IRangedEvaluator
{
    public float MaxValue => 1;

    public float MinValue => 0;

    public List<Object> whiteList = new List<Object>();

    public DistanceType distType;

    public float Evaluate(IOptimizable evaluable)
    {
        var ev = evaluable as ChromosomeBase2D;

        if (ev == null)
        {
            return MinValue;
        }

        var fitness = 0f;

        foreach (object o in whiteList)
        {
            if (o == null)
                continue;
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

        var max = ((Vector2)chr.ToMatrixPosition(chr.Length - 1)).Distance(distType);

        if (indexes.Count <= 1)
            return 0;

        var avgMin = indexes.Average(i => indexes.Where(j => j != i).Min(j => ((Vector2)(chr.ToMatrixPosition(i) - chr.ToMatrixPosition(j))).Distance(distType)));

        avgMin = (max - avgMin)/max;

        float val = avgMin < 1 ? avgMin : 1;

        return val;
    }

    public object Clone()
    {
        throw new System.NotImplementedException();
    }
}
