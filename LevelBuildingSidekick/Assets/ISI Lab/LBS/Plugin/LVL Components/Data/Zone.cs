using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Zone : ICloneable
{
    #region FIELDS

    [SerializeField, JsonRequired]
    protected string id = "Zone";
    [SerializeField, JsonRequired, JsonConverter(typeof(ColorConverter))]
    protected Color color;
    [SerializeField, JsonRequired, JsonConverter(typeof(Vector2))]
    protected Vector2 pivot;

    #endregion

    #region PROPERTIES

    [JsonIgnore]
    public string ID
    {
        get => id;
        set => id = value;
    }

    [JsonIgnore]
    public Color Color
    {
        get => color;
        set => color = value;
    }

    [JsonIgnore]
    public Vector2 Pivot
    {
        get => pivot;
        set => pivot = value;
    }

    #endregion

    #region CONSTRUCTORS

    public Zone() { }

    public Zone(string id, Color color)
    {
        this.id = id;
        this.color = color;
    }

    #endregion

    #region METHODS

    public object Clone()
    {
        return new Zone(id, color);
    }

    #endregion
}
