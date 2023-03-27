using Commons.Optimization.Evaluator;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MaxDistance : IRangedEvaluator
{
    public float MaxValue => 1;

    public float MinValue => 0;

    public List<object> whiteList = new List<object>();

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
            var indexes = ev.GetGenes().Where(e => e.Equals(o)).Select((e, i) => i).ToList();

            fitness += avgMax(indexes, ev);
        }

        return MaxValue - fitness / whiteList.Count;

    }

    private float avgMax(List<int> indexes, ChromosomeBase2D chr)
    {

        var absMax = Distance(distType, chr.ToMatrixPosition(chr.Length - 1));

        var avgMax = 0f;

        foreach (var i in indexes)
        {
            var max = 0f;
            foreach (var j in indexes)
            {
                if (i != j)
                {
                    var d = Distance(distType, chr.ToMatrixPosition(i) - chr.ToMatrixPosition(j));
                    if (max < d)
                        max = d;
                }
            }
            avgMax += max;
        }

        return MinValue + MaxValue * (avgMax / absMax * indexes.Count);
    }

    public float Distance(DistanceType diag, Vector2Int point)
    {

        var dist = 2f;
        switch (distType)
        {
            case DistanceType.MANHATTAN: dist = 2; break;
            case DistanceType.EUCLIDEAN: dist = 1.4f; break;
            case DistanceType.CHESS: dist = 1; break;
        }

        var min = Mathf.Abs(point.x) < Mathf.Abs(point.y) ? Mathf.Abs(point.x) : Mathf.Abs(point.y);
        var max = Mathf.Abs(point.x) > Mathf.Abs(point.y) ? Mathf.Abs(point.x) : Mathf.Abs(point.y);

        return (max - min) + min * dist;
    }
}
