using LevelBuildingSidekick;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class LBSRepesentationData : Data
{
    public override Type ControllerType => throw new NotImplementedException();

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
