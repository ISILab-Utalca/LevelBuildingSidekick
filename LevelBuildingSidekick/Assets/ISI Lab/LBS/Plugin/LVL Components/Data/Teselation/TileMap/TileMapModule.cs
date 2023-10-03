using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using LBS.Components.Teselation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace LBS.Components.TileMap
{
    [System.Serializable]
    public class TileMapModule : LBSModule , ISelectable
    {
        #region FIELDS
        [SerializeField, JsonRequired, SerializeReference]
        protected List<LBSTile> tiles = new List<LBSTile>();
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
            //this.tiles = new List<LBSTile>(tiles);
        }
        #endregion

        #region METHODS
        public virtual void AddTile(LBSTile tile)
        {
            var t = GetTile(tile.Position);
            if (t != null)
                tiles.Remove(t);

            tiles.Add(tile);
            OnAddTile?.Invoke(this, tile);
        }

        public void AddTiles(List<LBSTile> tiles)
        {
            foreach(var t in tiles)
            {
                AddTile(t);
            }
        }

        public LBSTile GetTile(Vector2Int pos)
        {
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
                return true;
            }
            return false;
        }

        public LBSTile RemoveAt(int index)
        {
            var tile = tiles[index];
            tiles.Remove(tile); 
            OnRemoveTile?.Invoke(this, tile);

            return tile;
        }

        public LBSTile RemoveAt(Vector2Int position)
        {
            var tile = GetTile(position);
            if (tile != null)
            {
                tiles.Remove(tile); 
                OnRemoveTile?.Invoke(this, tile);
            }
            return tile;
        }

        public void RemoveTiles(List<LBSTile> tiles)
        {
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

        /*
        public Vector2Int ToMatrixPosition(int index)
        {
            var r = GetBounds();
            return new Vector2Int((int)(index % r.width), (int)(index / r.width));
        }
        */

        /*
        public Vector2 ToWorldPosition(Vector2Int matrixPosition)
        {
            Vector2 worldPosition = new Vector2(
                matrixPosition.x * CellSize.x,
                matrixPosition.y * CellSize.y
            );
            return worldPosition;
        }
        */

        /*
        public int ToIndex(Vector2 matrixPosition)
        {
            var r = GetBounds();
            var pos = matrixPosition - r.position;
            return (int)(pos.y * r.width + pos.x);
        }
        */

        public LBSTile GetTileNeighbor(LBSTile tile, Vector2Int direction)
        {
            List<LBSTile> neighbors = new List<LBSTile>();
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

            //return IsEmpty() && tiles.Any(t => t.Position.x == (int)pos.x && t.Position.y == (int)pos.y);
        }

        public override void Clear()
        {
            tiles.Clear();
            //OnChanged?.Invoke(this);
        }

        public override void Rewrite(LBSModule other) // esto es necesario (??)
        {
            var module = other as TileMapModule;
            tiles.Clear();
            AddTiles(module.Tiles);
        }

        public override object Clone()
        {
            var tileMap = new TileMapModule();
            tileMap.tiles = tiles.Select(t => CloneRefs.Get(t)).Cast<LBSTile>().ToList();
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
            var r = new List<object>();
            var tile = GetTile(position);
            
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

        /*
        public override List<int> OccupiedIndexes()
        {
            return OccupiedPositions().Select(v => ToIndex(v)).ToList();
        }

        public override List<int> EmptyIndexes()
        {
            return EmptyPositions().Select(v => ToIndex(v)).ToList();
        }
        */

        /*
        public override List<Vector2> OccupiedPositions()
        {
            return tiles.Select(t => (Vector2)t.Position).ToList();
        }

        public override List<Vector2> EmptyPositions()
        {
            var r = GetBounds();
            var occupied = OccupiedPositions();

            List<Vector2> empty = new List<Vector2>();

            for(int j = 0; j < r.height; j++)
            {
                for(int i = 0; i < r.width; i++)
                {
                    var v = new Vector2(i, j);
                    if (!occupied.Contains(v))
                    {
                        empty.Add(v);
                    }
                }
            }

            return empty;
        }
        */
        #endregion
    }
}
