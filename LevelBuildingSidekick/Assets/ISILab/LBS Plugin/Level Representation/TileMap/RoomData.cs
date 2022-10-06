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
        // Info
        [SerializeField, JsonRequired]
        private string id; // laber or ID
        [SerializeField, JsonRequired]
        private List<TileData> tiles = new List<TileData>();
        [SerializeField, JsonRequired]
        private string color;

        // Metainfo
        [JsonIgnore]
        internal Vector2Int centroid; // esto podria ir en un controlador y no directamente en la data (??)
        [JsonIgnore]
        internal Vector2Int surface; // esto podria ir en un controlador y no directamente en la data (??)
        [JsonIgnore]
        private RectInt? rect; // esto podria ir en un controlador y no directamente en la data (??)

        [JsonIgnore]
        public string ID => this.id;
        [JsonIgnore]
        public Color Color { 
            get
            {
                var c = Parse.StrToColor(color);
                //Debug.Log("#"+color+": <color=#"+ color + ">"+c.ToString()+"</color>");
                return c;
            }
            set => color = Parse.ColorTosStr(value);
        }

        [JsonIgnore]
        public List<TileData> Tiles => new List<TileData>(tiles);

        [JsonIgnore]
        public int TilesCount => tiles.Count;

        /// <summary>
        /// Empty constructor.
        /// </summary>
        public RoomData() { }

        internal RoomData(List<TileData> tiles, string id)
        {
            this.id = id;
            this.tiles = tiles;
            this.color = Parse.ColorTosStr( new Color(UnityEngine.Random.Range(0f,1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f)));
        }

        public object Clone()
        {
            var tiles = new List<TileData>();
            foreach (var t in this.tiles)
            {
                var pos = t.GetPosition();
                
                tiles.Add(t.Clone() as TileData);
            }
            var clone = new RoomData(tiles, this.id);
            return clone;
        }

        public TileData GetTile(Vector2Int pos)
        {
            for (int i = 0; i < tiles.Count; i++)
            {
                var t = tiles[i];
                if(t.GetPosition().Equals(pos))
                {
                    return t;
                }
            }
            return null;
        }

        internal bool Contains(TileData data)
        {
            return tiles.Find(t => t.Equals(data)) != null;
        }

        internal bool Contains(Vector2Int pos)
        {
            return tiles.Find(t => t.GetPosition().Equals(pos)) != null;
        }

        /// <summary>
        /// Add the tile delivered by parameters, if have it does nothing.
        /// </summary>
        /// <param name="tile"></param>
        internal void AddTile(TileData tile)
        {
            if (!tiles.Contains(tile))
            {
                tiles.Add(tile);
            }
        }

        /// <summary>
        /// Remove tile delivered by parameters, if dont have it does nothing.
        /// </summary>
        /// <param name="tile"></param>
        internal void RemoveTile(TileData tile)
        {
            if (tiles.Contains(tile))
            {
                tiles.Remove(tile);
            }
        }

        internal RectInt GetRect()
        {

            if (/*!dirty &&*/ rect != null)
                return (RectInt)rect;

            Vector2Int max = new Vector2Int(int.MinValue, int.MinValue);
            Vector2Int min = new Vector2Int(int.MaxValue, int.MaxValue);
            foreach (var t in tiles)
            {
                var pos = t.GetPosition();
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

        internal float GetRatio()
        {
            if (/*!dirty &&*/ rect != null)
                return ((RectInt)rect).width / ((RectInt)rect).height;

            rect = GetRect();
            return ((RectInt)rect).width / ((RectInt)rect).height;
        }

        internal int GetWidth()
        {
            if (/*!dirty &&*/ rect != null)
                return ((RectInt)rect).width;

            rect = GetRect();
            return ((RectInt)rect).width;
        }

        internal int GetHeight()
        {
            if (/*!dirty &&*/ rect != null)
                return ((RectInt)rect).height;

            rect = GetRect();
            return ((RectInt)rect).height;
        }

        internal Vector2Int GetCentroid()
        {
            if (/*!dirty &&*/ centroid != null)
                return centroid;

            Vector2Int center = new Vector2Int(0, 0);
            foreach (var t in tiles)
                center += t.GetPosition();

            centroid = center / tiles.Count;
            return centroid;
        }

        internal List<TileData> GetConvexCorners()
        {
            var corners = new List<TileData>();
            foreach (var current in tiles)
            {
                var s = 0;
                for (int i = 0; i < Directions.sidedirs.Length; i++)
                {
                    var neighbor = current.GetPosition() + Directions.sidedirs[i];
                    if (!tiles.Contains(new TileData(neighbor, this.id))) // (!!!) podria no conicidir nunca
                    {
                        s += Mathf.RoundToInt(Mathf.Pow(2, i));
                    }
                }
                if (s != 0)
                {
                    if (s % 3 == 0 || s == 7 || s == 11 || s == 13 || s == 14)
                        corners.Add(current);
                    continue;
                }
            }
            return corners;
        }

        internal List<TileData> GetConcaveCorners()
        {
            var corners = new List<TileData>();
            foreach (var current in tiles)
            {
                var s = 0;
                var pos = current.GetPosition();
                for (int i = 0; i < Directions.sidedirs.Length; i++)
                {
                    var neighbor = pos + Directions.sidedirs[i];
                    if (!tiles.Contains(new TileData(neighbor, this.id))) // (!!!) podria no conicidir nunca
                    {
                        s += Mathf.RoundToInt(Mathf.Pow(2, i));
                    }
                }

                if (s != 0)
                    continue;

                for (int i = 0; i < Directions.diagdirs.Length; i++)
                {
                    var neighbor = pos  + Directions.diagdirs[i];
                    if (!tiles.Contains(new TileData(neighbor, this.id))) // (!!!) podria no conicidir nunca
                    {
                        var other1 = new Vector2Int(pos.x + Directions.diagdirs[i].x, pos.y);
                        if (!corners.Contains(new TileData(other1, this.id)))
                        {
                            var c = tiles.Find(t => t.GetPosition().Equals(other1));
                            corners.Add(c);  // (!!!) podria no conicidir nunca
                        }

                        var other2 = new Vector2Int(pos.x, pos.y + Directions.diagdirs[i].y);
                        if (!corners.Contains(new TileData(other2, this.id)))
                        {
                            var c = tiles.Find(t => t.GetPosition().Equals(other2));
                            corners.Add(c); // (!!!) podria no conicidir nunca
                        }
                    }
                }

            }
            return corners;
        }

        internal List<WallData> GetVerticalWalls()
        {
            var walls = new List<WallData>();

            var convexCorners = GetConvexCorners();
            var allCorners = GetConcaveCorners();
            allCorners.AddRange(convexCorners);

            foreach (var current in convexCorners)
            {
                TileData other = null;
                int lessDist = int.MaxValue;
                foreach (var candidate in allCorners)
                {
                    if (current == candidate)
                        continue;
                    var pos = current.GetPosition();
                    if (pos.x - candidate.GetPosition().x != 0)
                        continue;

                    var dist = Mathf.Abs(pos.y - candidate.GetPosition().y);
                    if (dist < lessDist)
                    {
                        lessDist = dist;
                        other = candidate;
                    }
                }

                if (other == null)
                    other = current;

                if (walls.Any(w => (w.firstCorner == other.GetPosition()) && (w.secondCorner == current.GetPosition())))
                    continue;

                var wallTiles = new List<Vector2Int>();
                var end = Mathf.Max(current.GetPosition().y, (int)other.GetPosition().y);
                var start = Mathf.Min(current.GetPosition().y, (int)other.GetPosition().y);
                for (int i = 0; i <= end- start; i++)
                {
                    wallTiles.Add(new Vector2Int(current.GetPosition().x, start + i));
                }
                var dir = (current.GetPosition().x >= GetCentroid().x) ? Vector2Int.right : Vector2Int.left;

                var wall = new WallData(current.GetPosition(),other.GetPosition(), this.id, dir, wallTiles);
                walls.Add(wall);
            }
            return walls;
        }

        internal List<WallData> GetHorizontalWalls()
        {
            var walls = new List<WallData>();

            var convexCorners = GetConvexCorners();
            var allCorners = GetConcaveCorners();
            allCorners.AddRange(convexCorners);

            foreach (var current in convexCorners)
            {
                TileData other = null;
                int lessDist = int.MaxValue;
                foreach (var candidate in allCorners)
                {
                    if (current == candidate)
                        continue;

                    var pos = current.GetPosition();
                    if (pos.y - candidate.GetPosition().y != 0)
                        continue;

                    var dist = Mathf.Abs(pos.x - candidate.GetPosition().x);
                    if (dist < lessDist)
                    {
                        lessDist = dist;
                        other = candidate;
                    }
                }

                if (other == null)
                    other = current;

                if (walls.Any(w => (w.firstCorner == other.GetPosition()) && (w.secondCorner == current.GetPosition())))
                    continue;

                var wallTiles = new List<Vector2Int>();
                var end = Mathf.Max(current.GetPosition().x, (int)other.GetPosition().x);
                var start = Mathf.Min(current.GetPosition().x, (int)other.GetPosition().x);
                for (int i = 0; i <= end - start; i++)
                {
                    wallTiles.Add(new Vector2Int(start + i, current.GetPosition().y));
                }
                var dir = (current.GetPosition().y >= GetCentroid().y) ? Vector2Int.up : Vector2Int.down;
                var wall = new WallData(current.GetPosition(), other.GetPosition(), this.id, dir, wallTiles);
                walls.Add(wall);
            }
            return walls;
        }
    }
}