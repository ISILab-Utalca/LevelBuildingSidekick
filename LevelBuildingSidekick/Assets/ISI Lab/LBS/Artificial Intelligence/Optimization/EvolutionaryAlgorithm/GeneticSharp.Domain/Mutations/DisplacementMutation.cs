﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GeneticSharp.Domain.Randomizations;
using UnityEngine;
using UnityEngine.UIElements;

namespace GeneticSharp.Domain.Mutations
{
    /// <summary>
    /// Displacement Mutation.
    /// <remarks>
    /// In the displacement mutation operator, a substring is randomly selected from chromosome, is removed, then replaced at a randomly selected position. 
    /// On implementation, we take a sequence S limited by two positions i and j randomly chosen, The selected substring in this sequence 
    /// will be left shifted or right shifted randomly to give the effect of removing the substring and inserting it on another position.
    /// <see href="https://web.cs.elte.hu/blobs/diplomamunkak/bsc_alkmat/2017/keresztury_bence.pdf">Genetic algorithms and the Traveling Salesman Problem</see>
    /// </remarks>
    /// </summary>
    [DisplayName("Displacement")]
    public class DisplacementMutation : SequenceMutationBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DisplacementMutation"/> class.
        /// </summary>
        public DisplacementMutation()
        {
            IsOrdered = true;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Mutate selected sequence.
        /// </summary>
        /// <returns>The resulted sequence after mutation operation.</returns>
        /// <param name="sequence">The sequence to be mutated.</param>
        protected override object[] MutateOnSequence(object[] sequence)
        {
            var geneToShift = DetermineGeneToShift(sequence.Length - 1);

            if (RandomizationProvider.Current.GetDouble() <= 0.5)
            {
                var s = sequence.AsEnumerable().LeftShift(geneToShift).ToArray();
                return s;
            }
            else
            {
                var s = sequence.AsEnumerable().RightShift(geneToShift).ToArray();
                return s;
            }
        }

        /// <summary>
        /// Determines genes to shift.
        /// <returns>Count of genes to be shifted</returns>
        /// </summary>
        /// <param name="maxCount">max possible count of genes to shift.</param>
        protected virtual int DetermineGeneToShift(int maxCount)
        {
             return RandomizationProvider.Current.GetInt(0, maxCount) + 1;
        }
        #endregion
    }
}
