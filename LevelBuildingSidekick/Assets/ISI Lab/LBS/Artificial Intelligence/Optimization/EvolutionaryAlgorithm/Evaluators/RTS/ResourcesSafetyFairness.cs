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

        List<int> playersPos = new();

        for(int i = 0; i < genes.Count; i++)
        {
            if (genes[i] != null)
            {
                if (genes[i].Characteristics.Contains(playerCharacteristc))
                {
                    playersPos.Add(i);
                }
                else
                {
                    foreach (var c in genes[i].Characteristics)
                        Debug.Log(c.Label);
                }
            }
        }


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

                    if (index < 0 || index >= genes.Count || genes[index] == null)
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

    public object Clone()
    { 
        var e = new ResourcesSafetyFairness();
        e.playerCharacteristc = playerCharacteristc;
        e.resourceCharactersitic = new List<LBSCharacteristic>(resourceCharactersitic);
        return e;
    }
}
