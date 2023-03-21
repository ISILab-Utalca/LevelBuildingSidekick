using LBS.Components;
using LBS.Components.TileMap;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaggedTileMap : LBSModule
{
    [SerializeField, JsonRequired, SerializeReference]
    public Dictionary<LBSTile, BundleData> tiles;

    public Rect Rect => new Rect();
    
    [JsonIgnore]
    protected Func<LBSTile, bool> OnAddTile;

    public TaggedTileMap() : base() 
    { 
        Key = GetType().Name;
        tiles = new Dictionary<LBSTile, BundleData>();
    }

    public TaggedTileMap(string key, Dictionary<LBSTile, BundleData> tiles) : base(key)
    {
        this.tiles = tiles;
    }

    public override void Clear()
    {
        tiles.Clear();
    }

    public void AddTile(LBSTile tile, Bundle bundle)
    {
        OnAddTile?.Invoke(tile);
        var data = new BundleData(bundle.ID.Label, bundle.GetCharacteristics());
        tiles.Add(tile, data);
    }

    public override object Clone()
    {
        var dir = new Dictionary<LBSTile, string>();
        foreach(var k in tiles.Keys)
        {
            dir.Add(k.Clone() as LBSTile, tiles[k].Clone() as string);
        }
        return  new TaggedTileMap(key, new Dictionary<LBSTile, BundleData>(tiles));
    }

    public override bool IsEmpty()
    {
        return tiles.Count == 0;
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
        tiles.Remove(tile as LBSTile);
    }

    public void AddEmpty(object tile)
    {
        var t = tile as LBSTile;
        if(tiles.ContainsKey(t))
            tiles.Add((t), new BundleData());
    }
}

