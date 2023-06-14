using Commons.Optimization.Evaluator;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Dispersion
{
    public float MaxValue => 1;

    public float MinValue => 0;

    int clusterCount = 1;

    public int ClusterCount
    {
        get => clusterCount;
        set => clusterCount = value;
    }

    public float Evaluate(IOptimizable evaluable)
    {/*
        var chr = evaluable as LBSChromosome;
        var genes = chr.GetGenes();
        var points = genes.Select((g, i) => new { g, i }).Where(x => x.g != null).Select(x => (Vector2)chr.ToMatrixPosition(x.i)).ToList();

        var c = points.Count < ClusterCount ? points.Count : ClusterCount;

        var kmeans = new Kmeans();

        kmeans.Init(points, c);

        //var max = ((Vector2)chr.ToMatrixPosition(chr.Length - 1)).Distance(DistanceType.EUCLIDEAN);

        var disp = kmeans.Dispersion();
        var max = kmeans.MaxDispersion(points);

        return MinValue + (MaxValue - MinValue) * (disp.SelectMany(d => d.Select(f => f)).Average()/max);*/
        return 0;
    }
}
