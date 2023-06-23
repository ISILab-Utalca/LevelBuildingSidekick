using System;
using System.Linq;
using GeneticSharp.Domain.Randomizations;

namespace GeneticSharp.Domain.Chromosomes
{
    /// <summary>
    /// A base class for binary chromosome of 0 and 1 genes.
    /// </summary>
    public abstract class BinaryChromosomeBase //: ChromosomeBase<bool>, IBinaryChromosome
    {/*
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Chromosomes.BinaryChromosomeBase"/> class.
        /// </summary>
        /// <param name="length">The length, in genes, of the chromosome.</param>
        protected BinaryChromosomeBase(int length) 
            : base(length)
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// Flips the gene.
        /// </summary>
        /// <remarks>>
        /// If gene's value is 0, the it will be flip to 1 and vice-versa.</remarks>
        /// <param name="index">The gene index.</param>
        public virtual void FlipGene (int index)    
        {
            var value = GetGene<bool>(index);

            ReplaceGene (index, !value);
        }

        /// <summary>
        /// Generates the gene for the specified index.
        /// </summary>
        /// <returns>The gene.</returns>
        /// <param name="geneIndex">Gene index.</param>
        public override object GenerateGene (int geneIndex)
        {
            return RandomizationProvider.Current.GetInt (0, 2) == 0;
        }


        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="GeneticSharp.Domain.Chromosomes.BinaryChromosomeBase"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="GeneticSharp.Domain.Chromosomes.BinaryChromosomeBase"/>.</returns>
        public override string ToString ()
        {
            return String.Join (string.Empty, GetGenes<bool>().Select (g => g.ToString()).ToArray());
        }
        #endregion*/
    }
}

