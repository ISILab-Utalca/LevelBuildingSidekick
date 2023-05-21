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

    public override object Clone()
    {
        return new LBSBoolCharacteristic(this.value);
    }

    public override void OnEnable()
    {
       // throw new System.NotImplementedException();
    }
}