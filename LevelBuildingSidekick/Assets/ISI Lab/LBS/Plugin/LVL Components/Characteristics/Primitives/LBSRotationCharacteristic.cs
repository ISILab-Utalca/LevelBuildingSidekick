using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LBSRotationCharacteristic : LBSCharacteristic
{
    [SerializeField, JsonRequired]
    Vector2 rotation = Vector2.right;

    [JsonIgnore]
    public Vector2 Rotation
    {
        get => rotation;
        set => rotation = value;
    }

    public LBSRotationCharacteristic() : base()
    {

    }

    public LBSRotationCharacteristic(string label, Vector2 vector) : base(label)
    {
        rotation = vector;
    }

    public override object Clone()
    {
        return new LBSRotationCharacteristic(label, rotation);
    }

    public override void OnEnable()
    {
        throw new System.NotImplementedException();
    }
}
