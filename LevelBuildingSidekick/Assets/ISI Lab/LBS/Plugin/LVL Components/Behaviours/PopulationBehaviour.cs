using LBS.Behaviours;
using LBS.Components;
using LBS.Components.TileMap;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[RequieredModule(typeof(TileMapModule), typeof(BundleTileMap))]
public class PopulationBehaviour : LBSBehaviour
{
    #region FIELDS
    [JsonIgnore]
    TileMapModule tileMap;
    [JsonIgnore]
    BundleTileMap bundleTileMap;
    #endregion

    public List<TileBundlePair> Tilemap => bundleTileMap.Tiles;

    public PopulationBehaviour(Texture2D icon, string name) : base(icon, name) { }

    #region METHODS
    public void AddTile(Vector2Int position, Bundle bundle)
    {
        var tile = new LBSTile(position, "Tile: " + position.ToString());
        tileMap.AddTile(tile);
        bundleTileMap.AddTile(tile, new BundleData(bundle));

        //Calculate connections
        //tileConnections.AddTile(tile, connections);
    }

    public void AddTile(Vector2Int position, BundleData bundle)
    {
        var tile = new LBSTile(position, "Tile: " + position.ToString());
        tileMap.AddTile(tile);
        bundleTileMap.AddTile(tile, bundle);

        //Calculate connections
        //tileConnections.AddTile(tile, connections);
    }

    public void RemoveTile(Vector2Int position)
    {
        var tile = tileMap.GetTile(position);
        tileMap.RemoveTile(tile);
        bundleTileMap.RemoveTile(tile);
    }

    public void SetBundle(LBSTile tile, Bundle bundle)
    {
        var t = bundleTileMap.GetTile(tile);
        t.BundleData = new BundleData(bundle);
    }

    public LBSTile GetTile(Vector2Int position)
    {
        return tileMap.GetTile(position);
    }

    public BundleData GetBundleData(LBSTile tile)
    {
        return bundleTileMap.GetTile(tile).BundleData;
    }

    public BundleData GetBundleData(Vector2 position)
    {
        return GetBundleData(tileMap.GetTile(position.ToInt()));
    }

    public override object Clone()
    {
        return new PopulationBehaviour(this.Icon, this.Name);
    }

    public override void OnAdd(LBSLayer layer)
    {
        Owner = layer;

        tileMap = Owner.GetModule<TileMapModule>();
        bundleTileMap = Owner.GetModule<BundleTileMap>();
    }
    #endregion
}
