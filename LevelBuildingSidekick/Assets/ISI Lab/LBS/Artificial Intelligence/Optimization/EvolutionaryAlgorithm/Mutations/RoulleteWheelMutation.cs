using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Randomizations;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class RoulleteWheelMutation : MutationBase
{
    [SerializeField]
    private List<Tuple<MutationBase, float>> mutations = new List<Tuple<MutationBase, float>>();

    public RoulleteWheelMutation()
    {

    }

    public RoulleteWheelMutation(List<Tuple<MutationBase, float>> mutations)
    {
        this.mutations = mutations;
    }

    protected override void PerformMutate(ChromosomeBase chromosome, float probability)
    {
        var r = RandomizationProvider.Current;

        var i = r.GetFloat(0, mutations.Sum(m => m.Item2));

        foreach(var t in mutations)
        {
            i -= t.Item2;
            if(i < 0)
            {
                t.Item1.Mutate(chromosome, probability);
            }
        }
    }
}
