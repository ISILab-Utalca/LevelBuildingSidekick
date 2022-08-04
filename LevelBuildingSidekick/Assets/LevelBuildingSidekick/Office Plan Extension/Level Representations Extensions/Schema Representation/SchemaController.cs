using LevelBuildingSidekick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace LevelBuildingSidekick.Schema
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
        public bool GetCollisions(RoomController room, out HashSet<Tile> collisions)
        {
            collisions = new HashSet<Tile>();

            foreach(RoomController r in Rooms)
            {
                if(r.ID == room.ID)
                {
                    continue;
                }
                room.CheckCollision(r, out HashSet<Tile> c);
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
                if(room.CheckCollision(r, out HashSet<Tile> positions))
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
        public int[,] ToMatrix()
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
                int x = room.Position.x + room.Tiles.Max((p) => p.x);
                int y = room.Position.y + room.Tiles.Max((p) => p.y);
                x2 = x > x2 ? x : x2;
                y2 = y > y2 ? y : y2;
            }
            //Debug.Log("W: " + x2 + " - H: " + y2);
            _TileMap = new int[x2+1, y2+1];
            //Debug.Log("Size: " + (x2 + 1) + " - " + (y2 + 1));
            foreach(RoomController r in Rooms)
            {
                foreach(Tile t in r.Tiles)
                {
                    //Debug.Log("Pos: " + (r.Position.x + v.x) + " - " + (r.Position.y + v.y));
                    _TileMap[r.Position.x + t.x, r.Position.y + t.y] = r.ID;
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
            //Debug.Log(s);

            return _TileMap;
        }
        public void FromMatrix(int[,] matrix)
        {
            foreach(RoomController r in Rooms)
            {
                r.Tiles.Clear();
            }
            for(int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    if (matrix[i,j] != 0)
                    {
                        var r = Rooms.Find(r => r.ID == matrix[i, j]);
                        if(r != null)
                        {
                            try
                            {
                                r.AddTilePosition(new Vector2Int(i, j));
                            }
                            catch
                            {
                                Debug.LogError("Error con los Tiles?");
                            }
                        }
                        else
                        {
                            Debug.LogWarning("Room Does Not Exist");
                        }
                    }
                }
            }
        }
        public bool IsLegal(Vector2 position)
        {
            foreach(RoomController r in Rooms)
            {
                var t = new Tile((int)position.x - r.Position.x, (int)position.y - r.Position.y);
                if (r.Tiles.Contains(t))
                {
                    return false;
                }
            }
            //Debug.Log(position + " Don't exist");
            return true;
        }
        public bool IsLegal(Vector2Int position, out RoomController room)
        {
            var t = new Tile(position);
            foreach (RoomController r in Rooms)
            {
                if (r.Tiles.Contains(t))
                {
                    room = r;
                    return false;
                }
            }
            //Debug.Log(t + " Don't exist");
            room = null;
            return true;
        }
        public Vector2Int CloserEmpty(RoomController room, Vector2 direction)
        {
            var pos = room.OuterTile(direction);
            if(!IsLegal(room.Position + pos))
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
            //Debug.Log("Check Collisions of " + room.Label);
            int tries = 20;
            for (int i = 0; i < tries; i++)
            {
                // do, while (room.CheckCollision)
                var cols = new HashSet<Vector2>();
                var center = room.Center;
                foreach (RoomController r in Rooms)
                {
                    if (r != room)
                    {
                        room.CheckCollision(r, out HashSet<Tile> collisions);
                        if(collisions.Count != 0)
                        {
                            //Debug.Log("Try: " + i + " collides with " + r.Label);
                            foreach (Tile v in collisions)
                            {
                                cols.Add(center - v);
                            }
                        }
                    }
                }

                if (cols.Count == 0)
                {
                    //Debug.Log("No collisions left");
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
                    //Debug.Log("Complex collision in X");
                }
                if (ymin < 0 && ymax > 0)
                {
                    yDeadlock = true;
                    //Debug.Log("Complex collision in Y");
                }

                int x = 0;
                if (xDeadlock)
                {
                    x = (int)(Mathf.Abs(xmax) < Mathf.Abs(xmin) ? xmax : xmin);
                }
                else
                {
                    x = (int)(Mathf.Abs(xmax) > Mathf.Abs(xmin) ? xmax : xmin);
                }

                int y = 0;
                if (yDeadlock)
                {
                    y = (int)(Mathf.Abs(ymax) < Mathf.Abs(ymin) ? ymax : ymin);
                }
                else
                {
                    y = (int)(Mathf.Abs(ymax) > Mathf.Abs(ymin) ? ymax : ymin);
                }

                //x = x > 0 ? x - 1 : x + 1;
                //y = y > 0 ? y - 1 : y + 1;
                room.Position += new Vector2Int(x, y);

            }
        }
        internal void SolveAdjacencie(RoomController room)
        {
            int tries = 20;
            for (int i = 0; i < tries; i++)
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
                    //Debug.Log("Complex adjacency in X");
                }
                if (ymin < 0 && ymax > 0)
                {
                    yDeadlock = true;
                    //Debug.Log("Complex adjacency in Y");
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

                foreach (Tile v in wallTiles)
                {
                    if (xMove && !IsLegal(Vector2Int.right * xi + room.Position + v))
                    {
                        xMove = false;
                    }
                    if (yMove && !IsLegal(Vector2Int.up * yi + room.Position + v))
                    {
                        yMove = false;
                    }
                    if (xyMove && !IsLegal(new Vector2Int(xi, yi) + room.Position + v))
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
                    break;
                }
            }

            //Expand
        }
        public HashSet<Vector2Int> GetMatrixCorners(int[,] matrix)
        {
            Vector2Int[] sidedirs = { Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down };
            Vector2Int[] diagdirs = { Vector2Int.right + Vector2Int.up, Vector2Int.up + Vector2Int.left, Vector2Int.left + Vector2Int.down, Vector2Int.down + Vector2Int.right };

            HashSet<Vector2Int> corners = new HashSet<Vector2Int>();
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                for (int i = 0; i < matrix.GetLength(0); i++)
                {
                    if (matrix[i, j] == 0)
                    {
                        continue;
                    }
                    int s = 0;
                    for (int k = 0; k < sidedirs.Length; k++)
                    {
                        if (!(i + sidedirs[k].x >= 0 && i + sidedirs[k].x < matrix.GetLength(0) && j + sidedirs[k].y >= 0 && j + sidedirs[k].y < matrix.GetLength(1))
                            || matrix[i, j] != matrix[i + sidedirs[k].x, j + sidedirs[k].y])
                        {
                                s += Mathf.RoundToInt(Mathf.Pow(2, k));
                        }
                    }
                    if (s != 0)
                    {
                        if (s % 3 == 0 || s == 7 || s == 11 || s == 13 || s == 14)
                        {
                            corners.Add(new Vector2Int(i, j));
                        }
                        continue;
                    }
                    for (int k = 0; k < diagdirs.Length; k++)
                    {
                        if (!(i + diagdirs[k].x >= 0 && i + diagdirs[k].x < matrix.GetLength(0) && j + diagdirs[k].y >= 0 && j + diagdirs[k].y < matrix.GetLength(1))
                            || (matrix[i, j] != matrix[i + diagdirs[k].x, j + diagdirs[k].y]))
                        {
                                corners.Add(new Vector2Int(i + diagdirs[k].x, j));
                                corners.Add(new Vector2Int(i, j + diagdirs[k].y));
                        }
                    }
                }
            }
            return corners;
        }
        public List<List<Vector2Int>> GetMatrixWalls(int[,] matrix)
        {
            Vector2Int[] sidedirs = { Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down };
            List<List<Vector2Int>> walls = new List<List<Vector2Int>>();
            var corners = GetMatrixCorners(matrix);
            HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
            //Debug.Log("Corners: " + corners.Count);
            foreach(Vector2Int v in corners)
            {
                int tries = 0;
                foreach (Vector2Int dir in sidedirs)
                {
                    List<Vector2Int> wall = new List<Vector2Int>();
                    Vector2Int step = v;
                    while (step.x + dir.x >= 0 && step.x + dir.x < matrix.GetLength(0) && step.y + dir.y >= 0 && step.y + dir.y < matrix.GetLength(1) &&
                        matrix[v.x,v.y] == matrix[step.x + dir.x, step.y + dir.y])
                    {
                        tries++;
                        step += dir;
                        wall.Add(step);
                        if(visited.Contains(step))
                        {
                            break;
                        }
                        if(corners.Contains(step))
                        {
                            walls.Add(wall);
                            break;
                        }
                    }
                }
                if (tries == 0)
                {
                    List<Vector2Int> wall = new List<Vector2Int>();
                    wall.Add(v);
                    walls.Add(wall);
                }
                visited.Add(v);
                //Debug.Log("Walls: " + walls.Count);
            }
            return walls;
        }
        public List<int[,]> GetMatrixNeighbors(int[,] matrix)
        {
            var walls = GetMatrixWalls(matrix);
            List<int[,]> neighbors = new List<int[,]>();
            Vector2Int[] vDirs = { Vector2Int.up, Vector2Int.down };
            Vector2Int[] hDirs = { Vector2Int.right, Vector2Int.left };
            foreach (var wall in walls)
            {
                if(wall[0].y == wall[^1].y)
                {
                    for(int i = 0; i < vDirs.Length; i++)
                    {
                        if(wall[0].y + vDirs[i].y >= 0 && wall[0].y + vDirs[i].y < matrix.GetLength(1))
                        {
                            var child = new int[matrix.GetLength(0),matrix.GetLength(1)];
                            var child2 = new int[matrix.GetLength(0), matrix.GetLength(1)];
                            Array.Copy(matrix, child, matrix.Length);
                            Array.Copy(matrix, child2, matrix.Length);
                            foreach (var v in wall)
                            {
                                child[v.x + vDirs[i].x, v.y + vDirs[i].y] = child[v.x, v.y];
                                child2[v.x, v.y] = child2[v.x + vDirs[i].x, v.y + vDirs[i].y];
                            }
                            neighbors.Add(child);
                            neighbors.Add(child2);
                        }
                        else
                        {
                            var child = new int[matrix.GetLength(0), matrix.GetLength(1)];
                            Array.Copy(matrix, child, matrix.Length);
                            foreach (var v in wall)
                            {
                                child[v.x, v.y] = 0;
                            }
                            neighbors.Add(child);
                        }
                    }
                }
                if(wall[0].x == wall[^1].x)
                {
                    for (int i = 0; i < hDirs.Length; i++)
                    {
                        if (wall[0].x + hDirs[i].x >= 0 && wall[0].x + hDirs[i].x < matrix.GetLength(0))
                        {
                            var child = new int[matrix.GetLength(0), matrix.GetLength(1)];
                            var child2 = new int[matrix.GetLength(0), matrix.GetLength(1)];
                            Array.Copy(matrix, child, matrix.Length);
                            Array.Copy(matrix, child2, matrix.Length);
                            foreach (var v in wall)
                            {
                                    child[v.x + hDirs[i].x, v.y + hDirs[i].y] = child[v.x, v.y];
                                    child2[v.x, v.y] = child2[v.x + hDirs[i].x, v.y + hDirs[i].y];
                            }
                            neighbors.Add(child);
                            neighbors.Add(child2);
                        }
                        else
                        {
                            var child = new int[matrix.GetLength(0), matrix.GetLength(1)];
                            Array.Copy(matrix, child, matrix.Length);
                            foreach (var v in wall)
                            {
                                child[v.x, v.y] = 0;
                            }
                            neighbors.Add(child);
                        }
                    }
                }
            }
            //Debug.Log("Neighbors: " + neighbors.Count);
            return neighbors;
        }
        public List<List<Tile>> GetSemiWalls(RoomController room)
        {
          
            Vector2Int[] dirs = { Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down };  HashSet<Tile> pseudoCorners = room.GetCorners().ToHashSet();
            //find all tiles where the room touch other room corner and add them as pseudoCorners
            foreach (RoomController r in Rooms)
            {
                if(room == r)
                {
                    continue;
                }
                var corners = r.GetCorners().Where(t => t.sideCode != 0);
                foreach(Tile t in corners)
                {
                    foreach(Vector2Int dir in dirs)
                    {
                        if (room.Tiles.TryGetValue(t + r.Position + dir - room.Position, out Tile tile))
                        {
                            pseudoCorners.Add(tile);
                        }
                    }
                }
            }

            var semiWalls = room.GetWalls();
            //Search and Add all walls created by pseudoCorners
            foreach(Tile t in pseudoCorners)
            {
                foreach(Vector2Int dir in dirs)
                {
                    List<Tile> wall = new List<Tile>();
                    while(true)
                    {
                        var moved = t + dir;
                        if(!room.Tiles.Contains(moved))
                        {
                            break;
                        }
                        if(pseudoCorners.Contains(moved))
                        {
                            semiWalls.Add(wall);
                            break;
                        }
                        wall.Add(moved);
                    }
                }
            }
            return semiWalls;
        }
        public void Optimize()
        {
            var optimized = Utility.HillClimbing.Run(ToMatrix(), 
                            () => { return Utility.HillClimbing.NonSignificantEpochs >= 100; },
                            GetMatrixNeighbors,
                            EvaluateMatrix);
            FromMatrix(optimized);
        }
        public List<SchemaData> GetNeighborsByWalls(SchemaData data)
        {
            List<SchemaData> neighbors = new List<SchemaData>();

            Vector2Int[] dirs = { Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down };

            for(int i = 0; i < data.rooms.Count; i++)
            {
                foreach(Vector2Int v in dirs)
                {
                    var s = data.Clone();
                    s.rooms[i].position += v;
                    neighbors.Add(s);
                }
            }

            for(int i = 0; i < data.rooms.Count; i++)
            {
                var s = data.Clone();
                var controller = new SchemaController(s);
                var r = controller.Rooms.Find(r => r.Label == data.rooms[i].room.label);
                var walls = controller.GetSemiWalls(r);
                foreach(List<Tile> wall in walls)
                {
                    if(wall.Count == 1)
                    {
                        foreach(Vector2Int dir in dirs)
                        {
                            foreach(Tile t in wall)
                            {
                                if (r.Tiles.Contains(t + dir))
                                {
                                    continue;
                                }
                                r.Tiles.Add(t + dir);
                                if(!controller.IsLegal((t + dir).vector, out RoomController room))
                                {
                                    room.Tiles.Remove(t + dir + r.Position - room.Position);
                                }
                            }
                        }
                    }
                    else if(wall[0].x == wall[1].x)
                    {
                        foreach (Tile t in wall)
                        {
                            
                        }
                    }
                    else
                    {

                    }
                }
                
            }

            return neighbors;
        }
        /*public List<SchemaData> GetNeighborsByTile(SchemaData data)
        {
            List<SchemaData> neighbors = new List<SchemaData>();
            Vector2Int[] dirs = { Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down };
            for (int i = 0; i < data.rooms.Count; i++)
            {
                foreach (Tile t in data.rooms[i].tiles)
                {
                    foreach(Vector2Int dir in dirs)
                    {
                        var tile = t + data.rooms[i].position + dir;
                        foreach (RoomData r2 in data.rooms)
                        {
                            if (data.rooms[i] == r2)
                            {
                                continue;
                            }
                            if (r2.tiles.Contains(t - r2.position))
                            {

                                break;
                            }
                        }
                    }
                    
                }
            }
        }*/
        public float Evaluate(SchemaData data)
        {
            SchemaController controller = new SchemaController(data);
            float score = 0;
            foreach(RoomController r in controller.Rooms)
            {
                if(!r.FulfillConstraints(out float dist))
                {
                    score++;
                }
            }
            return controller.Rooms.Count - score;
        }

        public float EvaluateMatrix(int[,] matrix)
        {
            float alfa = 0.84f;
            float beta = 0.16f;
            return EvaluateMatrixAdjacencies(matrix) * alfa + EvaluateMatrixAreas(matrix) * beta;
        }

        public float EvaluateMatrixAdjacencies(int[,] matrix)
        {
            float maxScore = 0;
            float score = 0;
            Vector2Int[] dirs = { Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down };
            Dictionary<int, HashSet<int>> adjacencies = new Dictionary<int, HashSet<int>>();
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                for (int i = 0; i < matrix.GetLength(0); i++)
                {
                    if (matrix[i, j] == 0)
                    {
                        continue;
                    }
                    var r = Rooms.Find(r => r.ID == matrix[i, j]);
                    if (r == null)
                    {
                        Debug.LogWarning("Room Not Found");
                        continue;
                    }
                    if (!adjacencies.ContainsKey(matrix[i, j]))
                    {
                        adjacencies.Add(matrix[i, j], new HashSet<int>());
                    }
                    foreach (var dir in dirs)
                    {
                        if(i + dir.x >= 0 && i + dir.x < matrix.GetLength(0) && j + dir.y >= 0 && j + dir.y < matrix.GetLength(1))
                        {
                            if (r.NeighborsIDs.Contains(matrix[i + dir.x, j + dir.y]))
                            {
                                adjacencies[matrix[i, j]].Add(matrix[i + dir.x, j + dir.y]);
                            }
                        }
                        
                    }
                }
                foreach (var r in Rooms)
                {
                    maxScore += r.NeighborsIDs.Count();
                }
                foreach (var pair in adjacencies)
                {
                    score += pair.Value.Count;
                }
            }
            return maxScore > 0 ? score / maxScore : 0;
        }

        public float EvaluateMatrixAreas(int[,] matrix)
        {
            float score = 0;
            //int[] length 5 -> minX, minY, maxX, maxY, number of tiles
            Dictionary<int, int[]> areas = new Dictionary<int, int[]>();
            for(int j = 0; j < matrix.GetLength(1); j++)
            {
                for(int i = 0; i < matrix.GetLength(0); i++)
                {
                    if(matrix[i,j] == 0)
                    {
                        continue;
                    }
                    var r = Rooms.Find(r => r.ID == matrix[i, j]);
                    if (r == null)
                    {
                        Debug.LogWarning("Room Not Found in Rooms: " + matrix[i, j]);
                        continue;
                    }
                    if (!areas.ContainsKey(matrix[i, j]))
                    {
                        areas.Add(matrix[i, j], new int[]{ i, j, i, j, 1});
                    }
                    //Update X values
                    if (areas[matrix[i, j]][0] > i)
                    {
                        areas[matrix[i, j]][0] = i;
                    }
                    else if (areas[matrix[i, j]][2] < i)
                    {
                        areas[matrix[i, j]][2] = i;
                    }
                    //Update y values
                    if (areas[matrix[i, j]][1] > j)
                    {
                        areas[matrix[i, j]][1] = j;
                    }
                    else if (areas[matrix[i, j]][3] < j)
                    {
                        areas[matrix[i, j]][3] = j;
                    }
                    areas[matrix[i, j]][4]++;
                }
            }

            foreach(var r in Rooms)
            {
                if(!areas.ContainsKey(r.ID))
                {
                    Debug.LogWarning("Room Not Found in Matrix: " + r.ID);
                    continue;
                }
                float distance = 0;
                float da = 0;
                float x = (areas[r.ID][2] - areas[r.ID][0]) + 1;
                float y = (areas[r.ID][3] - areas[r.ID][1]) + 1;
                if (r.ProportionType == ProportionType.RATIO)
                {
                    var expectedRatio = r.Ratio.x > r.Ratio.y ? (r.Ratio.x * 1.0f) / (r.Ratio.y * 1.0f) : (r.Ratio.y * 1.0f) / (r.Ratio.x * 1.0f);
                    var actualRatio = r.Ratio.x > r.Ratio.y ? (x) / (y) : (y) / (x);
                    distance = Mathf.Abs(expectedRatio - actualRatio);
                    float a = areas[r.ID][4]/(r.Ratio.x*r.Ratio.y*1.0f);
                    da = (a - (int)a);
                }
                else
                {
                    var dx = Mathf.Abs(x - r.WidthRange.x);
                    dx = dx > Mathf.Abs(x - r.WidthRange.y) ? Mathf.Abs(x - r.WidthRange.y) : dx;
                    var dy = Mathf.Abs(y - r.HeightRange.x);
                    dy = dy > Mathf.Abs(y - r.HeightRange.y) ? Mathf.Abs(y - r.HeightRange.y) : dy;
                    distance = (dx + dy) / 2;
                    var minA = r.WidthRange.x * r.HeightRange.x;
                    var maxA = r.WidthRange.y * r.HeightRange.y;
                    var a = x * y;
                    if(a < minA)
                    {
                        da = (minA - a) / minA;
                    }
                    else if(a > maxA)
                    {
                        da = (a - maxA) / a;
                    }
                }
                float rscore = 1 - (distance*0.5f + da*0.5f);
                score += rscore;
            }

            return (score/Rooms.Count);
        }
    }
}

