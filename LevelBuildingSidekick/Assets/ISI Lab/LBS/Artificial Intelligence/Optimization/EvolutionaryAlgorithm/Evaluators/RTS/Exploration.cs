using Commons.Optimization.Evaluator;
using LBS.Components.TileMap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Exploration : IRangedEvaluator
{
    public float MaxValue => 1;

    public float MinValue => 0;

    [SerializeField, SerializeReference]
    public LBSCharacteristic colliderCharacteristic;

    public float Evaluate(IOptimizable evaluable)
    {
        var chrom = evaluable as BundleTilemapChromosome;

        if (chrom == null)
        {
            throw new Exception("Wrong Chromosome Type");
        }

        float fitness = 0;

        var genes = chrom.GetGenes().ToList();


        foreach (var g in genes)
        {
            if (g == null)
            {
                fitness++;
                continue;
            }
            if (!(g as BundleData).Characteristics.Contains(colliderCharacteristic))
                fitness++;
        }
        /*
        var dir = Directions.Bidimencional.Edges;
        List<int> closed = new List<int>();
        List<int> open = new List<int>();

        int breaks = -1;

        for(int i = 0; i < genes.Count; i++)
        {
            if (closed.Contains(i))
                continue;

            if (genes[i] != null)
            {
                if ((genes[i] as BundleData).Characteristics.Contains(colliderCharacteristic))
                {
                    closed.Add(i);
                    continue;
                }
            }

            open.Add(i);

            while(open.Count > 0)
            {
                var parent = open[0];
                open.RemoveAt(0);
                var pos = chrom.ToMatrixPosition(i);

                foreach(var d in dir)
                {
                    var p = pos + d;
                    var child = chrom.ToIndex(p);

                    if (closed.Contains(child) || open.Contains(child))
                        continue;

                    if(child < 0 || child >= chrom.Length)
                        continue;

                    if (genes[child] != null)
                    {
                        if ((genes[child] as BundleData).Characteristics.Contains(colliderCharacteristic))
                        {
                            closed.Add(child);
                        }
                    }

                    open.Add(child);
                }

                closed.Add(parent);
                fitness++;

            }
            breaks++;
        }*/

        fitness /= (float)genes.Count;

        return fitness;
    }

    public object Clone()
    {
        var e = new Exploration();
        e.colliderCharacteristic = colliderCharacteristic;
        return e;
    }
}
