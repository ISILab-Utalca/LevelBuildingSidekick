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
        public List<TiledRoom<T>> rooms;

        public int RoomCount => rooms.Count;

        public bool AddRoom(TiledRoom<T> room)
        {
            if (room == null)
                return false;
            if (GetRoom(room.ID) != null)
                return false;
            rooms.Add(room);
            room.OnAddTile = (t) => 
            {
                RemoveTile(t);
            };
            return true;
        }

        private void RemoveTile(T t)
        {
            foreach(var r in rooms)
            {
                if(r.Contains(t.Position))
                {
                    r.RemoveTile(t);
                }
            }
        }

        public TiledRoom<T> GetRoom(string id)
        {
            return rooms.Find(r => r.Key == id);
        }

        public TiledRoom<T> GetRoom(int index)
        {
            return rooms[index];
        }

        public bool RemoveRoom(TiledRoom<T> area)
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
                var dist = room2.GetDistance(room1.GetTile(i).Position);

                if (dist <= lessDist)
                {
                    lessDist = dist;
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
    
}

