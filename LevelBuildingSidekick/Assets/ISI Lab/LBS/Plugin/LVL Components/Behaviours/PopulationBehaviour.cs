using LBS.Behaviours;
using LBS.Bundles;
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
    [SerializeField, JsonIgnore]
    TileMapModule tileMap;
    [SerializeField, JsonIgnore]
    BundleTileMap bundleTileMap;
    #endregion

    #region PROPERTIES
    [JsonIgnore]
    public List<TileBundlePair> Tilemap => bundleTileMap.Tiles;
    #endregion

    #region CONSTRUCTORS
    public PopulationBehaviour(Texture2D icon, string name) : base(icon, name) { }
    #endregion

    #region METHODS
    public void AddTile(Vector2Int position, Bundle bundle)
    {
        var tile = new LBSTile(position);
        tileMap.AddTile(tile);
        bundleTileMap.AddTile(tile, new BundleData(bundle), Vector2.right);

        //Calculate connections
        //tileConnections.AddTile(tile, connections);
    }

    public void AddTile(Vector2Int position, BundleData bundle)
    {
        var tile = new LBSTile(position);//, "Tile: " + position.ToString());
        tileMap.AddTile(tile);
        bundleTileMap.AddTile(tile, bundle, Vector2.right);

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

    public TileBundlePair GetTileBundle(Vector2Int position)
    {
        return bundleTileMap.GetTile(GetTile(position));
    }

    public BundleData GetBundleData(LBSTile tile)
    {
        return bundleTileMap.GetTile(tile).BundleData;
    }

    public void RotateTile(Vector2Int pos, Vector2 rotation)
    {
        var t = GetTileBundle(pos);
        if (t == null)
            return;
        t.Rotation = rotation;
    }

    public BundleData GetBundleData(Vector2 position)
    {
        return GetBundleData(tileMap.GetTile(position.ToInt()));
    }

    public override void OnAttachLayer(LBSLayer layer)
    {
        Owner = layer;

        tileMap = Owner.GetModule<TileMapModule>();
        bundleTileMap = Owner.GetModule<BundleTileMap>();
    }

    public override void OnDetachLayer(LBSLayer layer)
    {
        throw new System.NotImplementedException();
    }

    public override object Clone()
    {
        return new PopulationBehaviour(this.Icon, this.Name);
    }

    // override object.Equals
    public override bool Equals(object obj)
    {
        var other = obj as PopulationBehaviour;

        if (other == null) return false;

        return true;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
    #endregion
}
