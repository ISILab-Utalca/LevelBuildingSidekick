using Commons.Optimization.Evaluator;
using LBS.Components.TileMap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SafeAreaFairness : IRangedEvaluator
{

    public float MaxValue => 1;

    public float MinValue => 0;

    [SerializeField, SerializeReference]
    public LBSCharacteristic playerCharacteristc;


    public float Evaluate(IOptimizable evaluable)
    {
        var chrom = evaluable as BundleTilemapChromosome;

        if (chrom == null)
        {
            throw new Exception("Wrong Chromosome Type");
        }

        float fitness = 0;

        var genes = chrom.GetGenes().Cast<BundleData>().ToList();

        var players = genes.Select((g, i) => new { g, i }).Where(p => p.g.Characteristics.Any(c => c.Equals(playerCharacteristc)));

        if (players.Count() < 2)
        {
            Debug.LogWarning("Map is not suitable for the evaluation, it must have at least 2 players");
        }

        var playersPos = players.Select(x => x.i);


        List<float> localFitness = new List<float>();

        //Should use voronoi

        foreach (var pos in playersPos)
        {
            var dist = (int)playersPos.Where(p => p != pos).Min(p => (chrom.ToMatrixPosition(p) - chrom.ToMatrixPosition(pos)).magnitude);

            localFitness.Add(dist);
        }

        fitness = localFitness.Min() / localFitness.Max();

        return fitness;
    }

    public object Clone()
    {
        var e = new SafeAreaFairness();
        e.playerCharacteristc = playerCharacteristc;
        return e;
    }
}
