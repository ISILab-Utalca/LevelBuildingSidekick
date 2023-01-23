using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;
using LBS.Components.Teselation;

namespace LBS.Components.TileMap
{
    public class AreaTileMap<T> : TeselationModule where T : LBSTile
    {
        public List<Area<T>> rooms;

        public int RoomCount => rooms.Count;

        public Area<T> GetRoom(string id)
        {
            return rooms.Find(r => r.Key == id);
        }

        public Area<T> GetRoom(int index)
        {
            return rooms[index];
        }

        public bool RemoveArea(Area<T> area)
        {
            return rooms.Remove(area);
        }

        private int GetRoomDistance(string r1, string r2) // O2 - manhattan
        {
            var lessDist = int.MaxValue;
            var room1 = GetRoom(r1);
            var room2 = GetRoom(r2);
            for (int i = 0; i < room1.TileCount; i++)
            {
                for (int j = 0; j < room2.TileCount; j++)
                {
                    var t1 = room1.GetTile(i).Position;
                    var t2 = room2.GetTile(j).Position;

                    var dist = Mathf.Abs(t1.x - t2.x) + Mathf.Abs(t1.y - t2.y); // manhattan

                    if (dist <= lessDist)
                    {
                        lessDist = dist;
                    }
                }
            }
            return lessDist;
        }

        public override void Clear()
        {
            rooms.Clear();
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

    public class Area<T> : TileMapModule<T> where T : LBSTile
    {
        string id;
        public string ID => id;

        public Area(List<T> tiles, string id): base()
        {
            this.tiles = new List<LBSTile>();
            tiles.ForEach(t => AddTile(t));

            this.id = id;
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
            if (s != 0)
                return false;
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
                var s = NeighborhoodValue(t.Position, sideDir);
                if (s != 0)
                {
                    if (s % 3 == 0 || s == 7 || s == 11 || s == 13 || s == 14)
                        corners.Add(t as T);
                    continue;
                }
            }
            return corners;
        }

    }
}

