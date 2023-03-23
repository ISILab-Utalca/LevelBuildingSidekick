using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

[System.Serializable]
[LBSCharacteristic("Basic Characteristic","")]
public class LBSCharacteristic : ICloneable
{
    [JsonRequired, SerializeField]
    protected string label = "";

    [JsonIgnore]
    public string Label
    {
        get => label;
        set => label = value;
    }

    public LBSCharacteristic() { }

    public LBSCharacteristic(string label)
    {
        this.label = label;
    }

    public virtual object Clone()
    {
        return new LBSCharacteristic(this.label);
    }

    public override bool Equals(object obj)
    {
        return obj is LBSCharacteristic && (obj as LBSCharacteristic)?.label == label;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
