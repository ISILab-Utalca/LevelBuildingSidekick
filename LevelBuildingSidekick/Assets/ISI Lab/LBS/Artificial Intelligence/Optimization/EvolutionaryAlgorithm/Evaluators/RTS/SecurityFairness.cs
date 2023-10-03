using Commons.Optimization.Evaluator;
using LBS.Components.TileMap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.PlayerSettings;

[System.Serializable]
public class SecurityFairness : IRangedEvaluator
{

    public float MaxValue => 1;

    public float MinValue => 0;

    [SerializeField, SerializeReference]
    public LBSCharacteristic playerCharacteristic;

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

        var genes = chrom.GetGenes().Cast<BundleData>().ToList();

        var players = genes.Select((g, i) => new { g, i }).Where(p => p.g != null && p.g.Characteristics.Any(c => c.Equals(playerCharacteristic)));

        if (players.Count() < 2)
        {
            Debug.LogWarning("Map is not suitable for the evaluation, it must have at least 2 players");
            return MaxValue;
        }

        var playersPos = players.Select(x => x.i);


        List<float> localFitness = new List<float>();

        //Should use A* between all players

        foreach (var p1 in playersPos)
        {
            foreach (var p2 in playersPos)
            {
                if (p1 == p2)
                    continue;

                var dist = AstarDistance(p1, p2, chrom);

                if(dist < 0)
                {
                    localFitness.Add(chrom.Length);
                }

                localFitness.Add(dist);
            }
        }

        if(localFitness.Max() <= 0)
            return MaxValue;

        fitness = localFitness.Min() / localFitness.Max();

        return fitness;
    }

    public object Clone()
    {
        var e = new SecurityFairness();
        e.playerCharacteristic = playerCharacteristic;
        e.colliderCharacteristic = colliderCharacteristic;
        return e;
    }

    public float AstarDistance(int first, int second, BundleTilemapChromosome chrom)
    {
        var open = new Queue<int>(); 
        var closed = new Dictionary<int, int>();
        var openDic = new Dictionary<int, int>();

        open.Enqueue(first);
        openDic.Add(first, 0);

        while(open.Count > 0)
        {
            var parent = open.Dequeue();
            var g = openDic[parent];
            openDic.Remove(parent);

            if(parent == second)
                return g;// should be pathLength

            foreach (var dir in Directions.Bidimencional.Edges)
            {
                var pos = chrom.ToMatrixPosition(parent) + dir;
                var index = chrom.ToIndex(pos);

                if (index < 0 || index >= chrom.Length)
                    continue;

                var gen = chrom.GetGene(index) as BundleData;

                if (gen.Characteristics.Contains(colliderCharacteristic))
                    continue;

                if (closed.ContainsKey(index))
                    if(closed[index] < openDic[parent] + 1)
                        continue;
                    else
                        closed.Remove(index);


                if (openDic.ContainsKey(index))
                    if (openDic[index] < openDic[parent] + 1)
                        continue;
                    else
                        openDic.Remove(index);

                open.Enqueue(index);
                openDic.Add(index, parent + 1);

            }

            closed.Add(parent, g);
            open.OrderBy(x => openDic[x] + FlatDistance(x, second, chrom));
        }


        return -1;
    }

    public float FlatDistance(int first, int second, BundleTilemapChromosome chrom)
    {
        return (chrom.ToMatrixPosition(first) - chrom.ToMatrixPosition(second)).magnitude;
    }
}
