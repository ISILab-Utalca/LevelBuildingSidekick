using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick;
using System.Linq;
using System;

namespace LevelBuildingSidekick.Blueprint
{
    public class RoomController : Controller
    {
        public Vector2Int Position
        {
            get
            {
                if ((Data as RoomData) == null)
                {
                    Debug.LogWarning("Room does not Exist");
                    return Vector2Int.zero;
                }
                return (Data as RoomData).position;
            }
            set
            {
                (Data as RoomData).position = value;
            }
        }

        //Change to length of surface
        public Vector2Int Bounds
        {
            get
            {
                if ((Data as RoomData) == null || TilePositions == null || TilePositions.Count == 0)
                {
                    return Vector2Int.zero;
                }
                int x = TilePositions.Max((v) => v.x) - TilePositions.Min((v) => v.x);
                int y = TilePositions.Max((v) => v.y) - TilePositions.Min((v) => v.y);
                return new Vector2Int(x,y) + Vector2Int.one;
            }
        }
        public Vector2Int Centroid
        {
            get
            {
                return Position + (Bounds / 2);
            }
            
        }
        public Vector2Int Center
        {
            get
            {
                return (Bounds / 2);
            }
        }
        public int ID
        {
            get
            {
                return GetHashCode();
            }
        }
        public RoomCharacteristics Room
        {
            get
            {
                if ((Data as RoomData).room == null)
                {
                    return new RoomCharacteristics();
                }
                return (Data as RoomData).room;
            }
            set
            {
                (Data as RoomData).room = value;
            }
        }

        public string Label
        {
            get
            {
                if ((Data as RoomData).room == null)
                {
                    Debug.LogWarning("Room does not Exist");
                    return "";
                }
                return (Data as RoomData).room.label;
            }
            set
            {
                (Data as RoomData).room.label = value;
            }
        }
        public Vector2Int HeightRange
        {
            get
            {
                if ((Data as RoomData).room == null)
                {
                    Debug.LogWarning("Room does not Exist");
                    return Vector2Int.zero;
                }
                return (Data as RoomData).room.heightRange;
            }
            set
            {
                (Data as RoomData).room.heightRange = value;
            }
        }
        public Vector2Int WidthRange
        {
            get
            {
                if ((Data as RoomData).room == null)
                {
                    Debug.LogWarning("Room does not Exist");
                    return Vector2Int.zero;
                }
                return (Data as RoomData).room.widthRange;
            }
            set
            {
                (Data as RoomData).room.widthRange = value;
            }
        }
        public Vector2Int Ratio
        {
            get
            {
                if ((Data as RoomData).room == null)
                {
                    Debug.LogWarning("Room does not Exist");
                    return Vector2Int.zero;
                }
                return (Data as RoomData).room.aspectRatio;
            }
            set
            {
                (Data as RoomData).room.aspectRatio = value;
            }
        }
        public ProportionType ProportionType
        {
            get
            {
                return (Data as RoomData).room.proportionType;
            }
            set
            {
                (Data as RoomData).room.proportionType = value;
            }
        }
        public HashSet<Vector2Int> TilePositions
        {
            get
            {
                if((Data as RoomData).tilePositions == null)
                {
                    (Data as RoomData).tilePositions = new HashSet<Vector2Int>();
                }
                return (Data as RoomData).tilePositions;
            }
            private set
            {
                TilePositions = value;
            }
        }
        public int[,] Surface
        {
            get
            {
                if ((Data as RoomData) == null)
                {
                    Debug.LogWarning("Room does not Exist");
                    return null;
                }
                return (Data as RoomData).surface;
            }
            set
            {
                (Data as RoomData).surface = value;
            }
        }
        public HashSet<int> NeighborsIDs
        {
            get
            {
                if ((Data as RoomData).room.neighbors == null)
                {
                    (Data as RoomData).room.neighbors = new List<int>();
                }
                return (Data as RoomData).room.neighbors.ToHashSet();
            }
            set
            {
                (Data as RoomData).room.neighbors = value.ToList();
            }
        }


