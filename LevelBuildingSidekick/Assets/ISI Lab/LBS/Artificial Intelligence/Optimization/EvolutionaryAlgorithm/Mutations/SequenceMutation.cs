using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Mutations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceMutation : MutationBase
{

    [SerializeField]
    private List<MutationBase> mutations = new List<MutationBase>();

    protected override void PerformMutate(ChromosomeBase chromosome, float probability)
    {
        foreach(var m in mutations)
        {
            m.Mutate(chromosome, probability);
        }
    }
}
