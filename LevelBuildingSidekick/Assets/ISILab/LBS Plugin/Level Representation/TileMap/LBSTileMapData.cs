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
    public class LBSTileMapData : LBSRepesentationData, ICloneable // cambiar de nombre a mapData o baseStructureData
    {
        // Concrete info
        [SerializeField, JsonRequired, SerializeReference]
        private List<RoomData> rooms = new List<RoomData>();

        [SerializeField, JsonRequired, SerializeReference]
        private List<DoorData> doors = new List<DoorData>();

        // Meta info
        [HideInInspector, JsonIgnore] 
        private string[,] matrixIDs; // matriz con info de la habitacion a la que correspode cada tile
        [HideInInspector, JsonIgnore]
        private RectInt? rect;
        //[HideInInspector, JsonIgnore]
        //private int[,] tilevalue; // walls
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
            SetDirty();
        }

        public void SetDirty() // este nombre no es correcto pero se asemeja al comportamiento de unity (!)
        {
            matrixIDs = null;
            rect = null;
            //tilevalue = null;
            //RecalculateTilePos();
        }

        public Vector2Int GetDT()
        {
            return GetRect().min;
        }

        public void RecalculateTilePos()
        {
            rect = null;
            var m = GetRect().min;
            foreach (var room in rooms)
            {
                foreach (var tile in room.Tiles)
                {
                    tile.SetPosition(tile.GetPosition() - m);
                }
            }
        }

        public void AddDoor(DoorData door)
        {
            doors.Add(door);
        }

        public void RemoveDoor(DoorData door)
        {
            doors.Remove(door);
            /*
            DoorData r = null;
            foreach (var d in doors)
            {
                if(d.Equals(door))
                {
                    r = d;
                    break;
                }
            }
            doors.Remove(r);
            */
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
                    tiles.Add(new TileData(new Vector2Int(firstPos.x + i - (width / 2), firstPos.y + j - (height / 2)), ID));
                }
            }
            rooms.Add(new RoomData(tiles, ID));
            SetDirty();
        }

        internal TileData GetTile(Vector2Int pos)
        {
            foreach (var room in rooms)
            {
                foreach (var tile in room.Tiles)
                {
                    if(tile.GetPosition().Equals(pos))
                    {
                        return tile;
                    }
                }
            }
            return null;
        }

        public void AddTile(TileData tile,string roomId)
        {
            SetTiles(new List<TileData>() { tile }, roomId);
            SetDirty();
        }

        public void AddTiles(List<TileData> tiles, string roomId)
        {
            SetTiles(tiles,roomId);
            SetDirty();
        }

        public void RemoveTiles(List<Vector2Int> tiles)
        {
            var toRemove = new List<TileData>();
            foreach (var r in rooms)
            {
                foreach (var t in r.Tiles)
                {
                    foreach (var ot in tiles)
                    {
                        if (t.GetPosition() == ot)
                            toRemove.Add(t);
                    }
                }
            }
            RemoveTiles(toRemove);
        }

        public void RemoveTiles(List<TileData> tiles)
        {
            foreach (var t in tiles)
            {
                RemoveTile(t);
            }
            SetDirty();
        }

        public void RemoveTile(TileData tile)
        {
            foreach (var r in rooms)
            {
                if (r.Tiles.Contains(tile))
                {
                    r.RemoveTile(tile);
                }
            }
            SetDirty();
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
            SetDirty();
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

            foreach(var d in rooms)
            {
                var door = d.Clone() as DoorData;
                clone.doors.Add(door);
            }

            return clone;
        }

        internal string[,] GetMatrix()
        {
            if (matrixIDs != null)
                return matrixIDs;

            this.rect = null;
            var rect = GetRect();
            matrixIDs = new string[rect.width, rect.height];
            foreach (var r in rooms)
            {
                foreach (var t in r.Tiles)
                {
                    var gp = t.GetPosition();
                    var pos = gp - rect.min;
                    matrixIDs[pos.x, pos.y] = r.ID;
                }
            }
            return matrixIDs;
        }

        internal RectInt GetRect()
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
   