        public RoomController(Data data) : base(data)
        {
            AddTileLocalPosition(Vector2Int.zero);
        }

        public override void LoadData()
        {
        }

        public override void Update()
        {
        }

        public bool AddTileLocalPosition(Vector2Int pos)
        {
            //Debug.Log(pos);
            if(TilePositions.Contains(pos))
            {
                return false;
            }
            TilePositions.Add(pos);
            return true;
        }
        public bool AddTilePosition(Vector2Int pos)
        {
            //Debug.Log(pos);
            Vector2Int localPos = pos - Position;
            if(AddTileLocalPosition(localPos))
            {
                Vector2Int delta = Vector2Int.zero;
                if(localPos.x < 0)
                {
                    delta.x = -localPos.x;
                }
                if(localPos.y < 0)
                {
                    delta.y = -localPos.y;
                }
                if(delta == Vector2Int.zero)
                {
                    return true;
                }
                Position -= delta;
                var tiles = TilePositions.ToArray();
                for(int i = 0; i < tiles.Length; i++)
                {
                    tiles[i] += delta;
                }
                TilePositions = tiles.ToHashSet();
                return true;
            }
            return false;
        }
        public bool RemoveTilePosition(Vector2Int pos)
        {
            if(pos == Position)
            {
                if(TilePositions.Remove(pos))
                {
                    Position = new Vector2Int(TilePositions.ToList().OrderBy((v) => v.x).First().x , TilePositions.ToList().OrderBy((v) => v.y).First().y);
                    var delta = Position - pos;
                    if(delta == Vector2Int.zero)
                    {
                        return true;
                    }
                    var tiles = TilePositions.ToArray();
                    for (int i = 0; i < tiles.Length; i++)
                    {
                        tiles[i] -= delta;
                    }
                    TilePositions = tiles.ToHashSet();
                    return true;
                }
                return false;
            }
            return TilePositions.Remove(pos);
        }

        public void Expand(Vector2Int size)
        {
            //Resize Surface
            if(Bounds.x < size.x)
            {
                for(int j = 0; j < Bounds.y; j++)
                {
                    for(int i = Bounds.x; i < size.x; i++)
                    {
                        AddTileLocalPosition(Position + new Vector2Int(i,j));
                    }
                }
            }
            if(Bounds.y < size.y)
            {
                for (int i = 0; i < Bounds.x; i++)
                {
                    for (int j = Bounds.y; j < size.y; j++)
                    {
                        AddTileLocalPosition(Position + new Vector2Int(i, j));
                    }
                }
            }
        }
        public void ResizeToMin()
        {
            switch(ProportionType)
            {
                case ProportionType.RATIO:
                    Resize(Ratio);
                    break;
                case ProportionType.SIZE:
                    Resize(new Vector2Int(WidthRange.x, HeightRange.x));
                    break;
            }
        }
        public void Resize(Vector2Int size)
        {
            //Remove if out of size
            TilePositions.Clear();
            //Add till size
            for(int i = 0; i < size.x; i++)
            {
                for(int j = 0; j < size.y; j++)
                {
                    AddTileLocalPosition(new Vector2Int(i, j));
                }
            }
        }

