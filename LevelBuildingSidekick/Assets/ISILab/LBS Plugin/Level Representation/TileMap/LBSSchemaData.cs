using LBS;
using LBS.Schema;
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
    public class LBSSchemaData : LBSTileMapData, ICloneable // cambiar de nombre a mapData o baseStructureData
    {
        // Fields
        [SerializeField, JsonRequired, SerializeReference]
        private List<RoomData> rooms = new List<RoomData>();

        [SerializeField, JsonRequired, SerializeReference]
        private List<DoorData> doors = new List<DoorData>();

        // Meta info
        [HideInInspector, JsonIgnore]
        private RectInt? rect;
        [HideInInspector, JsonIgnore]
        private Vector2Int size;

        [JsonIgnore]
        public int RoomCount => rooms.Count;

        [JsonIgnore]
        public override Type ControllerType => typeof(Controller);

        internal RoomData GetRoom(int i) => rooms[i];

        public List<RoomData> GetRooms() => new List<RoomData>(rooms);

        public override void Clear()
        {
            rooms.Clear();
            doors.Clear();
            base.Clear();
        }

        public void RecalculateTilePos()
        {
            var m = GetRect().min;
            foreach (var room in rooms)
            {
                room.Move(-m);
            }
        }

        public void AddDoor(DoorData door)
        {
            doors.Add(door);
        }

        public void RemoveDoor(DoorData door)
        {
            doors.Remove(door);
        }

        internal void ClearDoors()
        {
            doors.Clear();
        }

        internal List<DoorData> GetDoors()
        {
            return new List<DoorData>(doors);
        }

        internal List<DoorData> GetDoors(string roomID) // roomID or roomLabel
        {
            var toReturn = new List<DoorData>();
            var room = rooms.Find(r => r.ID.Equals(roomID));
            if (room == null)
            {
                Debug.LogWarning("[error]: there is no room with that identifier.");
                return toReturn;
            }

            toReturn = doors.Where(d => room.Contains(d.GetFirstPosition())).ToList();
            return toReturn;
        }

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
            var tiles = new List<TileData>();
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    tiles.Add(new TileData(new Vector2Int(firstPos.x + i - (width / 2), firstPos.y + j - (height / 2))));
                }
            }
            var color = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
            rooms.Add(new RoomData(tiles.Select(t => t.Position).ToList(), color, ID));
        }

        /// <summary>
        /// Add a tile to the respective room. if the room does not exist
        /// it does nothing and if a room has already been assigned the
        /// corresponding tile it moves it to the new room.
        /// </summary>
        /// <param name="tiles"></param>
        /// <param name="roomId"></param>
        public void SetTiles(List<TileData> tiles, string roomId) 
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
            var clone = new LBSSchemaData();
            clone.size = size + new Vector2Int();
            clone.rooms = new List<RoomData>();
            foreach (var r in rooms)
            {
                var room = r.Clone() as RoomData;
                clone.rooms.Add(room);
            }

            foreach(var d in rooms)
            {
                var door = d.Clone() as DoorData;
                clone.doors.Add(door);
            }

            return clone;
        }

        internal string[,] GetMatrix() // (?) inecesaria ?
        {
            var rect = GetRect();
            var matrixIDs = new string[rect.width, rect.height];
            foreach (var r in rooms)
            {
                foreach (var t in r.TilesPositions)
                {
                    var gp = t.GetPosition();
                    var pos = gp - rect.min;
                    matrixIDs[pos.x, pos.y] = r.ID;
                }
            }
            return matrixIDs;
        }

        public override void Print()
        {
            var msg = "";
            var mtx = GetMatrix();

            for (int j = 0; j < mtx.GetLength(1); j++)
            {
                for (int i = 0; i < mtx.GetLength(0); i++)
                {
                    if (mtx[i, j] != null)
                    {
                        var c = Parse.ColorTosStr(GetRoom(mtx[i, j]).Color);
                        msg += "<color=#"+c+">#</color>";
                    }
                    else
                    {
                        msg += "#";
                    }
                    
                }
                msg += "\n";
            }
            Debug.Log(msg);
        }
    }
}
   
