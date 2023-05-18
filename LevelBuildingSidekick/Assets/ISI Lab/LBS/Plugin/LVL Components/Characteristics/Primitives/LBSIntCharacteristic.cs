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

    public LBSIntCharacteristic(int value)
    {
        this.value = value;
    }

    public override object Clone()
    {
        return new LBSIntCharacteristic(this.value);
    }

    public override void OnEnable()
    {
       // throw new System.NotImplementedException();
    }
}