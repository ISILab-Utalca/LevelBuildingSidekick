using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOptimizable
{
    /// <summary>
    /// Gets or sets the fitness.
    /// </summary>
    /// <value>The fitness.</value>
    double? Fitness { get; set; }
    public T GetData<T>();
    public void SetDataSequence<T>(T[] data);
    public void SetData<T>(T data);

    /// <summary>
    /// Creates a new evaluable using the same structure of this.
    /// </summary>
    /// <returns>The new evaluable.</returns>
    public IOptimizable CreateNew();

    /// <summary>
    /// Creates a clone.
    /// </summary>
    /// <returns>The Evaluable clone.</returns>
    public IOptimizable Clone();

    public bool IsValid();
    public T GetSampleData<T>();
    public T[] GetDataSquence<T>();
    public string ToString();
}
