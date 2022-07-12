using LevelBuildingSidekick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace LevelBuildingSidekick.Blueprint
{
    public class SchemaController : LevelRepresentationController
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
                if((Data as SchemaData).tilemap == null)
                {
                    (Data as SchemaData).tilemap = new int[Size.x,Size.y];
                }
                return (Data as SchemaData).tilemap;
            }
            set
            {
                (Data as SchemaData).tilemap = value;
            }
        }
        public Vector2Int Size
        {
            get
            {
                return (Data as SchemaData).size;
            }
            set
            {
                (Data as SchemaData).size = value;
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
                (Data as SchemaData).tilemap = newTilemap;
            }
        }
        public int TileSize
        {
            get
            {
                return (Data as SchemaData).tileSize;
            }
            set
            {
                (Data as SchemaData).tileSize = value;
            }
        }

        private List<Tuple<int, float>>[,] temptativeMap;

        public SchemaController(Data data) : base(data)
        {
            View = new SchemaView(this);
        }

        public override void LoadData()
        {
            base.LoadData();

            var data = Data as SchemaData;

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

            RoomData data = new RoomData();
            data.room = room;
            data.position = position;
            var r = Activator.CreateInstance(data.ControllerType, new object[] { data });
            if(r is RoomController)
            {
                if(AddRoom(r as RoomController))
                {
                    _Rooms.Find((_r) => _r.ID == (r as RoomController).ID).Data = data;
                    //return r as RoomController;
                }
            }
            return r as RoomController;
        }

        public bool AddRoom(RoomController room)
        {
            if (!_Rooms.Contains(room))
            {
                _Rooms.Add(room);
                (Data as SchemaData).rooms.Add(room.Data as RoomData);
                //TileMap[room.Position.x, room.Position.y] = room.ID;
                foreach(Vector2Int v in room.TilePositions)
                {
                    TileMap[v.x + room.Position.x, v.y + room.Position.y] = room.ID;
                }

                return true;
            }
            return false;
        }
        public bool ContainsRoom(int id)
        {
            if(Rooms.Count == 0)
            {
                return false;
            }
            return (Rooms.Find((r) => r.ID == id) !=  null);
        }
        public bool DoesCollides(Rect rect, int ID)
        {
            foreach(RoomController r in Rooms)
            {
                if(r.ID == ID)
                {
                    continue;
                }
                if(r.CheckCollision(rect))
                {
                    return true;
                }
            }
            return false;
        }
        public bool GetCollisions(RoomController room, out HashSet<Vector2Int> collisions)
        {
            collisions = new HashSet<Vector2Int>();

            foreach(RoomController r in Rooms)
            {
                if(r.ID == room.ID)
                {
                    continue;
                }
                room.CheckCollision(r, out HashSet<Vector2Int> c);
                var positions = c.ToList();
                for(int i = 0; i < positions.Count; i++)
                {
                    collisions.Add(positions[i]);
                }
            }

            return collisions.Count > 0;
        }
        internal void ClearRooms()
        {
            Rooms.Clear();
            Rooms = new List<RoomController>();
            (Data as SchemaData).rooms.Clear();
            (Data as SchemaData).rooms = new List<RoomData>();
            TileMap = new int[Size.x, Size.y];
            temptativeMap = new List<Tuple<int, float>>[Size.x, Size.y];
        }
        public Vector2Int Translate(RoomController room, Vector2Int pull)
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
                if (DoesCollides(new Rect(aux, room.Bounds), room.ID) || aux.x < 0 || aux.y < 0 || aux.x >= Size.x || aux.y >= Size.y
                    || aux.x + room.Bounds.x >= Size.x || aux.y + room.Bounds.y >= Size.y)
                {
                    aux -= step;
                    Vector2Int v = new Vector2Int((int)aux.x, (int)aux.y);
                    room.Position = v;
                    return v;
                }
            }
            room.Position += pull;
            return pull;
        }
        public Vector2Int GetCollisionPush(RoomController room)
        {
            Vector2Int collisionPush = Vector2Int.zero;
            foreach(RoomController r in Rooms)
            {
                if(room.Equals(r))
                {
                    continue;
                }
                if(room.CheckCollision(r, out HashSet<Vector2Int> positions))
                {
                    var xList = positions.AsEnumerable().OrderBy((v) => v.x);
                    var yList = positions.AsEnumerable().OrderBy((v) => v.y);

                    var width = Mathf.Abs(xList.First().x - xList.Last().x);
                    var height = Mathf.Abs(yList.First().y - yList.Last().y);

                    if(width > height)
                    {
                        int dir = room.Centroid.x < r.Centroid.x ? -1 : 1;
                        collisionPush += Vector2Int.right * width * dir;
                    }
                    else
                    {
                        int dir = room.Centroid.y < r.Centroid.y ? -1 : 1;
                        collisionPush += Vector2Int.up * height * dir;
                    }
                }
            }
            return collisionPush;
        }
        public int[,] ToTileMap()
        {
            var tiles = (Data as SchemaData).tilemap;
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
            //Debug.LogWarning("Dir: " + direction);
            lastPos = position;
            if((position.x < 0 || position.x >= Size.x) || (position.y < 0 || position.y >= Size.y))
            {
                //Debug.LogWarning("-1,-1");
                return -Vector2Int.one;
            }

            if(TileMap[position.x, position.y] == 0)
            {
                //Debug.LogWarning("Empty: " + position);
                return position;
            }

            var emptyPos = position;
            int i = 1;
            do
            {
                lastPos = emptyPos;
                //Debug.Log(new Vector2Int((int)(direction.x * i), (int)(direction.y * i)));
                emptyPos = position + (new Vector2Int((int)(direction.x * i), (int)(direction.y * i)));
                if (emptyPos.x >= Size.x || emptyPos.y >= Size.y || emptyPos.x < 0 || emptyPos.y < 0)
                {
                    //Debug.LogWarning("LastPos: " + lastPos);
                    return -Vector2Int.one;
                }
                i++;
            }
            while (TileMap[emptyPos.x, emptyPos.y] != 0);

            //Debug.LogWarning("Found Empty AT: " + emptyPos);
            return emptyPos;
        }
        public Vector2Int CloserEmptyFrom(Vector2Int position)
        {
            if ((position.x < 0 || position.x >= Size.x))
            {
                //Debug.LogWarning("-1,-1");
                position.x = Mathf.Clamp(position.x, 0, Size.x - 1);
            }
            if((position.y < 0 || position.y >= Size.y))
            {
                position.y = Mathf.Clamp(position.y, 0, Size.y - 1);
            }
            if (TileMap[position.x, position.y] == 0)
            {
                //Debug.LogWarning(position);
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
                    //Debug.LogWarning("-1,-1");
                    return -Vector2Int.one;
                }
            }

            //Debug.LogWarning(emptyPos);
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
                //Debug.LogWarning("R.P: " + room.Position +   "Pos: " + pos + " - Size: " + Size);
                TileMap[room.Position.x + pos.x, room.Position.y + pos.y] = 0;
            }

            room.TilePositions.Clear();

            var x = room.ProportionType == ProportionType.RATIO ? room.Ratio.x : room.Width.x;
            var y = room.ProportionType == ProportionType.RATIO ? room.Ratio.y : room.Height.x;

            if(x < y)
            {
                float step = (y*1.0f / x*1.0f);
                //Debug.Log("Step: " + step);
                for (int i = 0; i < x; i++)
                {
                    if (room.Position.x + i >= Size.x)
                    {
                        if (TileMap[room.Position.x - 1, room.Position.y] == 0)
                        {
                            room.Position -= Vector2Int.right;
                        }
                        else
                        {
                            Debug.LogWarning("Not Enough Space: " + TileMap.GetLength(0) + " , " + TileMap.GetLength(1));
                            return;
                        }
                    }
                    for (int j = 0; j < step * i; j++)
                    {
                        if (room.Position.y + j >= Size.y)
                        {
                            if (TileMap[room.Position.x, room.Position.y - 1] == 0)
                            {
                                room.Position -= Vector2Int.up;
                            }
                            else
                            {
                                Debug.LogWarning("Not Enough Space: " + TileMap.GetLength(0) + " , " + TileMap.GetLength(1));
                                return;
                            }
                        }
                        room.AddTilePositions(new Vector2Int(i, j));
                    }
                    for (int k = 0; k <= i; k++)
                    {
                        for (int j = (int)(step * i); j < step * (i + 1); j++)
                        {
                            if (room.Position.y + j >= Size.y)
                            {
                                if (TileMap[room.Position.x, room.Position.y - 1] == 0)
                                {
                                    room.Position -= Vector2Int.up;
                                }
                                else
                                {
                                    Debug.LogWarning("Not Enough Space: " + TileMap.GetLength(0) + " , " + TileMap.GetLength(1));
                                    return;
                                }
                            }
                            room.AddTilePositions(new Vector2Int(k, j));
                        }
                    }
                }
            }
            else
            {
                float step = (x*1.0f / y*1.0f);
                for (int j = 0; j < y; j++)
                {
                    if (room.Position.y + j >= Size.y)
                    {
                        if (TileMap[room.Position.x, room.Position.y - 1] == 0)
                        {
                            room.Position -= Vector2Int.up;
                        }
                        else
                        {
                            Debug.LogWarning("Not Enough Space: " + TileMap.GetLength(0) + " , " + TileMap.GetLength(1));
                            return;
                        }
                    }
                    for (int i = 0; i < step * j; i++)
                    {
                        if (room.Position.x + i >= Size.x)
                        {
                            if (TileMap[room.Position.x - 1, room.Position.y] == 0)
                            {
                                room.Position -= Vector2Int.right;
                            }
                            else
                            {
                                Debug.LogWarning("Not Enough Space: " + TileMap.GetLength(0) + " , " + TileMap.GetLength(1));
                                return;
                            }
                        }
                        room.AddTilePositions(new Vector2Int(i, j));
                    }
                    for (int k = 0; k <= j; k++)
                    {
                        for (int i = (int)(step * j); i < step * (j + 1); i++)
                        {
                            if (room.Position.x + i >= Size.x)
                            {
                                if (TileMap[room.Position.x - 1, room.Position.y] == 0)
                                {
                                    room.Position -= Vector2Int.right;
                                }
                                else
                                {
                                    Debug.LogWarning("Not Enough Space: " + TileMap.GetLength(0) + " , " + TileMap.GetLength(1));
                                    return;
                                }
                            }
                            room.AddTilePositions(new Vector2Int(i, k));
                        }
                    }
                }
            }

            foreach (Vector2Int pos in room.TilePositions)
            {
                //Debug.Log("X: " + (room.Position.x + pos.x) + ", Y: " + (room.Position.y + pos.y));
                TileMap[room.Position.x + pos.x, room.Position.y + pos.y] = room.ID;
            }
        }
        private void AddRoomChance(RoomCharacteristics room, Vector2Int position)
        {

        }
        public void SolveCollisions(RoomController room, HashSet<Vector2Int> collisions)
        {
            var colX = collisions.OrderBy((v) => v.x).ToList();
            var x1 = colX.First().x - (room.Position.x - 1);
            var x2 = colX.Last().x - (room.Position.x - 1);
            Vector2Int mov = Vector2Int.zero;


            if(x1*x2 > 0)
            {
                mov.x = Mathf.Abs(x1) > Mathf.Abs(x2) ? x1 : x2;
            }
            else if(x1 < 0)
            {
                mov.x = Mathf.Abs(x1) + 1;
            }

            var colY = collisions.OrderBy((v) => v.y).ToList();

            if (!colX.First().Equals(colY.First()))
            {
                var y1 = colY.First().y - (room.Position.y - 1);
                var y2 = colY.Last().y - (room.Position.y - 1);

                if (y1 * y2 > 0)
                {
                    mov.y = Mathf.Abs(y1) > Mathf.Abs(y2) ? y1 : y2;
                }
                else if (y1 < 0)
                {
                    mov.y = Mathf.Abs(y1) + 1;
                }
            }
            

            room.Position += mov;

            GetCollisions(room, out collisions);

            foreach(Vector2Int v in collisions)
            {
                if(room.TilePositions.Contains(v))
                {
                    room.TilePositions.Remove(v);
                }
            }

        }
        public void SolveAdjacencies(RoomController room, HashSet<int> IDs)
        {
            if(IDs.Count == 0)
            {
                return;
            }

            Vector2Int center = Vector2Int.zero;
            foreach (int id in IDs)
            {
                var r = Rooms.Find((r) => r.ID == id);
                center += r.Centroid;
            }
            center /= IDs.Count;

            var distance = room.Centroid - center;



                //Debug.Log(IDs.Count);
            /*Vector2Int distance = Vector2Int.zero;
            int rooms = 0;
            foreach(int id in IDs)
            {
                var r = Rooms.Find((r) => r.ID == id);
                if(r == null)
                {
                    continue;
                }
                if(!room.IsAdjacent(r, out Vector2Int dist))
                {
                    rooms++;
                    distance += dist;
                }

            }

            if(rooms == 0)
            {
                return;
            }

            distance /= rooms;
            //Debug.Log("Move: " + distance);

            //Vector2Int offset = new Vector2Int(distance.x < 0 ? 1 : -1, distance.y < 0 ? 1 : -1);

            //Debug.Log(new Vector2Int(x, y));

            //Translate(room, room.Position - distance / 2);
            room.Position -= distance;// (distance + offset);


            //Expand*/

        }


        public void RegenerateTileMap()
        {
            TileMap = new int[Size.x, Size.y];
            foreach(RoomController r in Rooms)
            {
                foreach(Vector2Int v in r.TilePositions)
                {
                    TileMap[v.x + r.Position.x, v.y + r.Position.y] = r.ID;
                }
            }
        }
    }
}

