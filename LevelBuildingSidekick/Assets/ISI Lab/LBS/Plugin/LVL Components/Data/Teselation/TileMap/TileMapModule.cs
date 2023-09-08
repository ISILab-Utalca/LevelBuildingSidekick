using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using LBS.Components.Teselation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
        public virtual bool AddTile(LBSTile tile)
        {
            var t = GetTile(tile.Position);
            if (t != null)
                tiles.Remove(t);

            tiles.Add(tile);
            OnChanged?.Invoke(this);
            return true;
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

            //var tile = tiles.Find(t => t.Position == pos);
            //return tile;
        }

        public LBSTile GetTile(int index)
        {
            return tiles[index];
        }

        public bool RemoveTile(LBSTile tile)
        {
            if(tiles.Remove(tile))
            {
                OnChanged?.Invoke(this);
                return true;
            }
            return false;
        }

        public LBSTile RemoveAt(int index)
        {
            var t = tiles[index];
            tiles.Remove(t);
            OnChanged?.Invoke(this);
            return t;
        }

        public LBSTile RemoveAt(Vector2Int position)
        {
            var tile = GetTile(position);
            if (tile != null)
            {
                tiles.Remove(tile);
                OnChanged?.Invoke(this);
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
            return (tiles.Count <= 0);
        }

        public override Rect GetBounds()
        {
            if (tiles == null || tiles.Count == 0)
            {
                //Debug.LogWarning("Esta tilemap no tiene tiles!!!");
                return new Rect(Vector2.zero, Vector2.zero);
            }

            var x = tiles.Min(t => t.Position.x);
            var y = tiles.Min(t => t.Position.y);
            var width = tiles.Max(t => t.Position.x) - x + 1;
            var height = tiles.Max(t => t.Position.y) - y + 1;
            return new Rect(x, y, width, height);
        }

        public Vector2Int ToMatrixPosition(int index)
        {
            var r = GetBounds();
            return new Vector2Int((int)(index % r.width), (int)(index / r.width));
        }

        public Vector2 ToWorldPosition(Vector2Int matrixPosition)
        {
            Vector2 worldPosition = new Vector2(
                matrixPosition.x * CellSize.x,
                matrixPosition.y * CellSize.y
            );
            return worldPosition;
        }

        public int ToIndex(Vector2 matrixPosition)
        {
            var r = GetBounds();
            var pos = matrixPosition - r.position;
            return (int)(pos.y * r.width + pos.x);
        }

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

        public bool Contains(Vector2 pos)
        {
            return IsEmpty() && tiles.Any(t => t.Position.x == (int)pos.x && t.Position.y == (int)pos.y);
        }

        public override void Clear()
        {
            tiles.Clear();
            OnChanged?.Invoke(this);
        }

        public override void Rewrite(LBSModule module)
        {
            if (module == null)
            {
                return;
            }

            var tm = module as TileMapModule;

            if (tm == null)
            {
                return;
            }

            tiles.Clear();

            AddTiles(tm.Tiles);
        }

        public override object Clone()
        {
            var tileMap = new TileMapModule();
            tileMap.tiles = tiles.Select(t => t.Clone() as LBSTile).ToList();
            return tileMap;
        }

        public override void Print()
        {
            string s = "TileMap Module: " + ID + "\n\n";
            foreach (var t in tiles)
            {
                s += "t - " + t.Position + "\n";
            }
            Debug.Log(s);
        }

        public override void OnAttach(LBSLayer layer)
        {
            Owner = layer;
        }

        public override void OnDetach(LBSLayer layer)
        {
            Owner = null;
        }

        public override void Reload(LBSLayer layer)
        {
            Owner = layer;
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

        /*
        public override List<int> OccupiedIndexes()
        {
            return OccupiedPositions().Select(v => ToIndex(v)).ToList();
        }

        public override List<int> EmptyIndexes()
        {
            return EmptyPositions().Select(v => ToIndex(v)).ToList();
        }*/

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
        }*/

        #endregion
    }
}

