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
    private TileMapModule tileMap;
    
    [JsonRequired, SerializeField]
    private ConnectedTileMapModule connections;
    
    [JsonRequired, SerializeField]
    public string targetBundle;
    #endregion

    #region PROPERTIES
    [JsonIgnore]
    public List<LBSTile> Tiles => tileMap.Tiles;
    #endregion

    #region CONSTRUCTORS
    public ExteriorBehaviour(Texture2D icon, string name) : base(icon, name) { }
    #endregion

    #region METHODS
    public override object Clone()
    {
        return new ExteriorBehaviour(this.Icon, this.Name);
    }

    public override void OnAdd(LBSLayer layer)
    {
        Owner = layer;

        tileMap = Owner.GetModule<TileMapModule>();
        connections = Owner.GetModule<ConnectedTileMapModule>();
    }

    public LBSTile GetTile(Vector2Int pos)
    {
        return tileMap.GetTile(pos);
    }

    public void RemoveTile(LBSTile tile)
    {
        tileMap.RemoveTile(tile);
        connections.RemoveTile(tile);
    }

    public void AddTile(LBSTile tile)
    {
        tileMap.AddTile(tile);
        connections.AddTile(tile, new List<string> { "", "", "", "" }, new List<bool> { false, false, false, false });
    }
    #endregion
}
