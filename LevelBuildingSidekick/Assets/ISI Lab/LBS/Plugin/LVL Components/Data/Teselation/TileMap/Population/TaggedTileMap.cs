using LBS.Bundles;
using LBS.Components;
using LBS.Components.TileMap;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
[Obsolete("??")]
public class TaggedTileMap : LBSModule
{
    [SerializeField, JsonRequired, SerializeReference]
    private List<TileBundlePair> pairTiles = new List<TileBundlePair>();

    [JsonIgnore]
    public List<TileBundlePair> PairTiles => pairTiles;

    public Func<LBSTile, bool> OnRemoveTile { get; private set; }

    [JsonIgnore]
    protected Func<LBSTile, bool> OnAddTile;

    public TaggedTileMap() : base() 
    { 
        ID = GetType().Name;
    }

    public TaggedTileMap(string key, List<TileBundlePair> tiles) : base(key)
    {
        this.pairTiles = tiles;
    }

    public override void Clear()
    {
        for(int i = 0; i < pairTiles.Count; i++)
        {
            OnRemoveTile?.Invoke(pairTiles[i].Tile);
        }
        pairTiles.Clear();
    }

    public BundleData GetBundleData(LBSTile tile)
    {
        return pairTiles.Find(x => x.Tile == tile)?.BundleData;
    }

    public void AddTile(LBSTile tile, Bundle bundle)
    {
        var data = new BundleData(bundle.name, bundle.Characteristics);
        AddTile(tile, data);
    }

    public void AddTile(LBSTile tile, BundleData data)
    {
        var t = pairTiles.Find(p => p.Tile.Equals(tile));

        if (t == null)
        {
            OnAddTile?.Invoke(tile);
            t = pairTiles.Find(p => p.Tile.Equals(tile));
            //pairTiles.Add(new TileBundlePair(tile, data));
        }
        t.BundleData = data;
    }

    public override object Clone()
    {
        var dir = new List<TileBundlePair>();
        foreach (var pair in pairTiles)
        {
            dir.Add(new TileBundlePair(pair.Tile, pair.BundleData, pair.Rotation));
        }

        return new TaggedTileMap(id, dir);
    }

    public override bool IsEmpty()
    {
        return pairTiles.Count == 0;
    }

    public override void OnAttach(LBSLayer layer)
    {
        var tileMap = layer.GetModule<TileMapModule>();
        //OnAddTile += tileMap.AddTile;
        OnRemoveTile += tileMap.RemoveTile;
    }

    public override void OnDetach(LBSLayer layer)
    {
        var tileMap = layer.GetModule<TileMapModule>();
        //OnAddTile -= tileMap.AddTile;
        OnRemoveTile -= tileMap.RemoveTile;
    }

    public override void Print()
    {
        throw new System.NotImplementedException();
    }

    public void RemoveTile(object tile)
    {
        var toR = tile as LBSTile;
        var xx = pairTiles.Find(x => x.Tile.Equals(toR));
        pairTiles.Remove(xx);
    }

    public void AddEmpty(object tile)
    {
        var t = tile as LBSTile;
        var xx = pairTiles.Find(x => x.Tile.Equals(t));
        if(xx != null)
        {
            xx.BundleData = null;
            //RemoveTile(xx);
            return;
        }
        pairTiles.Add(new TileBundlePair(t, null, Vector2.right));
        //if (pairTiles.ContainsKey(t))
        //    pairTiles.Add((t), new BundleData());
    }

    public override Rect GetBounds()
    {
        var x = pairTiles.Min(p => p.Tile.Position.x);
        var y = pairTiles.Min(p => p.Tile.Position.y);
        var with = pairTiles.Max(p => p.Tile.Position.x) - x + 1;
        var height = pairTiles.Max(p => p.Tile.Position.y) - y + 1;

        return new Rect(x, y, with, height);
    }

    public override void Rewrite(LBSModule module)
    {
        var tileMap = module as TaggedTileMap;

        if(tileMap == null)
        {
            throw new Exception("[ISI Lab] Modules have to be of the same type!");
        }

        Clear();

        foreach(var p in tileMap.PairTiles)
        {
            AddTile(p.Tile, p.BundleData);
        }
    }

    public override void Reload(LBSLayer layer)
    {
        OnAttach(layer);
    }
}