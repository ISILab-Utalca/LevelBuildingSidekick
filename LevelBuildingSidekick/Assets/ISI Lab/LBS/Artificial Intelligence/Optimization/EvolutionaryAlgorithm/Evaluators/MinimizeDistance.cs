using Commons.Optimization.Evaluator;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MinimizeDistance : IRangedEvaluator
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

        if (ev == null)
        {
            return MinValue;
        }

        var fitness = 0f;

        foreach (object o in whiteList)
        {
            var genes = ev.GetGenes();
            var indexes = new List<int>();

            for(int i = 0; i < genes.Length; i++)
            {
                if (genes[i] != null && genes[i].Equals(o))
                    indexes.Add(i);
            }


            if(indexes.Count == 0)
            {
                fitness += MinValue;
                continue;
            }


            fitness += avgMin(indexes, ev);
        }

        return MaxValue - fitness / whiteList.Count;

    }

    private float avgMin(List<int> indexes, ChromosomeBase2D chr)
    {

        var absMax = Distance(distType, new Vector2Int((int)chr.Rect.size.x, (int)chr.Rect.size.y));

        var avgMin = 0f;

        foreach (var i in indexes)
        {
            var min = absMax;
            foreach (var j in indexes)
            {
                if (i != j)
                {
                    var d = Distance(distType, chr.ToMatrixPosition(i) - chr.ToMatrixPosition(j));
                    if (min > d)
                        min = d;
                }
            }
            avgMin += min;
        }

        return MinValue + MaxValue * (avgMin / (absMax * indexes.Count));
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
