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
        List<T> tiles;

        //PROEPRTIES
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

        public Vector2 Size => Rect.size;

        public int Width => (int)Size.x;

        public int Height => (int)Size.y;

        public int TileCount => tiles.Count;

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
            var tile = tiles.Find(t => t.Position == pos);
            return tile;
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
            var t = tiles[index];
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
                matrixIDs[(int)pos.x, (int)pos.y] = tile;
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

