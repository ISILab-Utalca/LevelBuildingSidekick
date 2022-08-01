using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick;
using System.Linq;
using System;

namespace LevelBuildingSidekick.Schema
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
                if ((Data as RoomData) == null || Tiles == null || Tiles.Count == 0)
                {
                    return Vector2Int.zero;
                }
                int x = Tiles.Max((v) => v.x) - Tiles.Min((v) => v.x);
                int y = Tiles.Max((v) => v.y) - Tiles.Min((v) => v.y);
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
        public HashSet<Tile> Tiles
        {
            get
            {
                if((Data as RoomData).tiles == null)
                {
                    (Data as RoomData).tiles = new HashSet<Tile>();
                }
                return (Data as RoomData).tiles;
            }
            private set
            {
                Tiles = value;
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
            Tiles.Add(Tile.zero);
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
            var t = new Tile(pos);
            if(Tiles.Contains(t))
            {
                return false;
            }
            Tiles.Add(t);
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
                var tiles = Tiles.ToArray();
                for(int i = 0; i < tiles.Length; i++)
                {
                    tiles[i] += delta;
                }
                Tiles = tiles.ToHashSet();
                return true;
            }
            return false;
        }
        public bool RemoveTilePosition(Vector2Int pos)
        {
            var t = new Tile(pos);
            if(t == Position)
            {
                if(Tiles.Remove(t))
                {
                    Position = new Vector2Int(Tiles.ToList().OrderBy((v) => v.x).First().x , Tiles.ToList().OrderBy((v) => v.y).First().y);
                    var delta = Position - t;
                    if(delta == Vector2Int.zero)
                    {
                        return true;
                    }
                    var tiles = Tiles.ToArray();
                    for (int i = 0; i < tiles.Length; i++)
                    {
                        tiles[i] -= delta;
                    }
                    Tiles = tiles.ToHashSet();
                    return true;
                }
                return false;
            }
            return Tiles.Remove(t);
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
            Tiles.Clear();
            //Add till size
            for(int i = 0; i < size.x; i++)
            {
                for(int j = 0; j < size.y; j++)
                {
                    AddTileLocalPosition(new Vector2Int(i, j));
                }
            }
            //Debug.Log(Label + ": " + TilePositions.Count);
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
        public bool CheckCollision(RoomController other, out HashSet<Tile> collisions)
        {
            collisions = new HashSet<Tile>();

            foreach (Tile pos1 in Tiles)
            {
                foreach (Tile pos2 in other.Tiles)
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
            foreach (Tile v1 in Tiles)
            {
                foreach(Tile v2 in other.Tiles)
                {
                    int x = (int)Mathf.Abs((Position + v1).x - (other.Position + v2).x) + 1;
                    int y = (int)Mathf.Abs((Position + v1).y - (other.Position + v2).y) + 1;
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
            foreach(Tile v in Tiles)
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
            var c = new Tile(Center);
            float m = direction.y / direction.x;
            int x = 1;
            var outerTile = c;
            
            while(Tiles.Contains(outerTile))
            {
                outerTile = c + new Vector2Int(x, (int)(x * m));
                x++;
            }
            return outerTile.vector;
        }
        public void CategorizeTile(Tile t)
        {
            Vector2Int[] sidedirs = { Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down };
            Vector2Int[] diagdirs = { Vector2Int.right + Vector2Int.up, Vector2Int.up + Vector2Int.left, Vector2Int.left + Vector2Int.down, Vector2Int.down + Vector2Int.right };

            int s = 0;
            int d = 0;
            for (int i = 0; i < 4; i++)
            {
                //revisar laterales para saber si es esquina externa
                if (Tiles.Contains(t + sidedirs[i]))
                {
                    s += Mathf.RoundToInt(Mathf.Pow(2, i));
                }
                //revisar diagonales para saber si es esquina interna
                if (Tiles.Contains(t + diagdirs[i]))
                {
                    d += Mathf.RoundToInt(Mathf.Pow(2, i));
                }
            }
            t.SetSides(s);
            t.SetDiagonals(d);
        }
        public void CategorizeTiles()
        {
            foreach (Tile t in Tiles)
            {
                CategorizeTile(t);
            }
        }
        public List<Tile> GetCorners()
        {
            List<Tile> corners = new List<Tile>();
            if(Tiles.Count == 1)
            {
                corners.Add(Tile.zero);
                return corners;
            }
            foreach (Tile t in Tiles)
            {
                //s = 3,6,9,12 are corners, 15 is a single Tile, 7,11,13,14 are tile with 3 free sides
                //s = 0 means inner Tile
                if(t.sideCode != 0)
                {
                    if (t.sideCode % 3 == 0 || t.sideCode == 7 || t.sideCode == 11 || t.sideCode == 13 || t.sideCode == 14)
                    {
                        corners.Add(t);
                    }  
                }//d = 1,2,4,8 are inner corners 7,11,13,14 are inner corners wiht a free side ; d = 0 inner Tile
                else if (t.diagCode != 0 && (Mathf.IsPowerOfTwo(t.diagCode) || t.diagCode == 7 || t.diagCode == 11 || t.diagCode == 13 || t.diagCode == 14))
                {
                    corners.Add(t);
                }
            }
            return corners;
        }
        public List<Tuple<Tile,Tile>> GetWallCorners()
        {
            List<Tuple<Tile, Tile>> walls = new List<Tuple<Tile, Tile>>();
            var corners = GetCorners();
            Vector2Int[] dirs = { Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down };
            foreach (Tile t in corners)
            {
                //inner corners don't connect between them so its safe to only start with external corners
                if(t.sideCode <= 0)
                {
                    continue;
                }
                foreach(Vector2Int v in dirs)
                {
                    //3 sided corner
                    if (t.sideCode == 7 || t.sideCode == 11 || t.sideCode == 13 || t.sideCode == 14)
                    {
                        walls.Add(new Tuple<Tile, Tile>(t, t));
                        continue;
                    }
                    //2 Sided corners search for other corner
                    var step = t;
                    while(true)
                    {
                        step += v;
                        //must find other corner before exiting bounds
                        if(!Tiles.Contains(step))
                        {
                            //Debug.LogWarning("Should've found corner");
                            break;
                        }
                        if(corners.Contains(step))
                        {
                            //if external add wall
                            step = corners.Find((c) => c == step);
                            if(step.sideCode > 0) //external corner
                            {
                                walls.Add(new Tuple<Tile, Tile>(t, step));
                                break;
                            }
                            //if internal wall is 1 tile shorter
                            else //internal corner
                            {
                                walls.Add(new Tuple<Tile, Tile>(t, step-v));
                                break;
                            }
                        }
                    }
                }
            }
            return walls;
        }
        public List<List<Tile>> GetWalls()
        {
            List<List<Tile>> walls = new List<List<Tile>>();
            var wallCorners = GetWallCorners();
            foreach(Tuple<Tile,Tile> t in wallCorners)
            {
                var l = new List<Tile>();
                if(t.Item1 == t.Item2)
                {
                    l.Add(t.Item1);
                    walls.Add(l);
                    continue;
                }
                if(t.Item1.x == t.Item2.x)
                {
                    if(t.Item1.y < t.Item2.y)
                    {
                        for(int i = 0; i <= t.Item2.y - t.Item1.y; i++)
                        {
                            l.Add(t.Item1 + Vector2Int.up * i);
                        }
                    }
                    else
                    {
                        for (int i = 0; i <= t.Item1.x - t.Item2.x; i++)
                        {
                            l.Add(t.Item2 + Vector2Int.up * i);
                        }
                    }
                    walls.Add(l);
                }
                else
                {
                    if (t.Item1.x < t.Item2.x)
                    {
                        for (int i = 0; i <= t.Item2.x - t.Item2.x; i++)
                        {
                            l.Add(t.Item1 + Vector2Int.right * i);
                        }
                    }
                    else
                    {
                        for (int i = 0; i <= t.Item1.x - t.Item2.x; i++)
                        {
                            l.Add(t.Item2 + Vector2Int.right * i);
                        }
                    }
                    walls.Add(l);
                }
            }
            return walls;
        }

        public List<Tile> GetWallTiles()
        {
            List<Tile> wallTiles = new List<Tile>();
            foreach (Tile t in Tiles)
            {
                if(Mathf.IsPowerOfTwo(t.sideCode) || t.sideCode%5 == 0)
                {
                    wallTiles.Add(t);
                }
            }
            return wallTiles;
        }
    }

}

