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
            private set
            {
                (Data as RoomData).bounds = value;
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
        public Vector2Int Height
        {
            get
            {
                if ((Data as RoomData).room == null)
                {
                    Debug.LogWarning("Room does not Exist");
                    return Vector2Int.zero;
                }
                return (Data as RoomData).room.height;
            }
            set
            {
                (Data as RoomData).room.height = value;
            }
        }
        public Vector2Int Width
        {
            get
            {
                if ((Data as RoomData).room == null)
                {
                    Debug.LogWarning("Room does not Exist");
                    return Vector2Int.zero;
                }
                return (Data as RoomData).room.width;
            }
            set
            {
                (Data as RoomData).room.width = value;
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
        public List<Vector2Int> TilePositions
        {
            get
            {
                if((Data as RoomData).tilePositions == null)
                {
                    (Data as RoomData).tilePositions = new List<Vector2Int>();
                }
                return (Data as RoomData).tilePositions;
            }
        }
        public bool[,] Surface
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

        public RoomController(Data data) : base(data)
        {
            AddTilePositions(Vector2Int.zero);
        }

        public override void LoadData()
        {
        }

        public override void Update()
        {
        }

        public bool AddTilePositions(Vector2Int pos)
        {
            //Debug.Log(pos);
            if (Bounds.x < (pos + Vector2Int.one).x || Bounds.y < (pos + Vector2Int.one).y)
            {
                Bounds = pos + Vector2Int.one;
            }
            if(TilePositions.Contains(pos))
            {
                return false;
            }
            TilePositions.Add(pos);
            return true;
        }

        public bool RemoveTilePosition(Vector2Int pos)
        {
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
                        AddTilePositions(Position + new Vector2Int(i,j));
                    }
                }
            }
            if(Bounds.y < size.y)
            {
                for (int i = 0; i < Bounds.x; i++)
                {
                    for (int j = Bounds.y; j < size.y; j++)
                    {
                        AddTilePositions(Position + new Vector2Int(i, j));
                    }
                }
            }
            Bounds = size;
        }
        public void ResizeToMin()
        {
            switch(ProportionType)
            {
                case ProportionType.RATIO:
                    Resize(Ratio);
                    break;
                case ProportionType.SIZE:
                    Resize(new Vector2Int(Width.x, Height.x));
                    break;
            }
        }

        public void Resize(Vector2Int size)
        {
            Bounds = size;
            //Remove if out of size
            TilePositions.Clear();
            //Add till size
            for(int i = 0; i < size.x; i++)
            {
                for(int j = 0; j < size.y; j++)
                {
                    AddTilePositions(new Vector2Int(i, j));
                }
            }
        }

        public override int GetHashCode()
        {
            return Label.GetHashCode();
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
                        collisions.Add(pos1 + Position);
                        break; // -> each tile can collide just once with 1 other room
                    }
                }
            }
            return (collisions.Count > 0);
        }

        /*public bool CheckCollision(RoomController other, out Vector2Int distance)
        {
            distance = Vector2Int.one * -1;
            bool collides = CheckCollision(other, out HashSet<Vector2Int> collisions);
            if (collides)
            {
                collisions.ToList().OrderByDescending((v) => v.magnitude);
                distance = collisions.First();
            }
            return collides;
        }*/

        public void Generate3D()
        {

        }

        public bool IsAdjacent(RoomController other, out Vector2Int minDistance)
        {
            minDistance = Centroid - other.Centroid;
            foreach (Vector2Int v1 in TilePositions)
            {
                foreach(Vector2Int v2 in other.TilePositions)
                {
                    var v = (Position + v1) - (other.Position + v2);
                    if(v.x == 0 && v.y == 0)
                    {
                        minDistance = Vector2Int.zero;
                        return true;
                    }
                    if(v.x == 0)
                    {
                        if(Mathf.Abs(v.y) < (Bounds.y + other.Bounds.y))
                        {
                            minDistance = Vector2Int.zero;
                            return true;
                        }
                    }
                    else if(v.y == 0)
                    {
                        if(Mathf.Abs(v.x) < (Bounds.x + other.Bounds.x))
                        {
                            minDistance = Vector2Int.zero;
                            return true;
                        }
                    }
                    if(v.magnitude < minDistance.magnitude)
                    {
                        minDistance = v;
                    }
                }
            }
            //minDistance = Mathf.Abs(minDistance.x) < Mathf.Abs(minDistance.y ? minDistance.x * Vector2Int.right : minDistance.y * Vector2Int.up;
            //Debug.Log(minDistance);
            return false;
        }
    }

}

