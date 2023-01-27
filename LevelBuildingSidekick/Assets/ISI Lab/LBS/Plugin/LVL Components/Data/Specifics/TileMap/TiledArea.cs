using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Newtonsoft.Json;

namespace LBS.Components.TileMap
{
    [System.Serializable]
    public class TiledArea<T> : TileMapModule<T> where T : LBSTile
    {
        #region FIELDS

        [SerializeField, JsonRequired]
        protected string id;
        [SerializeField, JsonRequired]
        protected Color color;

        #endregion

        #region PROPERTIES

        [JsonIgnore]
        public string ID => id;
        [JsonIgnore]
        public Color Color => color;

        #endregion

        #region EVENTS

        [JsonIgnore]
        public Action<T> OnAddTile;

        #endregion

        #region CONSTRUCTORS

        public TiledArea() : base(){}

        public TiledArea(List<T> tiles, string id, string key) : base(tiles, key)
        {
            this.color = new Color().RandomColor(); // (?) esto debe ir aqui?

            this.id = id;
        }

        #endregion

        #region METHODS

        public override bool AddTile(T tile)
        {
            //if (GetDistance(tile.Position) > 1)
            //    return false;
            OnAddTile?.Invoke(tile);
            return base.AddTile(tile);
        }

        public int GetDistance(Vector2 pos)
        {
            var lessDist = int.MaxValue;
            for (int i = 0; i < TileCount; i++)
            {
                var t2 = tiles[i].Position;

                var dist = Mathf.Abs(pos.x - t2.x) + Mathf.Abs(pos.y - t2.y); // manhattan

                if (dist <= lessDist)
                {
                    lessDist = (int)dist;
                }
            }

            return lessDist;
        }

        private int NeighborhoodValue(Vector2Int position, List<Vector2> directions) // (!) el nombre es malisimo mejorar, esta tambien es de la clase de las tablas del gabo
        {
            var value = 0;
            for (int i = 0; i < directions.Count; i++)
            {
                var otherPos = position + directions[i];
                if (GetTile(otherPos.ToInt()) == null)
                {
                    value += Mathf.RoundToInt(Mathf.Pow(2, i));
                }
            }

            return value;
        }

        public Vector2 Centroid => Rect.center;

        public bool IsConvexCorner(Vector2 pos, List<Vector2> directions)
        {
            var s = NeighborhoodValue(pos.ToInt(), directions);
            if (s != 0)
            {
                if (s % 3 == 0 || s == 7 || s == 11 || s == 13 || s == 14)
                    return true;
            }
            return false;
        }

        public bool IsConcaveCorner(Vector2 pos, List<Vector2> directions)
        {
            var s = NeighborhoodValue(pos.ToInt(), directions);
            if (s == 1 || s == 2 || s == 4 || s == 8)
                return true;
            return false;
        }

        public bool IsWall(Vector2 pos, List<Vector2> directions)
        {
            var s = NeighborhoodValue(pos.ToInt(), directions);
            if (s == 1 || s == 2 || s == 4 || s == 8)
                return true;
            return false;

        }

        internal List<T> GetConvexCorners() // (??)  esto solo funciona para "4 conected", deberia estar en una clase aparte?, si en la clase de las tablas del gabo
        {
            var sideDir = new List<Vector2>() { Vector2.right, Vector2.up, Vector2.left, Vector2.down };
            var corners = new List<T>();
            foreach (var t in tiles)
            {
                if (IsConvexCorner(t.Position, sideDir))
                    corners.Add(t as T);
            }
            return corners;
        }

        internal List<T> GetConcaveCorners() // (!) Tambien es de la clase de las tablas del gabo 
        {
            var diagDir = new List<Vector2>() { Vector2.right + Vector2.up, Vector2.up + Vector2.left, Vector2.left + Vector2.down, Vector2.down + Vector2.right };
            var sideDir = new List<Vector2>() { Vector2.right, Vector2.up, Vector2.left, Vector2.down };

            var corners = new List<T>();

            foreach (var t in tiles)
            {
                if (!IsConcaveCorner(t.Position, diagDir))
                    continue;

                for (int i = 0; i < sideDir.Count; i++)
                {
                    var other = GetTile((t.Position + sideDir[i]).ToInt());
                    if (other == null)
                        continue;
                    if (IsWall(other.Position, sideDir))
                    {
                        corners.Add(other);
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
                T other = null;
                int lessDist = int.MaxValue;
                foreach (var candidate in allCorners)
                {
                    if (current == candidate)
                        continue;

                    var tile = current;
                    if (tile.Position.x - candidate.Position.x != 0)
                        continue;

                    var dist = Mathf.Abs(tile.Position.y - candidate.Position.y);
                    if (dist < lessDist)
                    {
                        lessDist = dist;
                        other = candidate;
                    }
                }

                if (other == null)
                    other = current;

                if (walls.Any(w => (w.First == other.Position) && (w.Last == current.Position)))
                    continue;

                var wallTiles = new List<Vector2Int>();
                var oth = other.Position;
                var end = Mathf.Max(current.Position.y, oth.y);
                var start = Mathf.Min(current.Position.y, oth.y);
                for (int i = 0; i <= end - start; i++)
                {
                    wallTiles.Add(new Vector2Int(current.Position.x, start + i));
                }
                var dir = (current.Position.x >= Centroid.x) ? Vector2Int.right : Vector2Int.left;

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
                T other = null;
                int lessDist = int.MaxValue;
                foreach (var candidate in allCorners)
                {
                    if (current == candidate)
                        continue;

                    var tile = current;
                    if (tile.Position.y - candidate.Position.y != 0)
                        continue;

                    var dist = Mathf.Abs(tile.Position.x - candidate.Position.x);
                    if (dist < lessDist)
                    {
                        lessDist = dist;
                        other = candidate;
                    }
                }

                if (other == null)
                    other = current;

                if (walls.Any(w => (w.First == other.Position) && (w.Last == current.Position)))
                    continue;

                var wallTiles = new List<Vector2Int>();
                var oth = other.Position;
                var end = Mathf.Max(current.Position.x, oth.x);
                var start = Mathf.Min(current.Position.x, oth.x);
                for (int i = 0; i <= end - start; i++)
                {
                    wallTiles.Add(new Vector2Int(start + i, current.Position.y));
                }
                var dir = (current.Position.y >= Centroid.y) ? Vector2Int.up : Vector2Int.down;
                var wall = new WallData(this.id, dir, wallTiles);
                walls.Add(wall);
            }
            return walls;
        }

        public override object Clone()
        {
            var tileMap = new TiledArea<T>();
            tileMap.tiles = tiles.Select(t => t.Clone() as LBSTile).ToList(); //new List<LBSTile>(tiles);
            return tileMap;
        }

        #endregion
    }
}

