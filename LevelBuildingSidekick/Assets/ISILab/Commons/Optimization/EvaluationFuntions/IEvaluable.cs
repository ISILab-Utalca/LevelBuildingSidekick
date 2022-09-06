using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEvaluable
{
    /// <summary>
    /// Gets or sets the fitness.
    /// </summary>
    /// <value>The fitness.</value>
    double? Fitness { get; set; }
    public T GetData<T>();
}
