using LBS;
using Newtonsoft.Json;
using System;
using System.Collections;
using UnityEngine;


[System.Serializable]
public class StampData : Data //cuestinable el nombre stamp pero no swe me ocurio otro (!!!)
{
    [HideInInspector, JsonRequired]
    private int x, y;
    [SerializeField, JsonRequired]
    private string label = ""; // "ID" or "name"

    /// <summary>
    /// Empty constructor, necessary for serialization with json.
    /// </summary>
    public StampData() { }

    public StampData(string label, Vector2 position)
    {
        this.label = label;
        x = (int)position.x;
        y = (int)position.y;
    }

    [JsonIgnore]
    public override Type ControllerType => throw new NotImplementedException(); // no se usa en nada (!!!)

    [JsonIgnore]
    public string Label
    {
        get => label;
        set => label = value;
    }

    [JsonIgnore]
    public Vector2Int Position
    {
        get => new Vector2Int(x, y);

        set
        {
            x = value.x;
            y = value.y;
        }
    }
}
