using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class LBSBoolCharacteristic : LBSCharacteristic
{
    [SerializeField]
    protected bool value;

    public bool Value
    {
        get => value;
        set => this.value = value;
    }
}