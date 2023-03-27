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

    protected override IList<IChromosome> PerformCross(IList<IChromosome> parents)
    {
        var offsprings = new List<IChromosome>();

        var parent1 = parents[0] as ChromosomeBase2D;
        var parent2 = parents[1] as ChromosomeBase2D;

        if(parent1 == null || parent2 == null)
        {
            throw new CrossoverException("Parents must be ChromosomeBase2D");
        }
        if (parent1.Length != parent2.Length)
        {
            throw new CrossoverException("Parents must be of same Length");
        }
        if (parent1.MatrixWidth != parent2.MatrixWidth)
        {
            throw new CrossoverException("Parents must be of same Width");
        }

        var r = RandomizationProvider.Current;

        var x = r.GetInt(0, (int)(parent1.MatrixWidth - (crossArea.x * parent1.MatrixWidth)));
        int height = parent1.Length / parent1.MatrixWidth;
        var y = r.GetInt(0, (int)(height - (crossArea.y * height)));

        var offspring1 = parent1.Clone() as ChromosomeBase2D;
        var offspring2 = parent2.Clone() as ChromosomeBase2D;

        for (int j = 0; j < crossArea.y * height; j++)
        {
            for (int i = 0; i < crossArea.y * parent1.MatrixWidth; i++)
            {
                var index = offspring1.ToIndex(new Vector2(x + i, y + j));
                var aux = offspring1.GetGene(index);
                offspring1.ReplaceGene(index, offspring2.GetGene(index));
                offspring2.ReplaceGene(index, aux);
            }
        }

        return new List<IChromosome>() { offspring1, offspring2 };
    }
}
