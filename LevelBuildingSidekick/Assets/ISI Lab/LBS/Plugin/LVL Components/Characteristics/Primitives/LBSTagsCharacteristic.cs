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

    public override bool Equals(object obj)
    {
        if (obj == null) 
            return false;
        if (!(obj is LBSTagsCharacteristic)) 
            return false;
        var ch = (LBSTagsCharacteristic)obj;
        if (ch.value != this.value) 
            return false;
        return true;
    }

    public override void OnEnable()
    {
       // throw new System.NotImplementedException();
    }
}
