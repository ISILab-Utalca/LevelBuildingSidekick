using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
[LBSCharacteristic("Directions", "")]
public class LBSDirection : LBSCharacteristic, ICloneable
{
    [Tooltip("4-Conected: 0: Foward, 1: Rigth, 2: Bottom, 3: Left")]
    [ScriptableToString(typeof(LBSIdentifier))]
    [SerializeField, JsonRequired]
    private List<string> tags = new List<string>();

    [JsonRequired]
    private int connectionAmount = 4;

    public List<string> Connections => new List<string>(tags);

    public LBSDirection() : base() { }

    public LBSDirection(string label,List<string> tags) : base(label) 
    {
        this.tags = tags;
    }

    public override object Clone()
    {
        return new LBSDirection(this.label, new List<string>(this.tags));
    }
}
