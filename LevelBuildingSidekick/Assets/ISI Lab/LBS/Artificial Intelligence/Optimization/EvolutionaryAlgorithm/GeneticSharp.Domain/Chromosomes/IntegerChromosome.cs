﻿using GeneticSharp.Domain.Randomizations;
using System;
using System.Collections;
using System.Data.SqlTypes;
using System.Linq;
using UnityEngine;

namespace GeneticSharp.Domain.Chromosomes
{
    /// <summary>
    /// Integer chromosome with binary values (0 and 1).
    /// </summary>
    public class IntegerChromosome //: BinaryChromosomeBase
    {/*
        private readonly int m_minValue;
        private readonly int m_maxValue;
        private readonly BitArray m_originalValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:GeneticSharp.Domain.Chromosomes.IntegerChromosome"/> class.
        /// </summary>
        /// <param name="minValue">Minimum value.</param>
        /// <param name="maxValue">Maximum value.</param>
        public IntegerChromosome(int minValue, int maxValue) : base(32)
        {
            m_minValue = minValue;
            m_maxValue = maxValue;
            var intValue = RandomizationProvider.Current.GetInt(m_minValue, m_maxValue);
            m_originalValue = new BitArray(new int[] { intValue });

            CreateGenes();
        }

        /// <summary>
        /// Generates the gene.
        /// </summary>
        /// <returns>The gene.</returns>
        /// <param name="geneIndex">Gene index.</param>
        public override object GenerateGene(int geneIndex)
        {
            var value = m_originalValue[geneIndex];

            return value;
        }

        /// <summary>
        /// Creates the new.
        /// </summary>
        /// <returns>The new.</returns>
        public override IChromosome CreateNewChromosome()
        {
            return new IntegerChromosome(m_minValue, m_maxValue);
        }

        /// <summary>
        /// Converts the chromosome to its integer representation.
        /// </summary>
        /// <returns>The integer.</returns>
        public int ToInteger()
        {
            var array = new int[1];
            var genes = GetGenes<bool>();
            var bitArray = new BitArray(genes);
            bitArray.CopyTo(array, 0);

            return array[0];
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:GeneticSharp.Domain.Chromosomes.FloatingPointChromosome"/>.
        /// </summary>
        /// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:GeneticSharp.Domain.Chromosomes.FloatingPointChromosome"/>.</returns>
        public override string ToString()
        {
            return String.Join("", GetGenes<bool>().Reverse().Select(g => g ? "1" : "0").ToArray());
        }

        /// <summary>
        /// Flips the gene.
        /// </summary>
        /// <remarks>>
        /// If gene's value is 0, the it will be flip to 1 and vice-versa.</remarks>
        /// <param name="index">The gene index.</param>
        public override void FlipGene(int index)
        {
            var realIndex = Math.Abs(31 - index);
            var value = GetGene<bool>(realIndex);

            ReplaceGene(realIndex, !value);
        }

        /// <summary>
        /// Converts an object to a texture.
        /// </summary>
        /// <returns>A <see cref="Texture2D"/> representing the object.</returns>
        public override Texture2D ToTexture()
        {
            return null;
        }*/
    }
}

