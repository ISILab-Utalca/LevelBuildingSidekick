using LevelBuildingSidekick;
using LevelBuildingSidekick.Schema;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBS.Representation.TileMap
{
    public static class Directions
    {
        public readonly static Vector2Int[] sidedirs = { Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down };
        public readonly static Vector2Int[] diagdirs = { Vector2Int.right + Vector2Int.up, Vector2Int.up + Vector2Int.left, Vector2Int.left + Vector2Int.down, Vector2Int.down + Vector2Int.right };
    }

    [System.Serializable]
    public class LBSTileMapData : LBSRepesentationData, ICloneable // cambiar de nombre a mapData o baseStructureData
    {
        // Concrete info
        private List<RoomData> rooms = new List<RoomData>();

        // Meta info
        [JsonIgnore] private string[,] matrixIDs;
        [JsonIgnore] private RectInt? rect;
        [JsonIgnore] private int[,] tilevalue; // walls
        [JsonIgnore] private Vector2Int size;
        //private bool dirty = false; // flag


        // Properties
        //public Vector2Int Size => 
        //public roomAmount =>

        [JsonIgnore] public override Type ControllerType => typeof(Controller);

        internal RoomData GetRoomByID(string id)
        {
            return rooms.Find(r => r.ID.Equals(id));
        }

        public void AddRoom(Vector2Int firstPos, int width, int height, string ID)
        {
            var tiles = new List<Vector2Int>();
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    tiles.Add(new Vector2Int(firstPos.x + i, firstPos.y + j));
                }
            }
            rooms.Add(new RoomData(tiles, ID));
        }

        public void AddTiles(List<Vector2Int> tiles, string roomId)
        {
            SetTiles(tiles,roomId);
        }

        public void RemoveTiles(List<Vector2Int> tiles)
        {
            foreach (var t in tiles)
            {
                RemoveTile(t);
            }
        }

        public void RemoveTile(Vector2Int tile)
        {
            foreach (var r in rooms)
            {
                if (r.tiles.Contains(tile))
                {
                    r.tiles.Remove(tile);
                }
            }
        }

        public void SetTiles(List<Vector2Int> tiles, string roomId)
        {
            if(!rooms.Exists(r => r.ID == (roomId)))
            {
                Debug.LogWarning("[Error]: There is no room with the id <b>'" + roomId + "'</b> in the level representation."); // change this to name or something like. (!)
                return;
            }
            
            RemoveTiles(tiles);

            var room = rooms.Find(r => r.ID.Equals(roomId));
            foreach (var t in tiles)
            {
                room.AddTile(t);
            }
        }

        internal int GetRoomAmount()
        {
            return rooms.Count;
        }

        public object Clone()
        {
            var clone = new LBSTileMapData();
            clone.size = size + new Vector2Int();
            clone.rooms = new List<RoomData>();
            foreach (var r in rooms)
            {
                var room = r.Clone() as RoomData;
                clone.rooms.Add(room);
            }
            return clone;
        }

        internal string[,] GetMatrix()
        {
            if (/**/matrixIDs != null)
                return matrixIDs;

            var rect = GetRect();
            matrixIDs = new string[rect.width, rect.height];
            foreach (var r in rooms)
            {
                foreach (var t in r.tiles)
                {
                    matrixIDs[t.x, t.y] = r.ID;
                }
            }
            return matrixIDs;
        }

        private RectInt GetRect()
        {
            if (/*!dirty &&*/ rect != null)
                return (RectInt)rect;

            Vector2Int max = new Vector2Int(int.MinValue, int.MinValue);
            Vector2Int min = new Vector2Int(int.MaxValue, int.MaxValue);
            foreach (var r in rooms)
            {
                var currentMax = r.GetRect().max;
                if (currentMax.x > max.x)
                    max.x = currentMax.x;
                if (currentMax.y > max.y)
                    max.y = currentMax.y;

                var currentMin = r.GetRect().min;
                if (currentMin.x < min.x)
                    min.x = currentMin.x;
                if (currentMin.y < min.y)
                    min.y = currentMin.y;
            }
            rect = new RectInt(min, max - min + new Vector2Int(1, 1));
            return (RectInt)rect;
        }

        internal RoomData GetRoom(int i)
        {
            return rooms[i];
        }

        public override void Print()
        {
            var msg = "";
            msg += "<b>Tile map. (step 1)</b>" + "\n";
            msg += "Room amount: " + this.rooms.Count + "\n";
            msg += "------------";
            rooms.ForEach(r => msg += r.ID + ": " + r.tiles.Count + "\n");
            Debug.Log(msg);
        }
    }
}
   
