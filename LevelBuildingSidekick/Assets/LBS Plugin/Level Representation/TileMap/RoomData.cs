using LevelBuildingSidekick.Schema;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBS.Representation.TileMap
{

    [Serializable]
    internal class RoomData : ICloneable
    {
        // Info
        internal string ID;
        internal List<Vector2Int> tiles = new List<Vector2Int>();

        // Metainfo
        [JsonIgnore] internal Vector2Int centroid;
        [JsonIgnore] internal Vector2Int surface;
        [JsonIgnore] private RectInt? rect;

        internal RoomData(List<Vector2Int> tiles, string ID)
        {
            this.ID = ID;
            this.tiles = tiles;
        }

        public object Clone()
        {
            var tiles = new List<Vector2Int>();
            foreach (var t in this.tiles)
            {
                tiles.Add(new Vector2Int(t.x, t.y));
            }
            var clone = new RoomData(tiles, this.ID);
            return clone;
        }

        internal void AddTile(Vector2Int tile)
        {
            if (!tiles.Contains(tile))
            {
                tiles.Add(tile);
            }
        }

        internal void RemoveTile(Vector2Int tile)
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
                if (t.x > max.x)
                    max.x = t.x;
                if (t.y > max.y)
                    max.y = t.y;

                if (t.x < min.x)
                    min.x = t.x;
                if (t.y < min.y)
                    min.y = t.y;
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
                center += t;

            centroid = center / tiles.Count;
            return centroid;
        }

        internal List<Vector2Int> GetConvexCorners()
        {
            var corners = new List<Vector2Int>();
            foreach (var current in tiles)
            {
                var s = 0;
                for (int i = 0; i < Directions.sidedirs.Length; i++)
                {
                    var neighbor = current + Directions.sidedirs[i];
                    if (!tiles.Contains(neighbor))
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

        internal List<Vector2Int> GetConcaveCorners()
        {
            var corners = new List<Vector2Int>();
            foreach (var current in tiles)
            {
                var s = 0;
                for (int i = 0; i < Directions.sidedirs.Length; i++)
                {
                    var neighbor = current + Directions.sidedirs[i];
                    if (!tiles.Contains(neighbor))
                    {
                        s += Mathf.RoundToInt(Mathf.Pow(2, i));
                    }
                }

                if (s != 0)
                    continue;

                for (int i = 0; i < Directions.diagdirs.Length; i++)
                {
                    var neighbor = current + Directions.diagdirs[i];
                    if (!tiles.Contains(neighbor))
                    {
                        var other1 = new Vector2Int(current.x + Directions.diagdirs[i].x, current.y);
                        if (!corners.Contains(other1))
                            corners.Add(other1);

                        var other2 = new Vector2Int(current.x, current.y + Directions.diagdirs[i].y);
                        if (!corners.Contains(other2))
                            corners.Add(other2);
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
                Vector2Int? other = null;
                int lessDist = int.MaxValue;
                foreach (var candidate in allCorners)
                {
                    if (current == candidate)
                        continue;

                    if (current.x - candidate.x == 0)
                    {
                        var dist = Mathf.Abs(current.y - candidate.y);
                        if (dist < lessDist)
                        {
                            lessDist = dist;
                            other = candidate;
                        }
                    }
                }

                if (other == null)
                    other = current;

                var wallTiles = new List<Vector2Int>();
                var end = Mathf.Min(current.x, (int)other?.x);
                var start = Mathf.Max(current.x, (int)other?.x);
                for (int i = start; i <= end; i++)
                {
                    wallTiles.Add(new Vector2Int(current.x + i, current.y));
                }
                var dir = (current.x >= GetCentroid().x) ? Vector2Int.right : Vector2Int.left;
                walls.Add(new WallData(current, (Vector2Int)other, this.ID, dir, wallTiles));
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
                Vector2Int? other = null;
                int lessDist = int.MaxValue;
                foreach (var candidate in allCorners)
                {
                    if (current == candidate)
                        continue;

                    if (current.y - candidate.y == 0)
                    {
                        var dist = Mathf.Abs(current.x - candidate.x);
                        if (dist < lessDist)
                        {
                            lessDist = dist;
                            other = candidate;
                        }
                    }
                }

                if (other == null)
                    other = current;

                var wallTiles = new List<Vector2Int>();
                var end = Mathf.Min(current.y, (int)other?.y);
                var start = Mathf.Max(current.y, (int)other?.y);
                for (int i = start; i <= end; i++)
                {
                    wallTiles.Add(new Vector2Int(current.x, current.y + i));
                }
                var dir = (current.y >= GetCentroid().y) ? Vector2Int.up : Vector2Int.down;
                walls.Add(new WallData(current, (Vector2Int)other, this.ID, dir, wallTiles));
            }
            return walls;
        }
    }
}