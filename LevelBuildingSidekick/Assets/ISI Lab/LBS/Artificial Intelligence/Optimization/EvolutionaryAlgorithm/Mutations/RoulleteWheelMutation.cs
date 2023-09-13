using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Randomizations;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoulleteWheelMutation : MutationBase
{
    [SerializeField, SerializeReference]
    public List<WeightedMutation> mutations = new List<WeightedMutation>();


    public RoulleteWheelMutation()
    {

    }

    public RoulleteWheelMutation(List<WeightedMutation> mutations)
    {
        this.mutations = mutations;
    }

    protected override void PerformMutate(ChromosomeBase chromosome, float probability)
    {
        var r = RandomizationProvider.Current;

        var i = r.GetFloat(0, mutations.Sum(m => m.weight));

        foreach(var t in mutations)
        {
            i -= t.weight;
            if(i < 0)
            {
                t.mutation.Mutate(chromosome, probability);
            }
        }
    }
}

[System.Serializable]
public class WeightedMutation
{
    [SerializeField, SerializeReference]
    public MutationBase mutation;
    [SerializeField]
    public float weight = 1;

    public WeightedMutation() { }
    public WeightedMutation(MutationBase mutation, float weight) 
    {
        this.mutation = mutation;
        this.weight = weight;
    }

}
