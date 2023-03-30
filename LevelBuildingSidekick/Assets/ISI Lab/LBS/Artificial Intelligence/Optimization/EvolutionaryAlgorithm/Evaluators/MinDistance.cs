using Commons.Optimization.Evaluator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MinDistance : IRangedEvaluator
{
    public float MaxValue => 1;

    public float MinValue => 0;

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
            var candidates = ev.GetGenes().Where(e => e.Equals(o));
            if (candidates.Count() == 0)
            {
                fitness += MinValue;
                continue;
            }

            var indexes = candidates.Select((e, i) => i).ToList();

            fitness += avgMin(indexes, ev);
        }

        return fitness / whiteList.Count;

    }

    private float avgMin(List<int> indexes, ChromosomeBase2D chr)
    {
        var dist = 2f;
        switch(distType)
        {
            case DistanceType.MANHATTAN: dist = 2; break;
            case DistanceType.EUCLIDEAN: dist = 1.4f; break;
            case DistanceType.CHESS: dist = 1; break;
        }

        var max = Distance(dist, chr.ToMatrixPosition(chr.Length - 1));

        var avgMin = 0f;

        foreach(var i in indexes)
        {
            var min = max;
            foreach(var j in indexes)
            {
                if(i != j)
                {
                    var d = Distance(dist, chr.ToMatrixPosition(i) - chr.ToMatrixPosition(j));
                    if (min > d)
                        min = d;
                }
            }
            avgMin += min;
        }

        return MinValue + MaxValue * (avgMin / max * indexes.Count);
    }

    public float Distance(float diag, Vector2Int point)
    {
        var min = Mathf.Abs(point.x) < Mathf.Abs(point.y) ? Mathf.Abs(point.x) : Mathf.Abs(point.y);
        var max = Mathf.Abs(point.x) > Mathf.Abs(point.y) ? Mathf.Abs(point.x) : Mathf.Abs(point.y);

        return (max - min) + min * diag;
    }
}

public enum DistanceType
{
    MANHATTAN,
    EUCLIDEAN,
    CHESS
}