using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeneticSharp.Domain.Chromosomes;

public interface IChromosomable
{
    public IChromosome ToChromosome();
    public void FromChromosome(IChromosome chromosome);
}
