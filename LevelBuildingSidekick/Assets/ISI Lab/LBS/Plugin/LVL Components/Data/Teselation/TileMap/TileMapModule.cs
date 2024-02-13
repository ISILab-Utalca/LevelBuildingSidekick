using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using ISILab.Extensions;
using ISILab.LBS.Modules;
using LBS.Components.TileMap;

namespace ISILab.LBS.Modules
{
    [System.Serializable]
    public class TileMapModule : LBSModule , ISelectable
    {
        #region FIELDS
        [SerializeField, JsonRequired, SerializeReference]
        private List<LBSTile> tiles = new List<LBSTile>();
        private Dictionary<Vector2Int,LBSTile> _tileDic = new Dictionary<Vector2Int, LBSTile>();
        #endregion

        #region PROEPRTIES
        [JsonIgnore]
        public Vector2 CellSize
        {
            get => Owner.TileSize;
        }

        [JsonIgnore]
        public Vector2 Origin
        {
            get => GetBounds().position;
            set
            {
                var offset = value - GetBounds().position;
                foreach(var t in tiles)
                {
                    t.Position += offset.ToInt();
                }
            }
        }

        [JsonIgnore]
        public Vector2 Centroid
        {
            get => GetBounds().center;
            set
            {
                foreach (var t in tiles)
                {
                    t.Position += new Vector2Int((int)value.x, (int)value.y);
                }
            }
        }

        [JsonIgnore]
        public Vector2 Size => GetBounds().size;

        [JsonIgnore]
        public int Width => (int)Size.x;

        [JsonIgnore]
        public int Height => (int)Size.y;

        [JsonIgnore]
        public int TileCount => tiles.Count;

        [JsonIgnore]
        public List<LBSTile> Tiles => new List<LBSTile>(tiles);

        #endregion

        #region EVENTS
        public event Action<TileMapModule,LBSTile> OnAddTile;
        public event Action<TileMapModule,LBSTile> OnRemoveTile;
        #endregion

        #region CONSTRUCTOR
        public TileMapModule() : base()
        {
            tiles = new List<LBSTile>();
            ID = GetType().Name;
        }

        public TileMapModule(IEnumerable<LBSTile> tiles, string key) : base(key)
        {
            foreach(var t in tiles)
            {
                AddTile(t);
            }
        }
        #endregion

        #region METHODS
        public virtual void AddTile(LBSTile tile)
        {
            var t = GetTile(tile.Position);
            if (t != null)
                tiles.Remove(t);

            tiles.Add(tile);
            
            _tileDic[tile.Position] = tile;

            OnChanged?.Invoke(this, new List<object>() { t }, new List<object>() { tile });
            OnAddTile?.Invoke(this, tile);
        }

        public void AddTiles(List<LBSTile> tiles)
        {
            //OnChanged?.Invoke(this, null, tiles.Cast<object>().ToList());
            foreach(var t in tiles)
            {
                AddTile(t);
            }
        }

        public LBSTile GetTile(Vector2Int pos)
        {
            //_tileDic.TryGetValue(pos, out LBSTile tile);
            //return tile;

            foreach (var tile in tiles)
            {
                if (tile.Position == pos)
                    return tile;
            }
            return null;
        }

        public LBSTile GetTileAt(int index)
        {
            return tiles[index];
        }

        public bool RemoveTile(LBSTile tile)
        {
            
            if(tiles.Remove(tile))
            {
                OnRemoveTile?.Invoke(this, tile);
                _tileDic.Remove(tile.Position);

                OnChanged?.Invoke(this, new List<object>() { tile }, null);
                return true;
            }
            return false;
        }

        public LBSTile RemoveAt(int index)
        {
            var tile = tiles[index];
            tiles.Remove(tile); 
            _tileDic.Remove(tile.Position);
            OnRemoveTile?.Invoke(this, tile);
            OnChanged?.Invoke(this, new List<object>() { tile }, null);
            return tile;
        }

        public LBSTile RemoveAt(Vector2Int position)
        {
            var tile = GetTile(position);
            if (tile != null)
            {
                tiles.Remove(tile);
                _tileDic.Remove(tile.Position);
                OnRemoveTile?.Invoke(this, tile);
            }
            return tile;
        }

        public void RemoveTiles(List<LBSTile> tiles)
        {
            //OnChanged?.Invoke(this, tiles.Cast<object>().ToList(), null);
            foreach(var t in tiles)
            {
                RemoveTile(t);
            }
        }

        public override bool IsEmpty()
        {
            return tiles.Count <= 0;
        }

        public override Rect GetBounds()
        {
            if (tiles.Count == 0)
            {
                return default(Rect);
            }

            return tiles.GetBounds();
        }

        public LBSTile GetTileNeighbor(LBSTile tile, Vector2Int direction)
        {
            var pos = tile.Position + direction;
            return this.GetTile(pos);
        }

        public List<LBSTile> GetTileNeighbors(LBSTile tile, List<Vector2Int> directions)
        {
            List<LBSTile> neighbors = new List<LBSTile>();
            for (int i = 0; i < directions.Count; i++)
            {
                var nei = GetTileNeighbor(tile, directions[i]);
                neighbors.Add(nei);
            }

            return neighbors;
        }

        public bool Contains(Vector2Int pos)
        {
            if (IsEmpty())
                return false;

            foreach (var tile in tiles)
            {
                if(tile.Position.Equals(pos))
                    return true;
            }

            return false;
        }

        public override void Clear()
        {
            tiles.Clear();
            _tileDic.Clear();
            //OnChanged?.Invoke(this);
        }

        public override void Rewrite(LBSModule other)
        {
            var module = other as TileMapModule;
            Clear();
            AddTiles(module.Tiles);
        }

        public override object Clone()
        {
            var tileMap = new TileMapModule();
            var newTiles = tiles.Select(t => CloneRefs.Get(t)).Cast<LBSTile>().ToList();
            tileMap.AddTiles(newTiles);
            return tileMap;
        }

        public override void Print()
        {
            string msg = "";
            msg += "Type: " + GetType() + "\n";
            msg += "Hash code: " + GetHashCode() + "\n";
            msg += "ID: " + ID + "\n";
            msg += "\n";
            foreach (var t in tiles)
            {
                msg += t.Position + "\n";
            }
            Debug.Log(msg);
        }

        public List<object> GetSelected(Vector2Int position)
        {
            var pos = Owner.ToFixedPosition(position);

            var r = new List<object>();
            var tile = GetTile(pos);
            
            if (tile != null)
            {
                r.Add(tile);
            }
            return r;
        }

        public override bool Equals(object obj)
        {
            var other = obj as TileMapModule;

            if (other == null) return false;

            var tileCount = other.tiles.Count;

            if(this.tiles.Count != tileCount) return false;

            for (int i = 0; i < tileCount; i++)
            {
                var t1 = this.tiles[i];
                var t2 = other.tiles[i];

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
}
