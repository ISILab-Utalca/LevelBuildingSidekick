using LBS.Components.TileMap;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LBSPopulationTile : LBSTile
{
    [SerializeField, JsonRequired]
    string brushTag;

    [JsonIgnore]
    public string BrushTag
    {
        get => brushTag;
        set => brushTag = value;
    }

    public LBSPopulationTile() : base() { }

    public LBSPopulationTile(string brushTag, Vector2 position, string id, int sides = 4) : base()
    {
        this.brushTag = brushTag;
    }

    public override object Clone()
    {
        return new LBSPopulationTile(brushTag, Position, ID, Sides);
    }
}
