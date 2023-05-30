using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using LBS.Components.Teselation;
using Newtonsoft.Json;

namespace LBS.Components.TileMap
{
    [System.Serializable]
    public class TileMapModule<T> : TeselationModule where T : LBSTile
    {
        #region FIELDS

        [SerializeField, JsonRequired, SerializeReference]
        protected List<LBSTile> tiles = new List<LBSTile>();

        #endregion

        #region PROEPRTIES

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
            Key = GetType().Name;
        }

        public TileMapModule(List<T> tiles, string key) : base(key)
        {
            AddTiles(tiles);
            //this.tiles = new List<LBSTile>(tiles);
        }

        #endregion

        #region METHODS

        public virtual bool AddTile(T tile)
        {
            var t = GetTile(tile.Position);
            if (t != null)
                tiles.Remove(t);

            tiles.Add(tile as LBSTile);
            OnAddData?.Invoke(tile);
            OnChanged?.Invoke(this);
            return true;
        }

        public void AddTiles(List<T> tiles)
        {
            foreach(var t in tiles)
            {
                AddTile(t);
            }
        }

        public T GetTile(Vector2Int pos)
        {
            var tile = tiles.Find(t => t.Position == pos) as T;
            return tile;
        }

        public T GetTile(int index)
        {
            return tiles[index] as T;
        }

        public bool Contains(Vector2 pos)
        {
            return tiles.Any(t => t.Position.x == (int)pos.x && t.Position.y == (int)pos.y);
        }

        public List<T> GetTileNeighbors(T tile, List<Vector2Int> directions)
        {
            List<T> neighbors = new List<T>();
            for (int i = 0; i < directions.Count; i++)
            {
                var nei = GetTileNeighbor(tile, directions[i]);
                neighbors.Add(nei);
            }
           
            return neighbors;
        }

        public T GetTileNeighbor(T tile, Vector2Int direction)
        {
            List<T> neighbors = new List<T>();
            var pos = tile.Position + direction;
            return this.GetTile(pos);
        }

        public bool RemoveTile(T tile)
        {
            if(tiles.Remove(tile))
            {
                OnRemoveData?.Invoke(tile);
                OnChanged?.Invoke(this);
                return true;
            }
            return false;
        }

        public T RemoveAt(int index)
        {
            if (!tiles.ContainsIndex(index))
            {
                return null;
            }
            var t = tiles[index] as T;
            tiles.Remove(t);
            OnRemoveData?.Invoke(t);
            OnChanged?.Invoke(this);
            return t;
        }

        public T RemoveAt(Vector2Int position)
        {
            var tile = GetTile(position);
            if (tile != null)
            {
                tiles.Remove(tile);
                OnRemoveData?.Invoke(tile);
                OnChanged?.Invoke(this);
            }
            return tile;
        }

        public void RemoveTiles(List<T> tiles)
        {
            foreach(var t in tiles)
            {
                RemoveTile(t);
            }
        }

        public T[,] ToMatrix()
        {
            var rect = GetBounds();
            var matrixIDs = new T[(int)rect.width, (int)rect.height];
            foreach (var tile in tiles)
            {
                var p = tile.Position;
                var pos = p - rect.min;
                matrixIDs[(int)pos.x, (int)pos.y] = tile as T;
            }
            return matrixIDs;
        }

        public override void Clear()
        {
            tiles.Clear();
            OnChanged?.Invoke(this);
        }

        public override void Print()
        {
            string s = "TileMap Module: " + Key + "\n\n";
            foreach(var t in tiles)
            {
                s += "t - " + t.Position + "\n";
            }
            Debug.Log(s);
        }

        public override bool IsEmpty()
        {
            return (tiles.Count() <= 0);
        }

        public override object Clone()
        {
            var tileMap = new TileMapModule<T>();
            tileMap.tiles = tiles.Select(t => t.Clone() as LBSTile).ToList(); //new List<LBSTile>(tiles);
            return tileMap;
        }

        public override void OnAttach(LBSLayer layer)
        {

        }

        public override void OnDetach(LBSLayer layer)
        {

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

        public override List<int> OccupiedIndexes()
        {
            return OccupiedPositions().Select(v => ToIndex(v)).ToList();
        }

        public override List<int> EmptyIndexes()
        {
            return EmptyPositions().Select(v => ToIndex(v)).ToList();
        }

        public override void Rewrite(LBSModule module)
        {
            throw new System.NotImplementedException();
        }

        public override void OnReload(LBSLayer layer)
        {

        }

        #endregion
    }
}

