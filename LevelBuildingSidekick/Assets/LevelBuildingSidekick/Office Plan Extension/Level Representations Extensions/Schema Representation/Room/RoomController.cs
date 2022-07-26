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
                int x = TilePositions.OrderBy((v) => v.x).Last().x - TilePositions.OrderBy((v) => v.x).First().x;
                int y = TilePositions.OrderBy((v) => v.y).Last().y - TilePositions.OrderBy((v) => v.y).First().y;
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
    }

}

