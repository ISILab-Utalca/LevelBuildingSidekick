using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class LBSCharacteristic : ICloneable
{
    [SerializeField]
    protected string label;
    
    public string Label
    {
        get => label;
        set => label = value;
    }

    public object Clone()
    {
        throw new NotImplementedException();
    }

    public override bool Equals(object obj)
    {
        return obj is LBSCharacteristic && (obj as LBSCharacteristic).label == label;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
