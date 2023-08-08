using LBS.Components;
using LBS.Components.TileMap;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConnectedTileMapModule : LBSModule
{
    #region FIELDS

    [SerializeField, JsonRequired]
    protected int connectedDirections = 4;

    [SerializeField, JsonRequired]
    protected List<TileConnectionsPair> tiles = new List<TileConnectionsPair>();

    #endregion

    #region PROPERTIES

    public int ConnecteDirections
    {
        get => connectedDirections;
        set => connectedDirections = value;
    }

    public List<TileConnectionsPair> Tiles => new List<TileConnectionsPair>(tiles);

    #endregion

    #region CONSTRUCTORS

    public ConnectedTileMapModule() : base()
    {
        id = GetType().Name;
    }

    public ConnectedTileMapModule(IEnumerable<TileConnectionsPair> tiles, int connectedDirections, string id = "ConnectedTileMapModule") : base(id)
    {
        this.connectedDirections = connectedDirections;
        foreach(var t in tiles)
        {
            AddTile(t);
        }
    }

    #endregion

    #region METHODS

    public void AddTile(TileConnectionsPair tile)
    {
        if(tile.Connections.Count < connectedDirections)
        {
            for (int i = tile.Connections.Count; i < connectedDirections; i++)
            {
                tile.AddConnection("");
            }
        }
        else
        {
            tile.ReplaceConnections(tile.Connections.Take(connectedDirections));
        }

        var t = GetTile(tile.Tile);
        if(t != null)
        {
            tiles.Remove(t);
        }
        tiles.Add(tile);

    }

    public void AddTile(LBSTile tile, List<string> connections)
    {
        AddTile(new TileConnectionsPair(tile, connections));
    }

    public TileConnectionsPair GetTile(LBSTile tile)
    { 
        if (tiles.Count <= 0)
            return null;
        return tiles.Find(t => t.Tile.Equals(tile));

    }

    public void RemoveTile(LBSTile tile)
    {
        var t = GetTile(tile);
        tiles.Remove(t);
    }

    public void RemoveTile(int index)
    {
        tiles.RemoveAt(index);
    }

    public bool Contains(LBSTile tile)
    {
        if (tiles.Count <= 0)
            return false;
        return tiles.Any(t => t.Tile.Equals(tile));
    }

    public override Rect GetBounds()
    {
        if (tiles == null || tiles.Count == 0)
        {
            //Debug.LogWarning("Esta tilemap no tiene tiles!!!");
            return new Rect(Vector2.zero, Vector2.zero);
        }
        return tiles.Select(t => t.Tile).GetBounds();
    }

    public override bool IsEmpty()
    {
        return tiles.Count <= 0;
    }

    public override void Clear()
    {
        tiles.Clear();
    }

    public override object Clone()
    {
        return new ConnectedTileMapModule(tiles.Select(t => t.Clone()).Cast<TileConnectionsPair>(), connectedDirections, ID);
    }

    public override void Rewrite(LBSModule module)
    {
        var connectedTileMap = module as ConnectedTileMapModule;
        if(connectedTileMap == null)
        {
            return;
        }
        Clear();
        connectedDirections = connectedTileMap.connectedDirections;
        foreach(var t in connectedTileMap.tiles)
        {
            AddTile(t);
        }

    }

    public override void OnAttach(LBSLayer layer)
    {
    }

    public override void OnDetach(LBSLayer layer)
    {
        throw new System.NotImplementedException();
    }

    public override void OnReload(LBSLayer layer)
    {
        //throw new System.NotImplementedException();
    }

    public override void Print()
    {
        throw new System.NotImplementedException();
    }

    #endregion

}

[System.Serializable]
public class TileConnectionsPair : ICloneable
{
    #region FIELDS

    [SerializeField, JsonRequired, SerializeReference]
    LBSTile tile;

    [SerializeField, JsonRequired]
    List<string> connections = new List<string>();

    #endregion

    #region PROEPRTIES

    [JsonIgnore]
    public LBSTile Tile
    {
        get => tile;
    }

    [JsonIgnore]
    public List<string> Connections
    {
        get => connections;
    }

    #endregion

    #region CONSTRUCTORS

    public TileConnectionsPair(LBSTile tile, IEnumerable<string> connections)
    {
        this.tile = tile;
        this.connections = connections.ToList();
    }

    #endregion

    #region METHODS

    public void ReplaceConnections(IEnumerable<string> connections)
    {
        this.connections = new List<string>(connections);
    }

    public void AddConnection(string connection)
    {
        connections.Add(connection);
    }

    public void SetConnection(int index, string connection)
    {
        connections[index] = connection;
    }

    public object Clone()
    {
        return new TileConnectionsPair(tile, connections.Select(c => c.Clone()).Cast<string>());
    }

    #endregion

}
