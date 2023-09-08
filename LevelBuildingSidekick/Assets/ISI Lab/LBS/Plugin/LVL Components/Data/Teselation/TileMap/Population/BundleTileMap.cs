using LBS.Components;
using LBS.Components.TileMap;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BundleTileMap : LBSModule
{
    #region FIELDS

    [SerializeField, JsonRequired]
    protected List<TileBundlePair> tiles = new List<TileBundlePair>();

    #endregion

    #region PROPERTIES

    public List<TileBundlePair> Tiles => new List<TileBundlePair>(tiles);

    #endregion

    #region CONSTRUCTORS

    public BundleTileMap() : base()
    {
        id = GetType().Name;
    }

    public BundleTileMap(IEnumerable<TileBundlePair> tiles, string id = "ConnectedTileMapModule") : base(id)
    {
        foreach (var t in tiles)
        {
            AddTile(t);
        }
    }

    #endregion

    #region METHODS

    public void AddTile(TileBundlePair tile)
    {
        var t = GetTile(tile.Tile);
        if (t != null)
        {
            tiles.Remove(t);
        }
        tiles.Add(tile);

    }

    public void AddTile(LBSTile tile, BundleData bundleData, Vector2 rotation)
    {
        AddTile(new TileBundlePair(tile, bundleData, rotation));
    }

    public TileBundlePair GetTile(LBSTile tile)
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
        return new BundleTileMap(tiles.Select(t => t.Clone()).Cast<TileBundlePair>(), ID);
    }

    public override void Rewrite(LBSModule module)
    {
        var map = module as BundleTileMap;
        if (map == null)
        {
            return;
        }
        Clear();
        foreach (var t in map.tiles)
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
public class TileBundlePair : ICloneable
{
    [SerializeField, JsonRequired]
    LBSTile tile;
    [SerializeField, JsonRequired]
    BundleData bData;
    [SerializeField, JsonRequired]
    Vector2 rotation;

    [JsonIgnore]
    public LBSTile Tile
    {
        get => tile;
        set => tile = value;
    }

    [JsonIgnore]
    public BundleData BundleData
    {
        get => bData;
        set => bData = value;
    }

    public Vector2 Rotation
    {
        get => rotation;
        set => rotation = value;
    }

    public TileBundlePair(LBSTile tile, BundleData bData, Vector2 rotation)
    {
        this.tile = tile;
        this.bData = bData;
        this.rotation = rotation;
    }

    public object Clone()
    {
        return new TileBundlePair(tile.Clone() as LBSTile, bData.Clone() as BundleData, rotation);
    }
}