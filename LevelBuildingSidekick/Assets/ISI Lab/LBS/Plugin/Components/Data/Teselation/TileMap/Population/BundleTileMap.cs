using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ISILab.Extensions;
using LBS.Components;
using LBS.Components.TileMap;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Modules
{
    public class BundleTileMap : LBSModule, ISelectable
    {
        #region FIELDS
        [SerializeField, JsonRequired, SerializeReference]
        protected List<TileBundleGroup> groups = new List<TileBundleGroup>();
        #endregion

        #region PROPERTIES
        [JsonIgnore]
        public List<TileBundleGroup> Groups => new List<TileBundleGroup>(groups);
        #endregion

        #region CONSTRUCTORS
        public BundleTileMap() : base()
        {
            id = GetType().Name;
        }

        //This one has to be compatible with groups so let's see how I'll make groups work
        public BundleTileMap(IEnumerable<TileBundlePair> tiles, string id = "ConnectedTileMapModule") : base(id)
        {
            foreach (var t in tiles)
            {
                AddTile(t);
            }
        }
        #endregion

        #region METHODS
        //For a bundle to add a tile, it should always add the entire group.
        //Check bundle size -> create group -> check tile location and add all tiles according to size.

        //TODO: Need to be able to check if a tile is in a bundle group. "foreach(TileBundleGroup group in groups) -> foreach(LBSTile tile in group.tileGroup)"
        public void AddTile(TileBundlePair tile)
        {
            var t = GetTile(tile.Tile);
            if (t != null)
            {
                tiles.Remove(t);
            }

            OnChanged?.Invoke(this, null, new List<object>() { tile });

            //Generate according to tilesize
            Vector2Int newTileSize = tile.GetBundleSize();
            Debug.Log("Tile size: X "+ newTileSize.x + "/ Y" + newTileSize.y);

            for(int i=0; i<newTileSize.x; i++)
            {
                for (int j = 0; j < newTileSize.y; j++)
                {

                }
            }
            Debug.Log("adding tile at " + tile.Tile.Position);
            tiles.Add(tile);

            Debug.Log("adding population tile");

        }

        public void AddTile(LBSTile tile, BundleData bundleData, Vector2 rotation) => AddTile(new TileBundlePair(tile, bundleData, rotation));

        //First of all, the option to create a group.
        //It needs a starting position, so either a tile or a position!
        public void AddGroup(TileBundleGroup group)
        {

        }

        public void AddGroup(LBSTile tile, BundleData bundleData, Vector2 rotation) => AddGroup(new TileBundleGroup(new List<LBSTile> { tile }, bundleData, rotation) );
        //This one starts it from a position.
        public void AddGroup(Vector2Int position, BundleData bundleData, Vector2 rotation) => AddGroup(new TileBundleGroup(new List<LBSTile> { new LBSTile(position) }, bundleData, rotation) );
          

        public TileBundlePair GetTile(LBSTile tile)
        {
            if (tiles.Count <= 0)
                return null;
            return tiles.Find(t => t.Tile.Equals(tile));

        }

        public TileBundlePair GetTile(Vector2 pos)
        {
            if (tiles.Count <= 0)
                return null;
            return tiles.Find(t => t.Tile.Position == pos);

        }

        public void RemoveTile(LBSTile tile)
        {
            var t = GetTile(tile);

            OnChanged?.Invoke(this, new List<object>() { t }, null);

            tiles.Remove(t);
        }

        public void RemoveTile(int index) => RemoveTile(tiles[index].Tile);

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

            OnChanged?.Invoke(this, new List<object>(tiles), new List<object>(map.tiles));

            Clear();
            foreach (var t in map.tiles)
            {
                AddTile(t);
            }
        }

        public List<object> GetSelected(Vector2Int position)
        {
            var pos = Owner.ToFixedPosition(position);
            var r = new List<object>();
            var tile = GetTile(pos);

            if (tile != null)
            {
                r.Add(tile.BundleData);
            }

            return r;
        }

        public override bool Equals(object obj)
        {
            var other = obj as BundleTileMap;

            if (other == null) return false;

            var tCount = other.tiles.Count;

            if (tCount != this.tiles.Count) return false;

            for (int i = 0; i < tCount; i++)
            {
                var t1 = other.tiles[i];
                var t2 = this.tiles[i];

                if (!t1.Equals(t2)) return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }

    //deprecated lol
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

        [JsonIgnore]
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

        public Vector2Int GetBundleSize()
        {
            return bData.Bundle.TileSize;
        }

        public object Clone()
        {
            return new TileBundlePair(tile.Clone() as LBSTile, bData.Clone() as BundleData, rotation);
        }

        public override bool Equals(object obj)
        {
            var other = obj as TileBundlePair;

            if (other == null) return false;

            if (!this.tile.Equals(other.tile)) return false;

            if (!this.bData.Equals(other.bData)) return false;

            if (!this.rotation.Equals(other.rotation)) return false;

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    /// <summary>
    /// So TileBundlePairs are a little too limited in functionality since they only link a tile to bundle data and that's it.
    /// I made a reworked version that SHOULD allow for multiple tiles to be stored with an ID.
    /// </summary>
    [System.Serializable]
    public class TileBundleGroup : ICloneable
    {
        [SerializeField, JsonRequired]
        List<LBSTile> tileGroup = new List<LBSTile>();
        [SerializeField, JsonRequired]
        BundleData bData;
        [SerializeField, JsonRequired]
        Vector2 rotation;

        #region PROPERTIES
        [JsonIgnore]
        public List<LBSTile> TileGroup
        {
            get => tileGroup;
            set => tileGroup = value;
        }

        [JsonIgnore]
        public BundleData BundleData
        {
            get => bData;
            set => bData = value;
        }

        [JsonIgnore]
        public Vector2 Rotation
        {
            get => rotation;
            set => rotation = value;
        }
        public TileBundleGroup(List<LBSTile> tiles, BundleData bData, Vector2 rotation)
        {
            this.tileGroup = tiles;
            this.bData = bData;
            this.rotation = rotation;
        }
        #endregion

        #region METHODS
        public Vector2Int GetBundleSize()
        {
            return bData.Bundle.TileSize;
        }

        public object Clone()
        {
            List<LBSTile> clonedTileGroup = new List<LBSTile>();
            foreach (LBSTile tile in tileGroup)
            {
                clonedTileGroup.Add(tile.Clone() as LBSTile);
            }
            return new TileBundleGroup(clonedTileGroup, bData.Clone() as BundleData, rotation);
        }

        public override bool Equals(object obj)
        {
            var other = obj as TileBundleGroup;

            if (other == null) return false;

            if (tileGroup.Count != other.tileGroup.Count) return false;

            for (int i = 0; i < tileGroup.Count; i++)
            {
                if (!tileGroup[i].Equals(other.tileGroup[i])) return false;
            }

            if (!this.bData.Equals(other.bData)) return false;

            if (!this.rotation.Equals(other.rotation)) return false;

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}

