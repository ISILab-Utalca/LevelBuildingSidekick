using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOptimizable : ICloneable
{
    /// <summary>
    /// Gets or sets the fitness.
    /// </summary>
    /// <value>The fitness.</value>
    double Fitness { get; set; }

    /// <summary>
    /// Creates a new evaluable using the same structure of this.
    /// </summary>
    /// <returns>The new evaluable.</returns>
    public IOptimizable CreateNew();

    public string ToString();
}
