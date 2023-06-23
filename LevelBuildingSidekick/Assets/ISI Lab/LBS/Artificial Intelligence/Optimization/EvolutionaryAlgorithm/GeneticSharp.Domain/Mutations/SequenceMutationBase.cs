using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;
using System.Linq;
using GeneticSharp.Infrastructure.Framework.Texts;
using UnityEngine;
using System;

namespace GeneticSharp.Domain.Mutations
{
    /// <summary>
    /// Base class for Mutations on a Sub-Sequence.
    /// </summary>
    public abstract class SequenceMutationBase : MutationBase
    {
        #region Methods
        /// <summary>
        /// Mutate the specified chromosome.
        /// </summary>
        /// <param name="chromosome">The chromosome.</param>
        /// <param name="probability">The probability to mutate each chromosome.</param>
        protected override void PerformMutate(ChromosomeBase evaluable, float probability)
        {
            var sequence = evaluable.GetGenes();

            ValidateLength(sequence);

            if (RandomizationProvider.Current.GetDouble() <= probability)
            {
                var indexes = RandomizationProvider.Current.GetUniqueInts(2, 0, sequence.Length).OrderBy(i => i).ToArray();
                var firstIndex = indexes[0];
                var secondIndex = indexes[1];
                var sequenceLength = (secondIndex - firstIndex) + 1;

                var m = MutateOnSequence(sequence.Skip(firstIndex).Take(sequenceLength).ToArray());

                Array.Copy(m, 0, sequence, firstIndex, sequenceLength);

                (evaluable as IChromosome).SetDataSequence(sequence);
            }
        }

        /// <summary>
        /// Validate length of the chromosome.
        /// </summary>
        /// <param name="sequence">The sequence.</param>
        protected virtual void ValidateLength(object[] sequence)
        {
            if (sequence.Length < 3)
            {
                throw new MutationException(this, "A chromosome should have, at least, 3 genes. {0} has only {1} gene.".With(sequence.GetType().Name, sequence.Length));
            }
        }

        /// <summary>
        /// Mutate selected sequence.
        /// </summary>
        /// <returns>The resulted sequence after mutation operation.</returns>
        /// <param name="sequence">The sequence to be mutated.</param>
        protected abstract object[] MutateOnSequence(object[] sequence);
        #endregion
    }
}
