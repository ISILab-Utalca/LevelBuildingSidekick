using LBS.Components;
using LBS.Components.TileMap;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
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
        Key = GetType().Name;
    }

    public TaggedTileMap(string key, List<TileBundlePair> tiles) : base(key)
    {
        this.pairTiles = tiles;
    }

    public override void Clear()
    {
        for(int i = 0; i < pairTiles.Count; i++)
        {
            OnRemoveTile?.Invoke(pairTiles[i].tile);
        }
        pairTiles.Clear();
    }

    public BundleData GetBundleData(LBSTile tile)
    {
        return pairTiles.Find(x => x.tile == tile)?.bData;
    }

    public void AddTile(LBSTile tile, Bundle bundle)
    {

        var data = new BundleData(bundle.ID.Label, bundle.GetCharacteristics());
        AddTile(tile, data);
    }
    public void AddTile(LBSTile tile, BundleData data)
    {
        var t = pairTiles.Find(p => p.tile.Equals(tile));

        if (t == null)
        {
            OnAddTile?.Invoke(tile);
            t = pairTiles.Find(p => p.tile.Equals(tile));
            //pairTiles.Add(new TileBundlePair(tile, data));
        }
        t.bData = data;
    }

    public override object Clone()
    {
        var dir = new List<TileBundlePair>();
        foreach (var pair in pairTiles)
        {
            dir.Add(new TileBundlePair(pair.tile, pair.bData));
        }

        return new TaggedTileMap(key, dir);
    }

    public override bool IsEmpty()
    {
        return pairTiles.Count == 0;
    }

    public override void OnAttach(LBSLayer layer)
    {
        var tileMap = layer.GetModule<LBSTileMap>();
        tileMap.OnRemoveData += RemoveTile;
        //Verificar posible recursividad
        tileMap.OnAddData += AddEmpty;
        OnAddTile += tileMap.AddTile;
        OnRemoveTile += tileMap.RemoveTile;
    }

    public override void OnDetach(LBSLayer layer)
    {
        var tileMap = layer.GetModule<LBSTileMap>();
        tileMap.OnRemoveData -= RemoveTile;
        tileMap.OnAddData -= AddEmpty;
        OnAddTile -= tileMap.AddTile;
        OnRemoveTile -= tileMap.RemoveTile;
    }

    public override void Print()
    {
        throw new System.NotImplementedException();
    }

    public void RemoveTile(object tile)
    {
        var toR = tile as LBSTile;
        var xx = pairTiles.Find(x => x.tile.Equals(toR));
        pairTiles.Remove(xx);
    }

    public void AddEmpty(object tile)
    {
        var t = tile as LBSTile;
        var xx = pairTiles.Find(x => x.tile.Equals(t));
        if(xx != null)
        {
            xx.bData = null;
            //RemoveTile(xx);
            return;
        }
        pairTiles.Add(new TileBundlePair(t, null));
        //if (pairTiles.ContainsKey(t))
        //    pairTiles.Add((t), new BundleData());
    }

    public override Rect GetBounds()
    {
        var x = pairTiles.Min(p => p.tile.Position.x);
        var y = pairTiles.Min(p => p.tile.Position.y);
        var with = pairTiles.Max(p => p.tile.Position.x) - x + 1;
        var height = pairTiles.Max(p => p.tile.Position.y) - y + 1;

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
            AddTile(p.tile, p.bData);
        }
    }
}

[System.Serializable]
public class TileBundlePair
{
    [SerializeField]
    public LBSTile tile;
    [SerializeField]
    public BundleData bData;

    public TileBundlePair(LBSTile tile, BundleData bData)
    {
        this.tile = tile;
        this.bData = bData;
    }
}
