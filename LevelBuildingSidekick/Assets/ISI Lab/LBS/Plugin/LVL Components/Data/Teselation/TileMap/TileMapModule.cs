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
        //FIELDS
        [SerializeField, JsonRequired, SerializeReference]
        protected List<LBSTile> tiles;

        //PROEPRTIES
        [JsonIgnore]
        public Rect Rect
        {
            get
            {
                if (tiles == null || tiles.Count == 0)
                {
                    return new Rect(Vector2.zero, Vector2.zero);
                }
                var x = tiles.Min(t => t.Position.x);
                var y = tiles.Min(t => t.Position.y);
                var width = tiles.Max(t => t.Position.x) - x;
                var height = tiles.Max(t => t.Position.y) - y;
                return new Rect(x, y, width, height);
            }
        }

        [JsonIgnore]
        public Vector2 Origin
        {
            get => Rect.position;
            set
            {
                var offset = value - Rect.position;
                foreach(var t in tiles)
                {
                    t.Position += offset.ToInt();
                }
            }
        }


        [JsonIgnore]
        public new Vector2 CellSize
        {
            get => TeselationModule.CellSize;
        }

        [JsonIgnore]
        public Vector2 Size => Rect.size;

        [JsonIgnore]
        public int Width => (int)Size.x;

        [JsonIgnore]
        public int Height => (int)Size.y;

        [JsonIgnore]
        public int TileCount => tiles.Count;

        [JsonIgnore]
        public List<LBSTile> Tiles => new List<LBSTile>(tiles);

        public TileMapModule()
        {
            tiles = new List<LBSTile>();
        }
        public TileMapModule(List<LBSTile> tiles)
        {
            this.tiles = tiles;
        }

        public virtual bool AddTile(T tile)
        {
            tile.Position = SnapPosition(tile.Position).ToInt();
            var t = GetTile(tile.Position);
            if (t != null)
            {
                tiles.Remove(t);
            }
            tiles.Add(tile);
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

        public List<T> GetTileNeighbors(T tile, List<Vector2> directions)
        {
            List<T> neighbors = new List<T>();
            foreach(var v in directions)
            {
                var pos = tile.Position + new Vector2(v.x * CellSize.x, v.y * CellSize.y);
                pos = SnapPosition(pos);
                var neigh = tiles.Find(t => t.Position == pos) as T;
                neighbors.Add(neigh);
            }
            return neighbors;
        }

        public bool RemoveTile(T tile)
        {
            return tiles.Remove(tile);
        }

        public T RemoveAt(int index)
        {
            if (!tiles.ContainsIndex(index))
            {
                return null;
            }
            var t = tiles[index] as T;
            tiles.Remove(t);
            return t;
        }

        public T RemoveAt(Vector2Int position)
        {
            var tile = GetTile(position);
            if (tile != null)
            {
                tiles.Remove(tile);
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
            var rect = Rect;
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
        }

        public override void Print()
        {
            throw new System.NotImplementedException();
        }

        public override object Clone()
        {
            throw new System.NotImplementedException();
        }
    }

    [System.Serializable]
    public class LBSBaseTileMap : TileMapModule<LBSTile> { }
}

