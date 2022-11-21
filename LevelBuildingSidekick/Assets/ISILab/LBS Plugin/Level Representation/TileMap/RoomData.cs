using LBS.Schema;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LBS.Representation.TileMap
{

    [System.Serializable]
    public class RoomData : ICloneable
    {
        // Fields
        [SerializeField, JsonRequired]
        private string id; // laber or ID
        [SerializeField, JsonRequired]
        private List<Vector2Int> tilesPos = new List<Vector2Int>(); // (!) podrian ser Vector3int para mapas 3D
        [SerializeField, JsonRequired]
        private string color;

        // Metainfo
        [JsonIgnore]
        internal Vector2Int surface; // esto podria ir en un controlador y no directamente en la data (??)
        [JsonIgnore]
        private RectInt? rect; // esto podria ir en un controlador y no directamente en la data (??)

        // Properties
        [JsonIgnore]
        public Vector2Int Centroid => new Vector2Int( tilesPos.Sum(tp => tp.x), tilesPos.Sum(tp => tp.y)/ tilesPos.Count);
        [JsonIgnore]
        public Vector2Int Size => GetRect().size;
        [JsonIgnore]
        public int Width => GetRect().width;
        [JsonIgnore]
        public int Height => GetRect().height;
        [JsonIgnore]
        public string ID => this.id;
        [JsonIgnore]
        public Color Color { 
            get => Parse.StrToColor(color);
            set => color = Parse.ColorTosStr(value);
        }
        [JsonIgnore]
        public List<Vector2Int> Tiles => new List<Vector2Int>(tilesPos);
        [JsonIgnore]
        public int TilesCount => tilesPos.Count;

        /// <summary>
        /// Empty constructor.
        /// </summary>
        public RoomData() { }

        internal RoomData(List<Vector2Int> tiles,Color color, string id)
        {
            this.id = id;
            this.tilesPos = tiles;
            this.color = Parse.ColorTosStr(color);
            //this.color = Parse.ColorTosStr( new Color(UnityEngine.Random.Range(0f,1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f)));
        }

        public object Clone()
        {
            var tiles = new List<Vector2Int>();
            foreach (var t in this.tilesPos)
            {
                var tilePos = new Vector2Int(t.x,t.y);
                
                tiles.Add(tilePos);
            }
            var clone = new RoomData(tiles,this.Color, this.id);
            return clone;
        }

        internal bool Contains(TileData data)
        {
            return tilesPos.Find(t => t.Equals(data.Position)) != null;
        }

        internal bool Contains(Vector2Int pos)
        {
            return tilesPos.Find(tp => tp.Equals(pos)) != null;
        }

        /// <summary>
        /// Add the tile delivered by parameters, if have it does nothing.
        /// </summary>
        /// <param name="tile"></param>
        internal void AddTile(Vector2Int tile)
        {
            if (!tilesPos.Contains(tile))
            {
                tilesPos.Add(tile);
            }
        }

        /// <summary>
        /// Remove tile delivered by parameters, if dont have it does nothing.
        /// </summary>
        /// <param name="tile"></param>
        internal void RemoveTile(Vector2Int tile)
        {
            if (tilesPos.Contains(tile))
            {
                tilesPos.Remove(tile);
            }
        }

        internal RectInt GetRect()
        {

            Vector2Int max = new Vector2Int(int.MinValue, int.MinValue);
            Vector2Int min = new Vector2Int(int.MaxValue, int.MaxValue);
            foreach (var tilePos in tilesPos)
            {
                var pos = tilePos;
                if (pos.x > max.x)
                    max.x = pos.x;
                if (pos.y > max.y)
                    max.y = pos.y;

                if (pos.x < min.x)
                    min.x = pos.x;
                if (pos.y < min.y)
                    min.y = pos.y;
            }
            rect = new RectInt(min, max - min + new Vector2Int(1, 1));
            return (RectInt)rect;
        }

        [Obsolete("this method is deprecated, instead use the 'Raatio' property instead")]
        internal float GetRatio()
        {
            rect = GetRect();
            return ((RectInt)rect).width / ((RectInt)rect).height;
        }

        [Obsolete("this method is deprecated, instead use the 'Width' property instead")]
        internal int GetWidth()
        {
            rect = GetRect();
            return ((RectInt)rect).width;
        }

        [Obsolete("this method is deprecated, instead use the 'Height' property instead")]
        internal int GetHeight()
        {
            rect = GetRect();
            return ((RectInt)rect).height;
        }

        internal List<Vector2Int> GetConvexCorners() // (??)  esto solo funciona para "4 conected", deberia estar en una clase aparte?, si en la clase de las tablas del gabo
        {
            var corners = new List<Vector2Int>();
            foreach (var currentPos in tilesPos)
            {
                var s = CalcNeighValueTile(currentPos);
                if (s != 0)
                {
                    if (s % 3 == 0 || s == 7 || s == 11 || s == 13 || s == 14)
                        corners.Add(currentPos);
                    continue;
                }
            }
            return corners;
        }

        private int CalcNeighValueTile(Vector2Int position) // (!) el nombre es malisimo mejorar, esta tambien es de la clase de las tablas del gabo
        {
            var value = 0;
            for (int i = 0; i < Directions.sidedirs.Length; i++)
            {
                var otherPos = position + Directions.sidedirs[i];
                if (!tilesPos.Contains(otherPos))
                {
                    value += Mathf.RoundToInt(Mathf.Pow(2, i));
                }
            }

            return value;
        }

        internal List<Vector2Int> GetConcaveCorners() // (!) Tambien es de la clase de las tablas del gabo 
        {
            var corners = new List<Vector2Int>();
            foreach (var currentPos in tilesPos)
            {
                var s = CalcNeighValueTile(currentPos);
                if (s != 0)
                    continue;

                for (int i = 0; i < Directions.diagdirs.Length; i++)
                {
                    var otherPos = currentPos  + Directions.diagdirs[i];
                    if (tilesPos.Contains(otherPos))
                        continue;

                    var other1 = new Vector2Int(currentPos.x + Directions.diagdirs[i].x, currentPos.y);
                    if (!corners.Contains(other1))
                    {
                        corners.Add(other1);
                    }

                    var other2 = new Vector2Int(currentPos.x, currentPos.y + Directions.diagdirs[i].y);
                    if (!corners.Contains(other2))
                    {
                        corners.Add(other2);
                    }
                }
            }
            return corners;
        }

        internal List<WallData> GetVerticalWalls() // (!) Tambien es de la clase de las tablas del gabo 
        {
            var walls = new List<WallData>();

            var convexCorners = GetConvexCorners();
            var allCorners = GetConcaveCorners();
            allCorners.AddRange(convexCorners);

            foreach (var current in convexCorners)
            {
                Vector2Int? other = null;
                int lessDist = int.MaxValue;
                foreach (var candidate in allCorners)
                {
                    if (current == candidate)
                        continue;

                    var pos = current;
                    if (pos.x - candidate.x != 0)
                        continue;

                    var dist = Mathf.Abs(pos.y - candidate.y);
                    if (dist < lessDist)
                    {
                        lessDist = dist;
                        other = candidate;
                    }
                }

                if (other == null)
                    other = current;

                if (walls.Any(w => (w.First == other) && (w.Last == current)))
                    continue;

                var wallTiles = new List<Vector2Int>();
                var oth = (Vector2Int)other;
                var end = Mathf.Max(current.y, oth.y);
                var start = Mathf.Min(current.y, oth.y);
                for (int i = 0; i <= end- start; i++)
                {
                    wallTiles.Add(new Vector2Int(current.x, start + i));
                }
                var dir = (current.x >= Centroid.x) ? Vector2Int.right : Vector2Int.left;

                var wall = new WallData(this.id, dir, wallTiles);
                walls.Add(wall);
            }
            return walls;
        }

        internal List<WallData> GetHorizontalWalls() // (!) Tambien es de la clase de las tablas del gabo 
        {
            var walls = new List<WallData>();

            var convexCorners = GetConvexCorners();
            var allCorners = GetConcaveCorners();
            allCorners.AddRange(convexCorners);

            foreach (var current in convexCorners)
            {
                Vector2Int? other = null;
                int lessDist = int.MaxValue;
                foreach (var candidate in allCorners)
                {
                    if (current == candidate)
                        continue;

                    var pos = current;
                    if (pos.y - candidate.y != 0)
                        continue;

                    var dist = Mathf.Abs(pos.x - candidate.x);
                    if (dist < lessDist)
                    {
                        lessDist = dist;
                        other = candidate;
                    }
                }

                if (other == null)
                    other = current;

                if (walls.Any(w => (w.First == other) && (w.Last == current) ))
                    continue;

                var wallTiles = new List<Vector2Int>();
                var oth = (Vector2Int)other;
                var end = Mathf.Max(current.x, (int)oth.x);
                var start = Mathf.Min(current.x, (int)oth.x);
                for (int i = 0; i <= end - start; i++)
                {
                    wallTiles.Add(new Vector2Int(start + i, current.y));
                }
                var dir = (current.y >= Centroid.y) ? Vector2Int.up : Vector2Int.down;
                var wall = new WallData(this.id, dir, wallTiles);
                walls.Add(wall);
            }
            return walls;
        }
    }
}