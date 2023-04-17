using System;
using System.ComponentModel;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;
using UnityEngine.UIElements;

namespace GeneticSharp.Domain.Mutations
{
    /// <summary>
    /// Takes the chosen genome and inverts the bits (i.e. if the genome bit is 1, it is changed to 0 and vice versa).
    /// </summary>
    /// <remarks>
    /// When using this mutation the genetic algorithm should use IBinaryChromosome.
    /// </remarks>
    [DisplayName("Flip Bit")]
    public class FlipBitMutation : MutationBase
    {
        #region Fields
        private readonly IRandomization m_rnd;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Mutations.FlipBitMutation"/> class.
        /// </summary>
        public FlipBitMutation ()
        {
            m_rnd = RandomizationProvider.Current;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Mutate the specified chromosome.
        /// </summary>
        /// <param name="evaluable">The chromosome.</param>
        /// <param name="probability">The probability to mutate each chromosome.</param>
        protected override void PerformMutate (ChromosomeBase evaluable, float probability)
        {
            var data = evaluable.GetGenes();
            if (m_rnd.GetDouble() <= probability)
            {
                var index = m_rnd.GetInt(0, data.Length);
                var boolean = data[index];
                if(!(boolean is bool))
                {
                    throw new TypeAccessException("Input must be colection of bool");
                }
                data[index] = !((bool)data[index]);
                (evaluable as IChromosome).SetDataSequence(data);
            }
        }
        #endregion
    }
}