        public bool CheckCollision(Rect other)
        {
            Rect rect = new Rect(Position, Bounds);
            return rect.Overlaps(other);
        }
        public bool CheckCollision(RoomController other)
        {
            Rect r = new Rect(other.Position, other.Bounds);
            return CheckCollision(r);
        }
        public bool CheckCollision(RoomController other, out HashSet<Vector2Int> collisions)
        {
            collisions = new HashSet<Vector2Int>();

            foreach (Vector2Int pos1 in TilePositions)
            {
                foreach (Vector2Int pos2 in other.TilePositions)
                {
                    if ((pos1 + Position) == (pos2 + other.Position))
                    {
                        collisions.Add(pos1);
                        break; // -> each tile can collide just once with 1 other room
                    }
                }
            }
            return (collisions.Count > 0);
        }
        public bool IsAdjacent(RoomController other, out Vector2Int minDistance)
        {
            int minX = Mathf.Abs(Centroid.x - other.Centroid.x);
            int minY = Mathf.Abs(Centroid.y - other.Centroid.y);
            minDistance = Centroid - other.Centroid;
            foreach (Vector2Int v1 in TilePositions)
            {
                foreach(Vector2Int v2 in other.TilePositions)
                {
                    int x = Mathf.Abs((Position + v1).x - (other.Position + v2).x) + 1;
                    int y = Mathf.Abs((Position + v1).y - (other.Position + v2).y) + 1;
                    if (x + y == 1)// one must be 0 and the other 1
                    {
                        minDistance = Vector2Int.zero;
                        return true;
                    }
                    else
                    {
                        x = (Position + v1).x - (other.Position + v2).x > 0 ? x + 1 : -x -1;
                        y = (Position + v1).y - (other.Position + v2).y > 0 ? y + 1 : -y - 1;
                        var v = new Vector2Int(x,y);
                        minDistance = v;
                    }
                }
            }
            return false;
        }
        public void CalculateSurface()
        {
            Surface = new int[Bounds.x, Bounds.y];
            foreach(Vector2Int v in TilePositions)
            {
                Surface[v.x, v.y] = 1;
            }
        }

        public bool FulfillConstraints(out float distance)
        {
            var bounds = Bounds;
            distance = 0;
            if (ProportionType == ProportionType.RATIO)
            {
                var expectedRatio = Ratio.x > Ratio.y ? (Ratio.x * 1.0f) / (Ratio.y * 1.0f) : (Ratio.y * 1.0f) / (Ratio.x * 1.0f);
                var actualRatio = Ratio.x > Ratio.y ? (bounds.x * 1.0f) / (bounds.y * 1.0f) : (bounds.y * 1.0f) / (bounds.x * 1.0f);
                distance = Mathf.Abs(expectedRatio - actualRatio);
                return Mathf.Floor(expectedRatio) <= actualRatio && actualRatio <= Mathf.Ceil(expectedRatio);
            }
            else
            {
                if(bounds.x < WidthRange.x)
                {
                    distance += WidthRange.x - bounds.x;
                }
                else if(bounds.x > WidthRange.y)
                {
                    distance += bounds.x - WidthRange.y;
                }
                if(bounds.y < HeightRange.x)
                {
                    distance += HeightRange.x - bounds.y;
                }
                else if(bounds.y > HeightRange.y)
                {
                    distance += bounds.y - HeightRange.y;
                }
                return distance > 0;
            }
        }

        public void Generate3D()
        {

        }
        public override int GetHashCode()
        {
            return Label.GetHashCode();
        }

        public Vector2Int OuterTile(Vector2 direction)
        {
            var c = Center;
            float m = direction.y / direction.x;
            int x = 1;
            var outerTile = c;
            
            while(TilePositions.Contains(outerTile))
            {
                outerTile = c + new Vector2Int(x, (int)(x * m));
                x++;
            }
            return outerTile;
        }

