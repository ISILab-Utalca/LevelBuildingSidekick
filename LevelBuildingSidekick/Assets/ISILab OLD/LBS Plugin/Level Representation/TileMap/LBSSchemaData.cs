using LBS;
using LBS.Schema;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
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

        // Properties
        [JsonIgnore]
        public int RoomCount => rooms.Count;

        // Constructors
        public LBSSchemaData() : base () { }

        // Methods
        internal RoomData GetRoom(int i) => rooms[i];

        public List<RoomData> GetRooms() => new List<RoomData>(rooms);

        public override void Clear()
        {
            rooms.Clear();
            doors.Clear();
            tiles.Clear();
            base.Clear();
        }

        public override Vector2Int RecalculateTilePos()
        {
            var rect = GetRect();
            var m = rect.min;
            for (int i = 0; i < rooms.Count; i++)
            {
                this.rooms[i].Move(-m);
            }

            for (int i = 0; i < this.tiles.Count; i++)
            {
                this.tiles[i].Position = this.tiles[i].Position - m;
            }
            return -m;
        }

        public void AddDoor(DoorData door)
        {
            doors.Add(door);
            var p1 = door.GetFirstPosition();
            var p2 = door.GetSecondPosition();
            var t1 = GetTile(p1);
            var t2 = GetTile(p2);
            var i1 = TileMapUtils.CalcDir4Connected(p1, p2);
            var i2 = TileMapUtils.CalcDir4Connected(p2, p1);
            t1.SetConection(i1,"Door"); // (!!!) ojo con este seteo de tag
            t2.SetConection(i2,"Door");
        }

        public void RemoveDoor(DoorData door)
        {
            doors.Remove(door);
            var p1 = door.GetFirstPosition();
            var p2 = door.GetSecondPosition();
            var t1 = GetTile(p1);
            var t2 = GetTile(p2);
            var i1 = TileMapUtils.CalcDir4Connected(p1, p2);
            var i2 = TileMapUtils.CalcDir4Connected(p2, p1);
            t1.SetConection(i1, null);
            t2.SetConection(i2, null);
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
            var tempTiles = new List<TileData>();
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    var pos = new Vector2Int(firstPos.x + i - (width / 2), firstPos.y + j - (height / 2));
                    var tile = new TileData(pos,0,new string[4]);
                    AddTile(tile);
                    //this.tiles.Add(tile);
                    tempTiles.Add(tile);
                }
            }
            var color = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
            this.rooms.Add(new RoomData(tempTiles.Select(t => t.Position).ToList(), color, ID));
        }

        /// <summary>
        /// Check if exist a room with 0 tiles.
        /// </summary>
        /// <returns> True if found a room with 0 tiles, False otherwise.</returns>
        public bool CheckTilesRooms()
        {
            foreach (var room in rooms)
            {
                if (room.TilesCount <= 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Find all the rooms without tiles in the schema.
        /// </summary>
        /// <returns> A list of RoomData.</returns>
        public List<RoomData> GetRoomsWithoutTiles()
        {
            var rwt = new List<RoomData>();

            foreach (var room in rooms)
            {
                if (room.TilesCount <= 0) 
                    rwt.Add(room);                               
            }

            return rwt; 
        }

        public void RepositionRooms()
        {
            var rwt = GetRoomsWithoutTiles();
            RoomData closetRoom = rwt[0];
            float closetDis = Mathf.Infinity;

            for(int i = 0; i < rwt.Count; i++)
            {
                for(int j = 0; j < rooms.Count; j++)
                {                   
                    float dis = Vector2Int.Distance(rwt[i].Centroid, rooms[j].Centroid);

                    if (rwt[i].ID == rooms[j].ID || rooms[j].Centroid == Vector2Int.zero) continue;

                    if(dis < closetDis)
                    {
                        closetDis = dis;
                        closetRoom = rooms[j];
                    }
                }

                List<Vector2Int> tilesPositionsSchema = new List<Vector2Int>();
                Vector2Int randomPos = closetRoom.GetRandomTilePosFromCenter();

                foreach(var r in rooms)
                {
                    foreach(var pos in r.TilesPositions)
                    {           
                        tilesPositionsSchema.Add(pos);
                    }
                }

                do
                {
                    randomPos = closetRoom.GetRandomTilePosFromCenter();
                }
                while (tilesPositionsSchema.Contains(randomPos));

                rwt[i].Move(randomPos);
                rwt[i].AddTile(randomPos);
                Debug.Log("New Pos: " + randomPos);
                break;
                
            }
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
                this.tiles.Add(t);
                room.AddTile(t.Position);
            }
        }

        public void AddTile(TileData tile, string roomId)
        {
            AddTile(tile);
            var room = rooms.Find(r => r.ID == roomId);
            room.AddTile(tile.Position);
        }

        public override void AddTile(TileData tile)
        {
            base.AddTile(tile);
            //Debug.LogError("do not use this method, instead use AddTile(TileData tile, string roomId)");
        }

        public override void AddTiles(List<TileData> tiles)
        {
            base.AddTiles(tiles);
            //Debug.LogError("do not use this method, instead useSetTiles(List<TileData> tiles, string roomId)");
        }

        public override void RemoveTile(TileData tile)
        {
            base.RemoveTile(tile);

            foreach (var room in rooms)
            {
                room.RemoveTile(tile.Position);
            }
        }

        public override void RemoveTiles(List<TileData> tiles)
        {
            foreach (var tile in tiles)
            {
                RemoveTile(tile);
            }
        }

        public override void RemoveTiles(List<Vector2Int> tiles)
        {
            base.RemoveTiles(tiles);
            foreach (var room in rooms)
            {
                tiles.ForEach(t => room.RemoveTile(t));
            }
        }

        internal RoomData GetRoom(Vector2Int tilePos)
        {
            foreach (var room in this.rooms)
            {
                if (room.Contains(tilePos))
                    return room;
            }
            return null;
        }

        public override object Clone()
        {
            var clone = new LBSSchemaData();
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

            foreach (var t in tiles)
            {
                var tile = t.Clone() as TileData;
                clone.tiles.Add(tile);
            }

            return clone;
        }

        internal string[,] GetMatrix() // (?) inecesaria ?
        {
            RecalculateTilePos();
            var rect = GetRect();
            var matrixIDs = new string[rect.width, rect.height];
            foreach (var r in this.rooms)    // (!!) esto solo considera a los tiles que pertenecen a una habitacion
            {
                foreach (var t in r.TilesPositions)
                {
                    matrixIDs[t.x, t.y] = r.ID;
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
   
