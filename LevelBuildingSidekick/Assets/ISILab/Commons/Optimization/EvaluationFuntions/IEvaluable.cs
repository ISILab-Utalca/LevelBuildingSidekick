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
    public void SetDataSequence<T>(T[] data);
    public void SetData<T>(T data);

    /// <summary>
    /// Creates a new evaluable using the same structure of this.
    /// </summary>
    /// <returns>The new evaluable.</returns>
    public IEvaluable CreateNew();

    /// <summary>
    /// Creates a clone.
    /// </summary>
    /// <returns>The Evaluable clone.</returns>
    public IEvaluable Clone();

    public bool IsValid();
    public T GetSampleData<T>();
    public T[] GetDataSquence<T>();
    public Texture2D ToTexture();
    public string ToString();
}
