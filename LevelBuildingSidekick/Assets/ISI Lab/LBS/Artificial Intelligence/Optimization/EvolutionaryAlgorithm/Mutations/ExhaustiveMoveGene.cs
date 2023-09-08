using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Randomizations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ExhaustiveMoveGene : MutationBase
{
    int range = 1;
    public int Range
    {
        get => range;
        set
        {
            if (value <= 1)
                range = 1;
            range = value;
        }
    }
    public List<object> blackList = new List<object>();

    public ExhaustiveMoveGene()
    {
        this.range = 1;
    }

    public ExhaustiveMoveGene(int range)
    {
        this.range = range;
    }

    protected override void PerformMutate(ChromosomeBase chromosome, float probability)
    {
        var r = RandomizationProvider.Current;

        var chr = chromosome as ChromosomeBase2D;

        if (chr == null)
        {
            throw new Exception("Chromosome must inherit from ChromosomeBase2D");
        }

        for (int i = 0; i < chr.Length; i++)
        {
            if (chromosome.IsImmutable(i))
                continue;
            if (!blackList.Contains(chromosome.GetGene(i)))
                continue;
            if (chr.GetGene(i) != default)
            {
                var d = r.GetDouble();
                if (d < probability)
                {
                    var pos = new Vector2Int(r.GetInt(-range, range), r.GetInt(-range, range));
                    var j = i + chr.ToIndex(pos);
                    if (j < chr.Length && j >= 0)
                    {
                        if (chromosome.IsImmutable(j))
                            continue;

                        var aux = chr.GetGene(i);
                        chr.ReplaceGene(j, chr.GetGene(i));
                        chr.SetDeafult(i);
                    }
                }
            }
        }
    }
}
