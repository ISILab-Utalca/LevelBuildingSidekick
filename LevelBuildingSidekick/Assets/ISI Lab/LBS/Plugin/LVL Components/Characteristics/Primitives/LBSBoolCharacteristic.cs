using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
[LBSCharacteristic("Boolean","")]
public class LBSBoolCharacteristic : LBSCharacteristic
{
    [SerializeField]
    protected bool value;

    public bool Value
    {
        get => value;
        set => this.value = value;
    }

    public LBSBoolCharacteristic(bool value)
    {
        this.value = value;
    }

    public LBSBoolCharacteristic()
    {
        this.value = false;
    }

    public override object Clone()
    {
        return new LBSBoolCharacteristic(this.value);
    }

    public override bool Equals(object obj)
    {
        var other = obj as LBSBoolCharacteristic;

        if (other == null) return false;

        return other.value == this.value;
    }

    public override string ToString()
    {
        return base.ToString();
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}