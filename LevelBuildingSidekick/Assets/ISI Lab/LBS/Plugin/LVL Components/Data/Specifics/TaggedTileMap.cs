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
    private List<PairTB> pairTiles = new List<PairTB>();

    [JsonIgnore]
    public List<PairTB> PairTiles => pairTiles; 

    [JsonIgnore]
    public Rect Rect
    {
        get
        {
            var x = pairTiles.Min(p => p.tile.Position.x);
            var y = pairTiles.Min(p => p.tile.Position.y);
            var with = pairTiles.Max(p => p.tile.Position.x) - x + 1;
            var height = pairTiles.Max(p => p.tile.Position.y) - y + 1;

            return new Rect(x, y, with, height);
        }
    } // (!!)
    
    [JsonIgnore]
    protected Func<LBSTile, bool> OnAddTile;

    public TaggedTileMap() : base() 
    { 
        Key = GetType().Name;
    }

    public TaggedTileMap(string key, List<PairTB> tiles) : base(key)
    {
        this.pairTiles = tiles;
    }

    public override void Clear()
    {
        pairTiles.Clear();
    }

    public BundleData GetBundleData(LBSTile tile)
    {
        return pairTiles.Find(x => x.tile == tile)?.bData;
    }

    public void AddTile(LBSTile tile, Bundle bundle)
    {
        OnAddTile?.Invoke(tile);

        var t = pairTiles.Find(p => p.tile.Equals(tile));

        if(t == null)
        {
            var data = new BundleData(bundle.ID.Label, bundle.GetCharacteristics());
            pairTiles.Add(new PairTB(tile, data));
        }
        else
        {
            t.bData = new BundleData(bundle.ID.Label, bundle.GetCharacteristics());
        }
    }

    public override object Clone()
    {
        var dir = new List<PairTB>();
        foreach (var pair in pairTiles)
        {
            dir.Add(new PairTB(pair.tile, pair.bData));
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
    }

    public override void OnDetach(LBSLayer layer)
    {
        var tileMap = layer.GetModule<LBSTileMap>();
        tileMap.OnRemoveData -= RemoveTile;
        tileMap.OnAddData -= AddEmpty;
        OnAddTile -= tileMap.AddTile;
    }

    public override void Print()
    {
        throw new System.NotImplementedException();
    }

    public void RemoveTile(object tile)
    {
        var toR = tile as LBSTile;
        var xx = pairTiles.Find(x => x.tile == toR);
        pairTiles.Remove(xx);
    }

    public void AddEmpty(object tile)
    {
        var t = tile as LBSTile;
        var xx = pairTiles.Find(x => x.tile == t);
        if(xx != null)
        {
            RemoveTile(xx);
            pairTiles.Add(new PairTB(t, new BundleData()));
        }
        //if (pairTiles.ContainsKey(t))
        //    pairTiles.Add((t), new BundleData());
    }
}

[System.Serializable]
public class PairTB
{
    [SerializeField]
    public LBSTile tile;
    [SerializeField]
    public BundleData bData;

    public PairTB(LBSTile tile, BundleData bData)
    {
        this.tile = tile;
        this.bData = bData;
    }
}
