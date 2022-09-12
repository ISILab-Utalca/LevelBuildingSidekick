using LBS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class LBSRepesentationData : Data
{
    public override Type ControllerType => throw new NotImplementedException(); // no se usa

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

public abstract class LBSCompositeRepresentationData : Data
{
    public List<LBSRepesentationData> representations = new List<LBSRepesentationData>();

    public override Type ControllerType => throw new NotImplementedException();


}
