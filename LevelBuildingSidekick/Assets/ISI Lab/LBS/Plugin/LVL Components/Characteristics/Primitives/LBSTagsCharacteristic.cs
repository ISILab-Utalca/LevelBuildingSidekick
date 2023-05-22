using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
[LBSCharacteristic("Tags","")]
public class LBSTagsCharacteristic : LBSCharacteristic
{
    [SerializeField]
    protected LBSIdentifier value;

    public LBSIdentifier Value
    {
        get => value;
        set => this.value = value;
    }

    public LBSTagsCharacteristic(LBSIdentifier value)
    {
        this.value = value;
    }

    public LBSTagsCharacteristic()
    {
        this.value = null;
    }

    public override object Clone()
    {
        return new LBSTagsCharacteristic(this.value);
    }

    public override void OnEnable()
    {
       // throw new System.NotImplementedException();
    }
}
