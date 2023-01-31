using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class listaIeva : IEvaluable
{

    public List<int> a = new List<int>();

    public listaIeva()
    {
        for(int i = 0; i < 15; i++) { a.Add(i); }

    }

    public double? Fitness { get; set; }


    public IEvaluable Clone()
    {
        throw new System.NotImplementedException();
    }

    public IEvaluable CreateNew()
    {
        throw new System.NotImplementedException();
    }

    public T GetData<T>()
    {
        throw new System.NotImplementedException();
    }

    public T[] GetDataSquence<T>()
    {
        throw new System.NotImplementedException();
    }

    public T GetSampleData<T>()
    {
        throw new System.NotImplementedException();
    }

    public bool IsValid()
    {
        throw new System.NotImplementedException();
    }

    public void SetData<T>(T data)
    {
        throw new System.NotImplementedException();
    }

    public void SetDataSequence<T>(T[] data)
    {
        throw new System.NotImplementedException();
    }
}
