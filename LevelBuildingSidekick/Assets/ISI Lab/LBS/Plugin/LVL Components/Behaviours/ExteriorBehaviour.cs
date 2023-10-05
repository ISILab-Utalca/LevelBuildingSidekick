using LBS.Behaviours;
using LBS.Components;
using Newtonsoft.Json;
using LBS.Components.TileMap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
[RequieredModule(typeof(TileMapModule),
                typeof(ConnectedTileMapModule))]
public class ExteriorBehaviour : LBSBehaviour
{
    #region FIELDS
    [JsonRequired, SerializeField]
    private string targetBundle = "Exterior_Plains";
    #endregion

    #region PROPERTIES
    [JsonIgnore]
    private TileMapModule TileMap => Owner.GetModule<TileMapModule>();

    [JsonIgnore]
    private ConnectedTileMapModule Connections => Owner.GetModule<ConnectedTileMapModule>();

    [JsonIgnore]
    public string TargetBundle
    {
        get => targetBundle;
        set => targetBundle = value;
    }

    [JsonIgnore]
    public List<LBSTile> Tiles => TileMap.Tiles;

    [JsonIgnore]
    public List<Vector2Int> Directions => global::Directions.Bidimencional.Edges;
    #endregion

    #region CONSTRUCTORS
    public ExteriorBehaviour(Texture2D icon, string name) : base(icon, name) { }
    #endregion

    #region METHODS
    public override void OnAttachLayer(LBSLayer layer)
    {
        Owner = layer;
    }

    public override void OnDetachLayer(LBSLayer layer)
    {
        throw new NotImplementedException();
    }

    public LBSTile GetTile(Vector2Int pos)
    {
        return TileMap.GetTile(pos);
    }

    public void RemoveTile(LBSTile tile)
    {
        Owner.GetModule<TileMapModule>().RemoveTile(tile);
        Owner.GetModule<ConnectedTileMapModule>().RemoveTile(tile);
    }

    public void AddTile(LBSTile tile)
    {
        TileMap.AddTile(tile);
        Connections.AddPair(tile, new List<string> { "", "", "", "" }, new List<bool> { false, false, false, false });
    }

    public void SetConnection(LBSTile tile, int direction, string connection, bool canEditedByAI)
    {
        var t = Owner.GetModule<ConnectedTileMapModule>() .GetPair(tile);
        t.SetConnection(direction, connection, canEditedByAI);
    }

    public List<string> GetConnections(LBSTile tile)
    {
        return Connections.GetConnections(tile);
    }

    public override object Clone()
    {
        return new ExteriorBehaviour(this.Icon, this.Name);
    }

    public override bool Equals(object obj)
    {
        var other = obj as ExteriorBehaviour;

        if (other == null) return false;

        if(!this.targetBundle.Equals(other.targetBundle)) return false;

        return true;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
    #endregion
}
