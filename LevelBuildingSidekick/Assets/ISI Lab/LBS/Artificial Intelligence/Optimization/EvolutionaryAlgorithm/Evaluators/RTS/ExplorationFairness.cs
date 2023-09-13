using Commons.Optimization.Evaluator;
using LBS.Components.TileMap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExplorationFairnes : IRangedEvaluator
{
    public float MaxValue => 1;

    public float MinValue => 0;

    public LBSCharacteristic playerCharacteristc;

    public LBSCharacteristic colliderCharastetistic;

    List<Vector2> directions = new List<Vector2>()
    {
        Vector2.right,
        Vector2.right + Vector2.up,
        Vector2.up,
        Vector2.up + Vector2.left,
        Vector2.left,
        Vector2.left + Vector2.down,
        Vector2.down,
        Vector2.down + Vector2.right
    };

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


        foreach (var pos in playersPos)
        {
            Queue<int> open = new Queue<int>();
            List<int> visited = new List<int>();

            open.Enqueue(pos);
            float walkable = 1;

            while(open.Count >= 0)
            {
                var index = open.Dequeue();
                visited.Add(index);

                foreach(var dir in directions)
                {
                    var cand = chrom.ToIndex(chrom.ToMatrixPosition(index) + dir);

                    if (cand < 0 || cand >= genes.Count || visited.Contains(cand))
                        continue;

                    if (genes[cand].Characteristics.Contains(colliderCharastetistic))
                        continue;

                    walkable++;
                    open.Enqueue(cand);
                }
            }

            localFitness.Add(walkable);
        }

        fitness = localFitness.Min() / localFitness.Max();

        return fitness;
    }

    public object Clone()
    {
        throw new NotImplementedException();
    }
}
