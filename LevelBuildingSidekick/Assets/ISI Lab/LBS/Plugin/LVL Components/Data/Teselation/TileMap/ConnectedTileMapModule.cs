using LBS.Components;
using LBS.Components.TileMap;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ConnectedTileMapModule : LBSModule
{
    #region FIELDS

    [SerializeField, JsonRequired]
    protected int connectedDirections = 4;

    [SerializeField, JsonRequired]
    protected List<TileConnectionsPair> pairs = new List<TileConnectionsPair>();

    #endregion

    #region PROPERTIES

    public int ConnecteDirections
    {
        get => connectedDirections;
        set => connectedDirections = value;
    }

    public List<TileConnectionsPair> Pairs => new List<TileConnectionsPair>(pairs);

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
            AddTile(t.Tile, t.Connections, t.EditedByIA);
        }
    }

    #endregion

    #region METHODS

    /*
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
            tile.SetConnections(tile.Connections.Take(connectedDirections));
        }

        var t = GetPair(tile.Tile);
        if(t != null)
        {
            pairs.Remove(t);
        }
        pairs.Add(tile);

    }
    */

    public void SetConnection(LBSTile tile, int direction, string connection, bool editedByIA)
    {
        var t = GetPair(tile);
        t.SetConnection(direction, connection, editedByIA);
    }

    public void AddTile(LBSTile tile, List<string> connections , List<bool> editedByIA)
    {
        var pair = new TileConnectionsPair(tile, connections, editedByIA);
        var t = GetPair(pair.Tile);
        if (t != null)
        {
            pairs.Remove(t);
        }
        pairs.Add(pair);
    }

    public TileConnectionsPair GetPair(LBSTile tile)
    { 
        if (pairs.Count <= 0)
            return null;
        return pairs.Find(t => t.Tile.Equals(tile));

    }

    public List<string> GetConnections(LBSTile tile)
    {
        var p = GetPair(tile);
        return p.Connections;
    }

    public void RemoveTile(LBSTile tile)
    {
        var t = GetPair(tile);
        pairs.Remove(t);
    }

    public void RemoveTile(int index)
    {
        pairs.RemoveAt(index);
    }

    public bool Contains(LBSTile tile)
    {
        if (pairs.Count <= 0)
            return false;
        return pairs.Any(t => t.Tile.Equals(tile));
    }

    public override Rect GetBounds()
    {
        if (pairs == null || pairs.Count == 0)
        {
            //Debug.LogWarning("Esta tilemap no tiene tiles!!!");
            return new Rect(Vector2.zero, Vector2.zero);
        }
        return pairs.Select(t => t.Tile).GetBounds();
    }

    public override bool IsEmpty()
    {
        return pairs.Count <= 0;
    }

    public override void Clear()
    {
        pairs.Clear();
    }

    public override object Clone()
    {
        return new ConnectedTileMapModule(pairs.Select(t => t.Clone()).Cast<TileConnectionsPair>(), connectedDirections, ID);
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
        foreach(var t in connectedTileMap.pairs)
        {
            AddTile(t.Tile, t.Connections, t.EditedByIA);
        }

    }

    public override void OnAttach(LBSLayer layer)
    {
    }

    public override void OnDetach(LBSLayer layer)
    {

    }

    public override void Reload(LBSLayer layer)
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
public class TileConnectionsPair : ICloneable // esto puede ser TAG/BUNDLE en vez de string (!!)
{
    #region FIELDS
    [SerializeField, SerializeReference, JsonRequired]
    private LBSTile tile;

    [SerializeField, JsonRequired]
    private List<string> connections = new List<string>();

    [SerializeField, JsonRequired]
    private List<bool> editedByIA = new List<bool>();
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

    public List<bool> EditedByIA => editedByIA;
    #endregion

    #region CONSTRUCTORS
    public TileConnectionsPair(LBSTile tile, IEnumerable<string> connections, List<bool> editedByIA)
    {
        this.tile = tile;
        this.connections = connections.ToList();
        this.editedByIA = editedByIA;
    }
    #endregion

    #region METHODS
    public void SetConnections(IEnumerable<string> connections, List<bool> editedByIA)
    {
        this.connections = new List<string>(connections);
        this.editedByIA = new List<bool>(editedByIA);
    }

    /*
    public void AddConnection(string connection)
    {
        connections.Add(connection);
    }
    */

    public void SetConnection(int index, string connection, bool editedByIA)
    {
        this.connections[index] = connection;
        this.editedByIA[index] = editedByIA;
    }

    public object Clone()
    {
        return new TileConnectionsPair(
            tile,
            connections.Select(c => c.Clone()).Cast<string>(),
            new List<bool>(editedByIA)
            );
    }
    #endregion

}