        public List<Tuple<Vector2Int,Vector2Int>> GetCorners()
        {
            List<Tuple<Vector2Int, Vector2Int>> walls = new List<Tuple<Vector2Int, Vector2Int>>();
            CalculateSurface();
            Vector2Int start = Vector2Int.zero;
            for(int i = 0; i < Surface.GetLength(0); i++)
            {
                if(Surface[i,0] != 0)
                {
                    start.x = i;
                    break;
                }
            }

            var current = start;
            var wallStart = start;
            Vector2Int[] dirs = { Vector2Int.right, Vector2Int.down, Vector2Int.left, Vector2Int.up }; // follow this order cuz we start at the top and moving in x > 0
            int dir = 0;
            do
            {
                var aux = current + dirs[dir];
                if(aux.x >= Surface.GetLength(0) || aux.x < 0 || aux.y >= Surface.GetLength(1) || aux.y < 0 ||
                    Surface[aux.x, aux.y] != 0)
                {
                    var border1 = aux + dirs[(dir + 1) % dirs.Length];
                    var border2 = aux + dirs[(dir + 3) % dirs.Length];
                    if(border1.x >= Surface.GetLength(0) || border1.x < 0 || border1.y >= Surface.GetLength(1) || border1.y < 0 ||
                        border2.x >= Surface.GetLength(0) || border2.x < 0 || border2.y >= Surface.GetLength(1) || border2.y < 0 ||
                        Surface[border1.x, border1.y] == 0 || Surface[border2.x, border2.y] == 0)
                    {
                        current = aux;
                        continue;
                    }
                    //Concave wall?;
                    else
                    {
                        walls.Add(new Tuple<Vector2Int, Vector2Int>(wallStart, current));

                        var n = aux + dirs[(dir + 1) % dirs.Length];
                        var nborder1 = n + dirs[(dir + 2) % dirs.Length];//dir+1 + 1
                        var nborder2 = n + dirs[(dir + 4) % dirs.Length];//dir+3 + 1
                        if (nborder1.x >= Surface.GetLength(0) || nborder1.x < 0 || nborder1.y >= Surface.GetLength(1) || nborder1.y < 0 ||
                            nborder2.x >= Surface.GetLength(0) || nborder2.x < 0 || nborder2.y >= Surface.GetLength(1) || nborder2.y < 0 ||
                            Surface[nborder1.x, nborder1.y] == 0 || Surface[nborder2.x, nborder2.y] == 0)
                        {
                            current = n;
                            wallStart = n;
                            continue;
                        }
                        n = aux + dirs[(dir + 3) % dirs.Length];
                        nborder1 = n + dirs[(dir + 4) % dirs.Length];//dir+1 + 3 => could be the same as above
                        nborder2 = n + dirs[(dir + 6) % dirs.Length];//dir+3 + 3 => could be the same as above
                        if (nborder1.x >= Surface.GetLength(0) || nborder1.x < 0 || nborder1.y >= Surface.GetLength(1) || nborder1.y < 0 ||
                            nborder2.x >= Surface.GetLength(0) || nborder2.x < 0 || nborder2.y >= Surface.GetLength(1) || nborder2.y < 0 ||
                            Surface[nborder1.x, nborder1.y] == 0 || Surface[nborder2.x, nborder2.y] == 0)
                        {
                            current = n;
                            wallStart = n;
                            continue;
                        }

                        Debug.LogError("Missing case");
                        return walls;
                    }
                }
                else
                {
                    //make wall
                    walls.Add(new Tuple<Vector2Int, Vector2Int>(wallStart, current));
                    wallStart = current;
                    dir = (dir + 1) % dirs.Length;
                }
                current = aux;
            }
            while (current != start || walls.Count < 4);

            return walls;
        }

        public List<List<Vector2Int>> GetWalls()
        {
            List<List<Vector2Int>> walls = new List<List<Vector2Int>>();
            var corners = GetCorners();
            foreach (Tuple<Vector2Int, Vector2Int> t in corners)
            {
                walls.Add(new List<Vector2Int>());
            }
            for(int i = 0; i < corners.Count; i++)
            {
                if(corners[i].Item1.x == corners[i].Item2.x)
                {
                    int x = corners[i].Item1.x;
                    int y1 = corners[i].Item1.y < corners[i].Item2.y ? corners[i].Item1.y : corners[i].Item2.y;
                    int y2 = corners[i].Item1.y > corners[i].Item2.y ? corners[i].Item1.y : corners[i].Item2.y;
                    for(int j = y1; j <= y2; j++)
                    {
                        walls[i].Add(new Vector2Int(x, j));
                    }
                }
                else
                {
                    int y = corners[i].Item1.y;
                    int x1 = corners[i].Item1.x < corners[i].Item2.x ? corners[i].Item1.x : corners[i].Item2.x;
                    int x2 = corners[i].Item1.x > corners[i].Item2.x ? corners[i].Item1.x : corners[i].Item2.x;
                    for (int j = x1; j <= x2; j++)
                    {
                        walls[i].Add(new Vector2Int(j, y));
                    }
                }
            }
            return walls;
        }
    }

}

