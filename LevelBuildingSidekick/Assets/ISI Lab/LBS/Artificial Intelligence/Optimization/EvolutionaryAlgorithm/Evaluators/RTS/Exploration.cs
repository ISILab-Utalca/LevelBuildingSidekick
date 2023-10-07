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
