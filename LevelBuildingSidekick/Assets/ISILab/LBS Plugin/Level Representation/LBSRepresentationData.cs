using LBS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class LBSRepresentationData
{
    private string label;
    public string Label
    {
        get => label;
        set => label = value;
    }

    public LBSRepresentationData()
    {
        label = this.GetType().Name;
    }
    public LBSRepresentationData(string label) 
    {
        this.label = label;
    }


    /// <summary>
    /// prints by console basic information of 
    /// the representation.
    /// </summary>
    public abstract void Print();

    /// <summary>
    /// Cleans all the information saved in.
    /// </summary>
    public abstract void Clear();
}

public abstract class LBSCompositeRepresentationData
{
    public List<LBSRepresentationData> representations = new List<LBSRepresentationData>();


}
