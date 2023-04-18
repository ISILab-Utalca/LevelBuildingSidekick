using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Infrastructure.Framework.Texts;
using GeneticSharp.Infrastructure.Framework.Commons;
using UnityEngine.UIElements;
using Utility;

namespace GeneticSharp.Domain.Mutations
{
    /// <summary>
    /// This operator replaces the value of the chosen gene with a uniform random value selected 
    /// between the user-specified upper and lower bounds for that gene. 
    /// <see href="http://en.wikipedia.org/wiki/Mutation_(genetic_algorithm)">Wikipedia</see>
    /// </summary>
    [DisplayName("Uniform")]
    public class UniformMutation : MutationBase
    {
        #region Fields
        private int[] m_mutableGenesIndexes;

        private bool m_allGenesMutable;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Mutations.UniformMutation"/> class.
        /// </summary>
        /// <param name="mutableGenesIndexes">Mutable genes indexes.</param>
        public UniformMutation(params int[] mutableGenesIndexes)
        {
            m_mutableGenesIndexes = mutableGenesIndexes;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Mutations.UniformMutation"/> class.
        /// </summary>
        /// <param name="allGenesMutable">If set to <c>true</c> all genes are mutable.</param>
        public UniformMutation(bool allGenesMutable)
        {
            m_allGenesMutable = allGenesMutable;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Mutations.UniformMutation"/> class.
        /// </summary>
        /// <remarks>Creates an instance of UniformMutation where some random genes will be mutated.</remarks>
        public UniformMutation()
        {
            m_mutableGenesIndexes = new int[0];
            m_allGenesMutable = true;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Mutate the specified chromosome.
        /// </summary>
        /// <param name="chromosome">The chromosome.</param>
        /// <param name="probability">The probability to mutate each chromosome.</param>
        protected override void PerformMutate(ChromosomeBase evaluable, float probability)
        {
            ExceptionHelper.ThrowIfNull("chromosome", evaluable);

            var data = evaluable.GetDataSquence<object>();

            var genesLength = data.Length;

            if (m_mutableGenesIndexes == null || m_mutableGenesIndexes.Length == 0)
            {
                if (m_allGenesMutable)
                {
                    m_mutableGenesIndexes = Enumerable.Range(0, genesLength).ToArray();
                }
                else
                {
                    m_mutableGenesIndexes = RandomizationProvider.Current.GetInts(1, 0, genesLength);
                }
            }

            for (int i = 0; i < m_mutableGenesIndexes.Length; i++)
            {
                var geneIndex = m_mutableGenesIndexes[i];

                if (geneIndex >= genesLength)
                {
                    throw new MutationException(this, "The chromosome has no gene on index {0}. The chromosome genes length is {1}.".With(geneIndex, genesLength));
                }

                if (RandomizationProvider.Current.GetDouble() <= probability)
                {
                    data[geneIndex] = evaluable.GenerateGene();
                }
            }
            evaluable.SetDataSequence(data);
        }
        #endregion
    }
}