using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;
using UnityEngine.UIElements;

namespace GeneticSharp.Domain.Crossovers
{
    /// <summary>
    /// Partially mapped crossover (PMX).
    /// <remarks>
    /// The partially-mapped crossover operator was suggested by Gold- berg and Lingle (1985). 
    /// It passes on ordering and value information from the parent tours to the offspring tours. 
    /// A portion of one parent’s string is mapped onto a portion of the other parent’s string and the remaining information is exchanged.
    /// <see href="http://www.dca.fee.unicamp.br/~gomide/courses/EA072/artigos/Genetic_Algorithm_TSPR_eview_Larranaga_1999.pdf">Genetic Algorithms for the Travelling Salesman Problem - A Review of Representations and Operators</see>
    /// </remarks>
    /// </summary>
    [DisplayName("Partially Mapped (PMX)")]
    public class PartiallyMappedCrossover : CrossoverBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Crossovers.PartiallyMappedCrossover"/> class.
        /// </summary>
        public PartiallyMappedCrossover()
        {
            ParentsNumber = 2;
            ChildrenNumber = 2;
            MinLength = 3;
            IsOrdered = true;
        }
        #endregion

        #region Methods        
        /// <summary>
        /// Performs the cross with specified parents generating the children.
        /// </summary>
        /// <param name="parents">The parents chromosomes.</param>
        /// <returns>
        /// The offspring (children) of the parents.
        /// </returns>
        protected override IList<ChromosomeBase> PerformCross(IList<ChromosomeBase> parents)
        {
            var datas = parents.Select(p => p.GetGenes()).ToList();

            if (datas.AnyHasRepeatedValue())
            {
                throw new CrossoverException(this, "The Partially Mapped Crossover (PMX) can be only used with ordered chromosomes. The specified chromosome has repeated genes.");
            }

            // Choose the thow parents.
            var parent1 = datas[0];
            var parent2 = datas[1];

            // Create, sort and define the cut point indexes.
            var cutPointsIndexes = RandomizationProvider.Current.GetUniqueInts(2, 0, parent1.Length);
            Array.Sort(cutPointsIndexes);
            var firstCutPointIndex = cutPointsIndexes[0];
            var secondCutPointIndex = cutPointsIndexes[1];

            // Parent1 creates the mapping section.
            var parent1MappingSection = parent1.Skip(firstCutPointIndex).Take((secondCutPointIndex - firstCutPointIndex) + 1).ToArray();

            // Parent12 creates the mapping section.
            var parent2MappingSection = parent2.Skip(firstCutPointIndex).Take((secondCutPointIndex - firstCutPointIndex) + 1).ToArray();

            // The new offsprings are created and 
            // their genes ar replaced start in the first cut point index
            // and using the genes in the mapping section from parent 1 e 2.
            var offspring1 = new object[parent1.Length];
            var offspring2 = new object[parent2.Length];

            Array.Copy(parent1MappingSection, 0, offspring2, firstCutPointIndex, parent1MappingSection.Length);
            Array.Copy(parent2MappingSection, 0, offspring1, firstCutPointIndex, parent2MappingSection.Length);

            var length = parent1.Length;

            for (int i = 0; i < length; i++)
            {
                if (i >= firstCutPointIndex && i <= secondCutPointIndex)
                {
                    continue;
                }

                var geneForOffspring1 = GetGeneNotInMappingSection(parent1[i], parent2MappingSection, parent1MappingSection);
                offspring1[i] = geneForOffspring1;

                var geneForOffspring2 = GetGeneNotInMappingSection(parent2[i], parent1MappingSection, parent2MappingSection);
                offspring2[i] = geneForOffspring2;
            }

            var child1 = parents[0].CreateNewChromosome();
            var child2 = parents[0].CreateNewChromosome();

            child1.SetDataSequence(offspring1);
            child2.SetDataSequence(offspring2);

            return new List<ChromosomeBase>() { child1, child2 };
        }

        private object GetGeneNotInMappingSection(object candidateGene, object[] mappingSection, object[] otherParentMappingSection)
        {
            var indexOnMappingSection = mappingSection
                .Select((item, index) => new { Gene = item, Index = index })
                .FirstOrDefault(g => g.Gene.Equals(candidateGene));

            if (indexOnMappingSection != null)
            {
                return GetGeneNotInMappingSection(otherParentMappingSection[indexOnMappingSection.Index], mappingSection, otherParentMappingSection);
            }

            return candidateGene;
        }
        #endregion
    }
}
