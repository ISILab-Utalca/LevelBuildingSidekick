using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct IntRange
{
    //[Range(1,16)]
    [SerializeField, JsonRequired]
    public int min;
    [SerializeField, JsonRequired]
    public int max;

    [JsonIgnore]
    public int Middle => (int)((min + max) / 2f);

    public IntRange(int min, int max)
    {
        this.min = min;
        this.max = max;
    }
}
