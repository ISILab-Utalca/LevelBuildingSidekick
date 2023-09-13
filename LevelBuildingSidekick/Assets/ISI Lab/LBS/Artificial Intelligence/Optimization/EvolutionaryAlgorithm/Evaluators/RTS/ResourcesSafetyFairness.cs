using Commons.Optimization.Evaluator;
using LBS.Components.TileMap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ResourcesSafetyFairness : IRangedEvaluator
{
    public float MaxValue => 1;

    public float MinValue => 0;

    [SerializeField, SerializeReference]
    public LBSCharacteristic playerCharacteristc;

    [SerializeField, SerializeReference]
    public List<LBSCharacteristic> resourceCharactersitic = new List<LBSCharacteristic>(); // Could be a list


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
            float res = 0;

            for (int j = -dist; j <= dist; j++)
            {
                for (int i = -dist; i <= dist; i++)
                {
                    var index = chrom.ToIndex(new Vector2(i, j));

                    if (index < 0 || index >= genes.Count)
                    {
                        continue;
                    }

                    if (genes[index].Characteristics.Any(c => resourceCharactersitic.Contains(c)))
                    {
                        res++;
                    }
                }
            }

            localFitness.Add(res);
        }

        fitness = localFitness.Min() / localFitness.Max();

        return fitness;
    }
}
