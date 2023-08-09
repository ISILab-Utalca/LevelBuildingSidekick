using LBS.Behaviours;
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

    public BundleTileMap TileMap => bundleTileMap;

    public PopulationBehaviour(Texture2D icon, string name) : base(icon, name)
    {

    }

    #region METHODS
    public void CheckModules()
    {
        var tileMap = Owner.GetModule<TileMapModule>();
        var bundleTileMap = Owner.GetModule<BundleTileMap>();

        if (tileMap == null)
        {
            Debug.LogError("[ISILab] TileMap has been removed from Layer");
            return;
        }
        if (bundleTileMap == null)
        {
            Debug.LogError("[ISILab] Connections Module has been removed from Layer");
            return;
        }

        if (!tileMap.Equals(this.tileMap))
        {
            this.tileMap = tileMap;
        }
        if (!bundleTileMap.Equals(this.bundleTileMap))
        {
            this.bundleTileMap = bundleTileMap;
        }
    }

    public void AddTile(Vector2Int position, Bundle bundle)
    {
        CheckModules();
        var tile = new LBSTile(position, "Tile: " + position.ToString());
        tileMap.AddTile(tile);
        bundleTileMap.AddTile(tile, new BundleData(bundle));

        //Calculate connections
        //tileConnections.AddTile(tile, connections);
    }

    public void RemoveTile(Vector2Int position)
    {
        CheckModules();
        var tile = tileMap.GetTile(position);
        tileMap.RemoveTile(tile);
        bundleTileMap.RemoveTile(tile);
    }

    public void SetBundle(LBSTile tile, Bundle bundle)
    {
        CheckModules();
        var t = bundleTileMap.GetTile(tile);
        t.BundleData = new BundleData(bundle);
    }

    public LBSTile GetTile(Vector2Int position)
    {
        CheckModules();
        return tileMap.GetTile(position);
    }

    public BundleData GetBundleData(LBSTile tile)
    {
        CheckModules();
        return bundleTileMap.GetTile(tile).BundleData;
    }

    public BundleData GetBundleData(Vector2 position)
    {
        CheckModules();
        return GetBundleData(tileMap.GetTile(position.ToInt()));
    }

    public override object Clone()
    {
        return new PopulationBehaviour(this.Icon, this.Name);
    }
    #endregion
}
