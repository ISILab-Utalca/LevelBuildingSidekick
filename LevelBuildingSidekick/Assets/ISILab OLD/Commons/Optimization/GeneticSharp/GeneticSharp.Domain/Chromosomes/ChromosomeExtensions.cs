using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using GeneticSharp.Infrastructure.Framework.Texts;

namespace GeneticSharp.Domain.Chromosomes
{
    /// <summary>
    /// Chromosome extensions.
    /// </summary>
    public static class ChromosomeExtensions
    {
        /// <summary>
        /// Checks if any of the chromosomes has repeated gene.
        /// </summary>
        /// <remarks>
        /// This can happen when used with a IMutation's implementation that not keep the chromosome ordered, 
        /// like OnePointCrossover, TwoPointCrossover and UniformCrossover is combined with a ICrossover's implementation
        /// that need ordered chromosomes, like OX1 and PMX.
        /// </remarks>
        /// <returns><c>true</c>, if chromosome has repeated gene, <c>false</c> otherwise.</returns>
        /// <param name="objectColections">The chromosomes.</param>
        public static bool AnyHasRepeatedValue(this IList<object[]> objectColections)
        {
            for (int i = 0; i < objectColections.Count; i++)
            {
                var c = objectColections[i];
                var notRepeatedGenesLength = c.Distinct().Count();

                if (notRepeatedGenesLength < c.Length)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Validates the chromosomes.
        /// </summary>
        /// <param name="chromosomes">The chromosomes.</param>
        public static void ValidateGenes(this IList<IChromosome> chromosomes)
        {
            if (chromosomes.Any(c => c.GetGenes().Any(g => g is INullable && g == null)))
            {
                throw new InvalidOperationException("The chromosome '{0}' is generating genes with null value.".With(chromosomes.First().GetType().Name));
            }
        }

        /// <summary>
        /// Validates the chromosome.
        /// </summary>
        /// <param name="chromosome">The chromosomes.</param>
        public static bool ValidateGenes(this IChromosome chromosome)
        {
            if(chromosome != null && chromosome.GetGenes() == null)
            {
                throw new InvalidOperationException("Genes are null");
            }

            if (chromosome != null && chromosome.GetGenes().Any(g => g == null))
            {
                throw new InvalidOperationException("The chromosome '{0}' is generating genes with null value.".With(chromosome.GetType().Name));
            }
            return true;
        }
    }
}