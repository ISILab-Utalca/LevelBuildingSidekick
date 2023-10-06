using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Randomizations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AreaCrossover : CrossoverBase
{
    [Range(0,1)]
    private Vector2 crossArea = 0.5f*Vector2.one;

    public AreaCrossover()
    {
        ParentsNumber = 2;
        ChildrenNumber = 2;
    }

    protected override IList<ChromosomeBase> PerformCross(IList<ChromosomeBase> parents)
    {
        var offsprings = new List<IChromosome>();

        var parent1 = parents[0] as ChromosomeBase2D;
        var parent2 = parents[1] as ChromosomeBase2D;

        if(parent1 == null || parent2 == null)
        {
            throw new CrossoverException("Parents must be ChromosomeBase2D");
        }
        if (parent1.Rect.size != parent2.Rect.size)
        {
            throw new CrossoverException("Parents must be of same Size");
        }

        var r = RandomizationProvider.Current;

        var rect = parent1.Rect;

        var w = (int)(crossArea.x * rect.width);
        var h = (int)(crossArea.y * rect.height);

        var x = r.GetInt(w, (int)(rect.width)) - w;
        var y = r.GetInt(h, (int)(rect.height)) - h;

        var offspring1 = parent1.Clone() as ChromosomeBase2D;
        var offspring2 = parent2.Clone() as ChromosomeBase2D;

        for (int j = 0; j < h; j++)
        {
            for (int i = 0; i < w; i++)
            {
                var index = offspring1.ToIndex(new Vector2(x + i, y + j));
                if (parent1.IsImmutable(index))
                    continue;
                var aux = offspring1.GetGene(index);
                offspring1.ReplaceGene(index, offspring2.GetGene(index));
                offspring2.ReplaceGene(index, aux);
            }
        }

        return new List<ChromosomeBase>() { offspring1, offspring2 };
    }
}
