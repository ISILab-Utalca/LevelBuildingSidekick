using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using UnityEngine.UIElements;

namespace GeneticSharp.Domain.Crossovers
{
    /// <summary>
    /// Cycle Crossover (CX).
    /// <remarks>
    /// The Cycle Crossover (CX) proposed by Oliver builds offspring in such a way that each 
    /// city (and its position) comes from one of the parents.
    /// <see href="http://arxiv.org/ftp/arxiv/papers/1203/1203.3097.pdf">A Comparative Study of Adaptive Crossover Operators for Genetic Algorithms to Resolve the Traveling Salesman Problem</see>
    /// <para>
    /// The Cycle Crossover operator identifies a number of so-called cycles between two parent chromosomes. 
    /// Then, to form Child 1, cycle one is copied from parent 1, cycle 2 from parent 2, cycle 3 from parent 1, and so on.
    /// <see ref="http://www.rubicite.com/Tutorials/GeneticAlgorithms/CrossoverOperators/CycleCrossoverOperator.aspx">Crossover Technique: Cycle Crossover</see>
    /// </para>
    /// </remarks>
    /// </summary>
    [DisplayName("Cycle (CX)")]
    public class CycleCrossover : CrossoverBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Crossovers.CycleCrossover"/> class.
        /// </summary>
        public CycleCrossover()
        {
            ParentsNumber = 2;
            ChildrenNumber = 2;
            IsOrdered = true;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Performs the cross with specified parents generating the children.
        /// </summary>
        /// <param name="parents">The parents chromosomes.</param>
        /// <returns>The offspring (children) of the parents.</returns>
        protected override IList<ChromosomeBase> PerformCross(IList<ChromosomeBase> parents)
        {
            var datas = parents.Select(p => p.GetGenes()).ToList();

            var parent1 = datas[0];
            var parent2 = datas[1];

            if (datas.AnyHasRepeatedValue())
            {
                throw new CrossoverException(this, "The Cycle Crossover (CX) can be only used with ordered chromosomes. The specified chromosome has repeated genes.");
            }

            var cycles = new List<List<int>>();
            var offspring1 = new object[parent1.Length];
            var offspring2 = new object[parent1.Length];

            // Search for the cycles.
            for (int i = 0; i < parent1.Length; i++)
            {
                if (!cycles.SelectMany(p => p).Contains(i))
                {
                    var cycle = new List<int>();
                    CreateCycle(parent1, parent2, i, cycle);
                    cycles.Add(cycle);
                }
            }

            // Copy the cycles to the offpring.
            for (int i = 0; i < cycles.Count; i++)
            {
                var cycle = cycles[i];

                if (i % 2 == 0)
                {
                    // Copy cycle index pair: values from Parent 1 and copied to Child 1, and values from Parent 2 will be copied to Child 2.
                    CopyCycleIndexPair(cycle, parent1, offspring1, parent2, offspring2);
                }
                else
                {
                    // Copy cycle index odd: values from Parent 1 will be copied to Child 2, and values from Parent 1 will be copied to Child 1.
                    CopyCycleIndexPair(cycle, parent1, offspring2, parent2, offspring1);
                }
            }

            var child1 = parents[0].CreateNewChromosome();
            var child2 = parents[0].CreateNewChromosome();

            child1.SetDataSequence(offspring1);
            child2.SetDataSequence(offspring2);

            return new List<ChromosomeBase>() { child1, child2 };
        }

        /// <summary>
        /// Copies the cycle index pair.
        /// </summary>
        /// <param name="cycle">The cycle.</param>
        /// <param name="fromParent1Genes">From parent1 genes.</param>
        /// <param name="toOffspring1">To offspring1.</param>
        /// <param name="fromParent2Genes">From parent2 genes.</param>
        /// <param name="toOffspring2">To offspring2.</param>
        private static void CopyCycleIndexPair(IList<int> cycle, object[] fromParent1Genes, object[] toOffspring1, object[] fromParent2Genes, object[] toOffspring2)
        {
            int geneCycleIndex = 0;

            for (int j = 0; j < cycle.Count; j++)
            {
                geneCycleIndex = cycle[j];
                toOffspring1[geneCycleIndex] = fromParent1Genes[geneCycleIndex];
                toOffspring2[geneCycleIndex] = fromParent2Genes[geneCycleIndex];
            }
        }

        /// <summary>
        /// Creates the cycle recursively.
        /// </summary>
        /// <param name="parent1Genes">The parent one's genes.</param>
        /// <param name="parent2Genes">The parent two's genes.</param>
        /// <param name="geneIndex">The current gene index.</param>
        /// <param name="cycle">The cycle.</param>
        private void CreateCycle(object[] parent1Genes, object[] parent2Genes, int geneIndex, List<int> cycle)
        {
            if (!cycle.Contains(geneIndex))
            {
                var parent2Gene = parent2Genes[geneIndex];
                cycle.Add(geneIndex);
                var newGeneIndex = parent1Genes.Select((g, i) => new { g, Index = i }).First(g => g.Equals(parent2Gene));

                if (geneIndex != newGeneIndex.Index)
                {
                    CreateCycle(parent1Genes, parent2Genes, newGeneIndex.Index, cycle);
                }
            }
        }
        #endregion
    }
}