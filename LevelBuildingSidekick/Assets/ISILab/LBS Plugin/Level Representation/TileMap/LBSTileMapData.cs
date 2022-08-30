using LevelBuildingSidekick;
using LevelBuildingSidekick.Schema;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField,JsonRequired, SerializeReference]
        private List<RoomData> rooms = new List<RoomData>();

        // Meta info
        [HideInInspector, JsonIgnore] 
        private string[,] matrixIDs; // matriz con info de la habitacion a la que correspode cada tile
        [HideInInspector, JsonIgnore]
        private RectInt? rect;
        [HideInInspector, JsonIgnore]
        private int[,] tilevalue; // walls
        [HideInInspector, JsonIgnore]
        private Vector2Int size;

        //private bool dirty = false; // flag

        // Properties
        //public Vector2Int Size => 
        //public roomAmount =>
        public int RoomCount => rooms.Count;
        internal RoomData GetRoom(int i) => rooms[i];


        [JsonIgnore] 
        public override Type ControllerType => typeof(Controller);

        /// <summary>
        /// Returns the room with the ID provided by parameters.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal RoomData GetRoom(string id) // "ID" = "Label"
        {
            return rooms.Find(r => r.ID == id);
        }

        /// <summary>
        /// Add a room.
        /// </summary>
        /// <param name="firstPos"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="ID"></param>
        public void AddRoom(Vector2Int firstPos, int width, int height, string ID)
        {
            var tiles = new List<Vector2Int>();
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    tiles.Add(new Vector2Int(firstPos.x + i - (width/2), firstPos.y + j - (height/2)));
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
                if (r.Tiles.Contains(tile))
                {
                    r.RemoveTile(tile);
                }
            }
        }

        /// <summary>
        /// Add a tile to the respective room. if the room does not exist
        /// it does nothing and if a room has already been assigned the
        /// corresponding tile it moves it to the new room.
        /// </summary>
        /// <param name="tiles"></param>
        /// <param name="roomId"></param>
        public void SetTiles(List<Vector2Int> tiles, string roomId) 
        {
            var room = rooms.Find(r => r.ID == roomId);
            if (room == null) // esto podria crear la habitacion y asignarla en vez de no hacer nada (??)
            {
                // change this to name or something like. (!)
                Debug.LogWarning("[Error]: There is no room with the label <b>'" + roomId + "'</b> in the level representation."); 
                return;
            }
            RemoveTiles(tiles);
            
            foreach (var t in tiles)
            {
                room.AddTile(t);
            }
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
                foreach (var t in r.Tiles)
                {
                    var pos = t - rect.min;
                    matrixIDs[pos.x, pos.y] = r.ID;
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



        public override void Print()
        {
            var msg = "";
            msg += "<b>Tile map. (step 1)</b>" + "\n";
            msg += "Room amount: " + this.rooms.Count + "\n";
            msg += "------------";
            rooms.ForEach(r => msg += r.ID + ": " + r.TilesCount + "\n");
            Debug.Log(msg);
        }
    }
}
   
