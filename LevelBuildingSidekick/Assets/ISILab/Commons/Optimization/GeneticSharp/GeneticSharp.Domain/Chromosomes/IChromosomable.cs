using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeneticSharp.Domain.Chromosomes;

public interface IChromosomable
{
    public IChromosome ToChromosome();

    /// <summary>
    /// Initializes an object from a chromosome.
    /// </summary>
    /// <param name="chromosome">The chromosome to use for initialization.</param>
    public void FromChromosome(IChromosome chromosome);
}
