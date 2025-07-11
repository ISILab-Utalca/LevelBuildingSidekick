using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ISILab.AI.Optimization
{
    public interface IOptimizable : ICloneable
    {
        /// <summary>
        /// Gets or sets the fitness.
        /// </summary>
        /// <value>The fitness.</value>
        double Fitness { get; set; }

        /// <summary>
        /// Gets or sets the fitness.
        /// </summary>
        /// <value>The fitness.</value>
        double xFitness { get; set; }
        
        /// <summary>
        /// Gets or sets the fitness.
        /// </summary>
        /// <value>The fitness.</value>
        double yFitness { get; set; }
        
        /// <summary>
        /// Creates a new evaluable using the same structure of this.
        /// </summary>
        /// <returns>The new evaluable.</returns>
        public IOptimizable CreateNew();

        public string ToString();
    }
}