using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ISILab.AI.Optimization
{
    public class EvolutinaryAlgorithmPreset : BaseOptimizerPreset
    {

        [SerializeField, SerializeReference]
        public ICrossover crossover;
        [SerializeField, SerializeReference]
        public IMutation mutation;

        /// <summary>
        /// The default crossover probability.
        /// </summary>
        [SerializeField]
        public float crossoverProbability = 1f;

        /// <summary>
        /// The default mutation probability.
        /// </summary>
        [SerializeField]
        public float mutationProbability = 0.05f;
    }
}
