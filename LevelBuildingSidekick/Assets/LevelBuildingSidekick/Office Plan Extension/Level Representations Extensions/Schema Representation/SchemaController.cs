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
        private int[,] _TileMap;
        public int[,] TileMap
        {
            get
            {
                if(_TileMap == null)
                {
                    _TileMap = new int[Size.x,Size.y];
                }
                return _TileMap;
            }
            set
            {
                _TileMap = value;
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
                _TileMap = Utility.MathTools.ResizeArray<int>(TileMap, value.x, value.y);
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

        //private List<Tuple<int, float>>[,] temptativeMap;

        public SchemaController(Data data) : base(data)
        {
            View = new SchemaView(this);
            Size = 20 * Vector2Int.one;
            TileSize = 20;
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
            RoomData data = new RoomData();
            data.room = room;
            data.position = position;
            var r = Activator.CreateInstance(data.ControllerType, new object[] { data });
            if(r is RoomController)
            {
                if(!AddRoom(r as RoomController))
                {
                    _Rooms.Find((_r) => _r.ID == (r as RoomController).ID).Data = data;
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
        internal void Clear()
        {
            Rooms.Clear();
            Rooms = new List<RoomController>();
            (Data as SchemaData).rooms.Clear();
            (Data as SchemaData).rooms = new List<RoomData>();
            TileMap = new int[Size.x, Size.y];
            //temptativeMap = new List<Tuple<int, float>>[Size.x, Size.y];
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
            int x1 = Rooms.Select((r) => r.Position).Min((p) => p.x);
            int y1 = Rooms.Select((r) => r.Position).Min((p) => p.y);

            foreach (RoomController room in Rooms)
            {
                room.Position -= new Vector2Int(x1, y1);
            }

            int x2 = 0;
            int y2 = 0;

            foreach(RoomController room in Rooms)
            {
                int x = room.Position.x + room.TilePositions.Max((p) => p.x);
                int y = room.Position.y + room.TilePositions.Max((p) => p.y);
                x2 = x > x2 ? x : x2;
                y2 = y > y2 ? y : y2;
            }
            _TileMap = new int[x2+1, y2+1];
            //Debug.Log("Size: " + (x2 + 1) + " - " + (y2 + 1));
            foreach(RoomController r in Rooms)
            {
                foreach(Vector2Int v in r.TilePositions)
                {
                    //Debug.Log("Pos: " + (r.Position.x + v.x) + " - " + (r.Position.y + v.y));
                    _TileMap[r.Position.x + v.x, r.Position.y + v.y] = r.ID;
                }
            }

            string s = "";
            for (int i = 0; i < _TileMap.GetLength(0); i++)
            {
                for (int j = 0; j < _TileMap.GetLength(1); j++)
                {
                    s += " " + (_TileMap[i,j] == 0 ? 0 : 5) + " ";
                }
                s += "\n";
            }
            Debug.Log(s);

            return _TileMap;
        }
        public bool IsLegal(Vector2Int position)
        {
            foreach(RoomController r in Rooms)
            {
                if(r.TilePositions.Contains(position))
                {
                    return false;
                }
            }
            return true;
        }
        public Vector2Int CloserEmpty(RoomController room, Vector2 direction)
        {
            var pos = room.OuterTile(direction);
            if(!IsLegal(pos))
            {
                return CloserEmptyFrom(pos);
            }
            return pos;
        }
        public Vector2Int CloserEmptyFrom(Vector2Int position)
        {
            var emptyPos = position;

            Vector2Int[] dirs = { new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(-1, 0), new Vector2Int(0, -1) };
            int steps = 1;
            int iterations = 0;
            int dirIndex = 0;

            var aux = emptyPos;
            while(!IsLegal(emptyPos))
            {
                aux += dirs[dirIndex]*steps;
                dirIndex++;
                dirIndex %= dirs.Length;
                iterations++;
                if(iterations % 2 == 0)
                {
                    steps++;
                }
                emptyPos = aux;
            }
            return emptyPos;
        }
        internal void SolveCollision(RoomController room)
        {
            // do, while (room.CheckCollision)
            var cols = new HashSet<Vector2Int>();
            var center = room.Center;
            foreach(RoomController r in Rooms)
            {
                if(r != room)
                {
                    room.CheckCollision(r, out HashSet<Vector2Int> collisions);
                    foreach(Vector2Int v in collisions)
                    {
                        cols.Add(center - v);
                    }
                }
            }

            if(cols.Count == 0)
            {
                return;
            }

            var xmin = cols.Min((v) => v.x);
            var xmax = cols.Max((v) => v.x);

            var ymin = cols.Min((v) => v.y);
            var ymax = cols.Max((v) => v.y);

            bool xDeadlock = false;
            bool yDeadlock = false;

            if (xmin < 0 && xmax > 0)
            {
                xDeadlock = true;
                Debug.Log("Complex collision in X");
            }
            if(ymin < 0 && ymax > 0)
            {
                yDeadlock = true;
                Debug.Log("Complex collision in Y");
            }

            int x = 0;
            if (xDeadlock)
            {
                x = Mathf.Abs(xmax) < Mathf.Abs(xmin) ? xmax : xmin;
            }
            else
            {
                x = Mathf.Abs(xmax) > Mathf.Abs(xmin) ? xmax : xmin;
            }

            int y = 0;
            if(yDeadlock)
            {
                y = Mathf.Abs(ymax) < Mathf.Abs(ymin) ? ymax : ymin;
            }
            else
            {
                y = Mathf.Abs(ymax) > Mathf.Abs(ymin) ? ymax : ymin;
            }

            //x = x > 0 ? x - 1 : x + 1;
            //y = y > 0 ? y - 1 : y + 1;

            room.Position += new Vector2Int(x, y);

            //Not a good solution
            SolveCollision(room);
        }

        internal void SolveAdjacencie(RoomController room)
        {
            var neighbors = Rooms.Where((r) => room.NeighborsIDs.Contains(r.ID));
            List<Vector2Int> distances = new List<Vector2Int>();

            foreach (RoomController r in neighbors)
            {
                if (!room.IsAdjacent(r, out Vector2Int distance))
                {
                    distances.Add(distance);
                }
            }

            if (distances.Count == 0)
            {
                return;
            }

            var xmin = distances.Min((v) => v.x);
            var xmax = distances.Max((v) => v.x);

            var ymin = distances.Min((v) => v.y);
            var ymax = distances.Max((v) => v.y);

            bool xDeadlock = false;
            bool yDeadlock = false;

            if (xmin < 0 && xmax > 0)
            {
                xDeadlock = true;
                Debug.Log("Complex adjacency in X");
            }
            if (ymin < 0 && ymax > 0)
            {
                yDeadlock = true;
                Debug.Log("Complex adjacency in Y");
            }

            int x = 0;
            int xe = 0;
            if (xDeadlock)
            {
                x = Mathf.Abs(xmax) < Mathf.Abs(xmin) ? xmax : xmin;
                xe = Mathf.Abs(xmax) > Mathf.Abs(xmin) ? xmax : xmin;
            }
            else
            {
                x = Mathf.Abs(xmax) > Mathf.Abs(xmin) ? xmax : xmin;
                xe = Mathf.Abs(xmax) < Mathf.Abs(xmin) ? xmax : xmin;
            }

            int y = 0;
            int ye = 0;
            if (yDeadlock)
            {
                y = Mathf.Abs(ymax) < Mathf.Abs(ymin) ? ymax : ymin;
                ye = Mathf.Abs(ymax) > Mathf.Abs(ymin) ? ymax : ymin;
            }
            else
            {
                y = Mathf.Abs(ymax) > Mathf.Abs(ymin) ? ymax : ymin;
                ye = Mathf.Abs(ymax) < Mathf.Abs(ymin) ? ymax : ymin;
            }

            int xi = x < 0 ? -1 : 1;
            int yi = y < 0 ? -1 : 1;
            //Move
            bool xMove = x != 0;
            bool yMove = y != 0;
            bool xyMove = xMove && yMove;

            var wallTiles = room.GetWalls().SelectMany(v => v);

            foreach (Vector2Int v in wallTiles)
            {
                if (xMove && !IsLegal(v + Vector2Int.right * xi + room.Position))
                {
                    xMove = false;
                }
                if (yMove && !IsLegal(v + Vector2Int.up * yi + room.Position))
                {
                    yMove = false;
                }
                if (xyMove && !IsLegal(v + new Vector2Int(xi, yi) + room.Position))
                {
                    xyMove = false;
                }
                if (!xMove && !yMove && !xyMove)
                {
                    break;
                }
            }

            if (xyMove)
            {
                room.Position += new Vector2Int(xi, yi);
            }
            else if (xMove)
            {
                room.Position += Vector2Int.right * xi;
            }
            else if (yMove)
            {
                room.Position += Vector2Int.up * yi;
            }
            else
            {
                return;
            }

            SolveAdjacencie(room);

            //Expand

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
        public void Optimize()
        {
            Utility.HillClimbing.Run(this, 
               () => { return Utility.HillClimbing.NonSignificantEpochs > 10; },
               GetNeighbors,
               Evaluate);
        }
        public List<SchemaData> GetNeighbors(SchemaData schema)
        {
            //SchemaData orignial = schema.Clone();
            List<SchemaData> neighbors = new List<SchemaData>();

            Vector2Int[] dirs = { Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down };

            for(int i = 0; i < schema.rooms.Count; i++)
            {
                foreach(Vector2Int v in dirs)
                {
                    var s = schema.Clone();
                    s.rooms[i].position += v;
                    neighbors.Add(s);
                }
            }



            return neighbors;
        }
        public float Evaluate(SchemaController schema)
        {
            float score = 0;
            foreach(RoomController r in schema.Rooms)
            {
                if(!r.FulfillConstraints(out float dist))
                {
                    score++;
                }
            }
            return schema.Rooms.Count - score;
        }
    }
}

