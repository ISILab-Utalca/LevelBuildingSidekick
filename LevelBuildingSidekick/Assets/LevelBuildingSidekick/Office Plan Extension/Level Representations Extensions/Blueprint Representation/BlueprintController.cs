using LevelBuildingSidekick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace LevelBuildingSidekick.Blueprint
{
    public class BlueprintController : LevelRepresentationController
    {
        private List<RoomController> _Rooms;
        public List<RoomController> Rooms
        {
            get
            {
                if(_Rooms == null)
                {
                    _Rooms = new List<RoomController>();
                }
                return _Rooms;
            }
            set
            {
                _Rooms = value;
            }
        }
        public int[,] TileMap
        {
            get
            {
                if((Data as BlueprintData).tilemap == null)
                {
                    (Data as BlueprintData).tilemap = new int[Size.x,Size.y];
                }
                return (Data as BlueprintData).tilemap;
            }
            set
            {
                (Data as BlueprintData).tilemap = value;
            }
        }
        public Vector2Int Size
        {
            get
            {
                return (Data as BlueprintData).size;
            }
            set
            {
                (Data as BlueprintData).size = value;
                var newTilemap = new int[value.x, value.y];
                var minx = Mathf.Min(value.x, TileMap.GetLength(0));
                var miny = Mathf.Min(value.y, TileMap.GetLength(1));
                for (int i = 0; i < minx; i++)
                {
                    for (int j = 0; j < miny; j++)
                    { 
                        newTilemap[i, j] = TileMap[i, j];
                    }
                }
                (Data as BlueprintData).tilemap = newTilemap;
            }
        }
        public int Step
        {
            get
            {
                return (Data as BlueprintData).step;
            }
            set
            {
                (Data as BlueprintData).step = value;
            }
        }
        public int Stride
        {
            get
            {
                return (Data as BlueprintData).stride;
            }
            set
            {
                (Data as BlueprintData).stride = value;
            }
        }

        public BlueprintController(Data data) : base(data)
        {
            View = new BlueprintView(this);
        }

        public override void LoadData()
        {
            base.LoadData();

            var data = Data as BlueprintData;

            if(data.rooms == null)
            {
                data.rooms = new List<RoomData>();
            }
            foreach (RoomData d in data.rooms)
            {
                var room = Activator.CreateInstance(d.ControllerType, new object[] { d });
                if(room is RoomController)
                {
                    Rooms.Add(room as RoomController);
                }
            }
        }

        public override void Update()
        {
            base.Update();
        }
        public RoomController AddRoom(RoomCharacteristics room, Vector2Int position)
        {

            if (position.x < 0 || position.y < 0)
            {
                Debug.LogWarning("No space left Pos: " + position + " - Size: " + Size);
                return null;
            }
            if (TileMap[position.x, position.y] != 0)
            {
                position = CloserEmptyFrom(position);
            }

            RoomData data = ScriptableObject.CreateInstance<RoomData>();
            data.room = room;
            data.position = position;
            var r = Activator.CreateInstance(data.ControllerType, new object[] { data });
            if(r is RoomController)
            {
                if(_Rooms.Contains(r as RoomController))
                {
                    (r as RoomController).Data = data;
                    return r as RoomController;
                }
                _Rooms.Add(r as RoomController);
                (Data as BlueprintData).rooms.Add(data);
            }
            return r as RoomController;
        }
        public bool ContainsRoom(int id)
        {
            if(Rooms.Count == 0)
            {
                return false;
            }
            return (Rooms.Find((r) => r.ID == id) !=  null);
        }
        /*public bool DoesCollides(Rect rect, int ID)
        {
            foreach(RoomController r in Rooms)
            {
                if(r.ID == ID)
                {
                    continue;
                }
                if(r.CheckCollision(rect,true))
                {
                    return true;
                }
            }
            return false;
        }*/

        internal void ClearRooms()
        {
            Rooms.Clear();
            Rooms = new List<RoomController>();
            (Data as BlueprintData).rooms.Clear();
            (Data as BlueprintData).rooms = new List<RoomData>();
            TileMap = new int[Size.x, Size.y];
        }

        /*public Vector2Int Translate(RoomController room, Vector2Int pull)
        {
            if(pull.magnitude == 0)
            {
                return Vector2Int.zero;
            }

            int min = 0;
            if(pull.x == 0)
            {
                min = pull.y;
            }
            else if (pull.y == 0)
            {
                min = pull.x;
            }
            else
            {
                min = pull.x < pull.y ? pull.x : pull.y;
            }

            Vector2 step = pull / min;
            Vector2 pos = room.Position;
            for (int i = 0; i < min; i++)
            {
                Vector2 aux = (pos + step * (i + 1));
                if (DoesCollides(new Rect(aux, room.InnerBounds), room.ID))
                {
                    aux -= step;
                    Vector2Int v = new Vector2Int((int)aux.x, (int)aux.y);
                    room.Position = v;
                    return v;
                }
            }
            room.Position += pull;
            return pull;
        }*/
        /*public Vector2Int GetCollisionPush(RoomController room)
        {
            Vector2Int collisionPush = Vector2Int.zero;
            foreach(RoomController r in Rooms)
            {
                if(room.Equals(r))
                {
                    continue;
                }
                if(room.CheckCollision(r, out Vector2Int distance))
                {
                    collisionPush += distance;
                }
            }
            return collisionPush;
        }*/
        public int[,] ToTileMap()
        {
            var tiles = (Data as BlueprintData).tilemap;
            tiles = new int[Size.x, Size.y];
            foreach(RoomController r in Rooms)
            {
                foreach(Vector2Int v in r.TilePositions)
                {
                    tiles[r.Position.x + v.x, r.Position.y + v.y] = r.ID;
                }
            }
            return tiles;
        }

        public Vector2Int CloserEmpty(Vector2 direction, Vector2Int position, out Vector2Int lastPos)
        {
            lastPos = position;
            if((position.x < 0 || position.x >= Size.x) && (position.y < 0 || position.y >= Size.y))
            {
                Debug.LogWarning("-1,-1");
                return -Vector2Int.one;
            }

            if(TileMap[position.x, position.y] == 0)
            {
                Debug.LogWarning(position);
                return position;
            }

            var emptyPos = position;
            int i = 1;
            do
            {
                lastPos = emptyPos;
                emptyPos = position + (new Vector2Int((int)(direction.x * i), (int)(direction.y * i)));
                if (emptyPos.x >= Size.x || emptyPos.y >= Size.y || emptyPos.x < 0 || emptyPos.y < 0)
                {
                    Debug.LogWarning("-1,-1");
                    return -Vector2Int.one;
                }
                i++;
            }
            while (TileMap[emptyPos.x, emptyPos.y] != 0);

            Debug.LogWarning(emptyPos);
            return emptyPos;
        }

        public Vector2Int CloserEmptyFrom(Vector2Int position)
        {
            if ((position.x < 0 || position.x >= Size.x) && (position.y < 0 || position.y >= Size.y))
            {
                Debug.LogWarning("-1,-1");
                return -Vector2Int.one;
            }
            if (TileMap[position.x, position.y] == 0)
            {
                Debug.LogWarning(position);
                return position;
            }

            var emptyPos = position;

            Vector2Int[] dirs = { new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(-1, 0), new Vector2Int(0, -1) };
            int steps = 1;
            int iterations = 0;
            int dirIndex = 0;

            var aux = emptyPos;
            while(TileMap[emptyPos.x, emptyPos.y] != 0)
            {
                aux += dirs[dirIndex]*steps;
                dirIndex++;
                dirIndex %= dirs.Length;
                iterations++;
                if(iterations % 2 == 0)
                {
                    steps++;
                }
                if(!(aux.x < 0 || aux.x >= Size.x) && !(aux.y < 0 || aux.y >= Size.y))
                {
                    emptyPos = aux;
                }
                if(iterations > TileMap.Length*4)
                {
                    Debug.LogWarning("-1,-1");
                    return -Vector2Int.one;
                }
            }

            Debug.LogWarning(emptyPos);
            return emptyPos;
        }

        public void ResizeRoomToMin(RoomController room)
        {
            if(!Rooms.Contains(room))
            {
                return;
            }

            foreach(Vector2Int pos in room.TilePositions)
            {
                Debug.LogWarning("R.P: " + room.Position +   "Pos: " + pos + " - Size: " + Size);
                TileMap[room.Position.x + pos.x, room.Position.y + pos.y] = 0;
            }
            room.ResizeToMin();
            foreach (Vector2Int pos in room.TilePositions)
            {
                TileMap[room.Position.x + pos.x, room.Position.y + pos.y] = room.ID;
            }
        }
    }
}

