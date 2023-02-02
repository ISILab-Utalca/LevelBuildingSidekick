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
        protected override void PerformMutate(IOptimizable evaluable, float probability)
        {
            ExceptionHelper.ThrowIfNull("chromosome", evaluable);

            var data = (evaluable as IChromosome).GetDataSquence<object>();

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
                    data[geneIndex] = (evaluable as IChromosome).GetSampleData<object>();
                }
            }
            (evaluable as IChromosome).SetDataSequence(data);
        }

        public override VisualElement CIGUI()
        {
            var content = new VisualElement();
            var allMutableToggle = new Toggle("All Mutable ");
            allMutableToggle.value = m_allGenesMutable;
            allMutableToggle.RegisterCallback<ChangeEvent<bool>>(e => m_allGenesMutable = e.newValue);

            var geneIndex = new ListView(
                m_mutableGenesIndexes,
                -1,
                () => 
                {
                    var i = m_mutableGenesIndexes.Length;
                    m_mutableGenesIndexes = m_mutableGenesIndexes.Resize(i + 1);
                    var iF = new IntegerField("Element " + i, -1);
                    iF.RegisterCallback<ChangeEvent<int>>(e => m_mutableGenesIndexes[i] = e.newValue);
                    return iF;
                },
                (e, i) => (e as IntegerField).value = m_mutableGenesIndexes[i]
                );
            geneIndex.headerTitle = "Mutable Genes Indexes";
            //Probably some USS already has all this, or XML
            geneIndex.showBorder = true;
            geneIndex.showFoldoutHeader = true;
            geneIndex.showAddRemoveFooter = true;
            geneIndex.showBoundCollectionSize = true;
            geneIndex.fixedItemHeight = 20;
            geneIndex.tooltip = "Use only if not all genes are mutable to specify which ones are";

            content.Add(allMutableToggle);
            content.Add(geneIndex);

            return content;
        }
        #endregion
    }
}