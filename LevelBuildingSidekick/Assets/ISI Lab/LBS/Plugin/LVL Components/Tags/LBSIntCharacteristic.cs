using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class LBSIntCharacteristic : LBSCharacteristic
{
    [SerializeField]
    protected int value;

    public int Value
    {
        get => value;
        set => this.value = value;
    }
}
