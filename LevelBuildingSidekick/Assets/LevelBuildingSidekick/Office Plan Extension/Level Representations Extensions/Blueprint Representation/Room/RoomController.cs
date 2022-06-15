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
        public Vector2Int InnerBounds
        {
            get
            {
                if ((Data as RoomData) == null)
                {
                    Debug.LogWarning("Room does not Exist");
                    return Vector2Int.zero;
                }
                return (Data as RoomData).bounds;
            }
            set
            {
                (Data as RoomData).bounds = value;
            }
        }
        public Vector2Int OuterBounds
        {
            get
            {
                if ((Data as RoomData) == null)
                {
                    Debug.LogWarning("Room does not Exist");
                    return Vector2Int.zero;
                }
                return (Data as RoomData).outerBounds;
            }
            private set
            {
                (Data as RoomData).outerBounds = value;
            }
        }
        public Vector2Int Centroid
        {
            get
            {
                return Position + (InnerBounds / 2);
            }
            
        }
        public Vector2Int Center
        {
            get
            {
                return (InnerBounds / 2);
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
        public List<GameObject> FloorTiles
        {
            get
            {
                if (Room.floorTiles == null)
                {
                    Room.floorTiles = new List<GameObject>();
                }
                return Room.floorTiles;
            }
        }

        public List<GameObject> WallTiles
        {
            get
            {
                if (Room.wallTiles == null)
                {
                    Room.wallTiles = new List<GameObject>();
                }
                return Room.wallTiles;
            }
        }

        public List<GameObject> DoorTiles
        {
            get
            {
                if (Room.doorTiles == null)
                {
                    Room.doorTiles = new List<GameObject>();
                }
                return Room.doorTiles;
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
            if (OuterBounds.x < (pos + Vector2Int.one).x || OuterBounds.y < (pos + Vector2Int.one).y)
            {
                OuterBounds = pos + Vector2Int.one;
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
            if(InnerBounds.x < size.x)
            {
                for(int j = 0; j < InnerBounds.y; j++)
                {
                    for(int i = InnerBounds.x; i < size.x; i++)
                    {
                        AddTilePositions(Position + new Vector2Int(i,j));
                    }
                }
            }
            if(InnerBounds.y < size.y)
            {
                for (int i = 0; i < InnerBounds.x; i++)
                {
                    for (int j = InnerBounds.y; j < size.y; j++)
                    {
                        AddTilePositions(Position + new Vector2Int(i, j));
                    }
                }
            }
            InnerBounds = size;
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
            InnerBounds = size;
            //Remove if out of size
            foreach(Vector2Int v in TilePositions)
            {
                if(v.x > size.x || v.y > size.y)
                {
                    TilePositions.Remove(v);
                }
            }
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

        /*public bool CheckCollision(Rect other, bool useOuterBounds)
        {
            Rect rect = new Rect(Position, InnerBounds);
            if (useOuterBounds)
            {
                rect = new Rect(Position, OuterBounds);
            }
            return rect.Overlaps(other);
        }*/

        /*public bool CheckCollision(RoomController other, bool useOuterBounds)
        {
            Rect r = new Rect(other.Position, other.InnerBounds);
            if (useOuterBounds)
            {
                r = new Rect(other.Position, other.OuterBounds);
            }
            return CheckCollision(r, useOuterBounds);
        }*/

        /*public bool CheckCollision(RoomController other, out HashSet<Vector2Int> collisions)
        {
            collisions = new HashSet<Vector2Int>();
            if (!CheckCollision(other, true))
            {
                return false;
            }

            foreach (Vector2Int pos1 in TilePositions)
            {
                foreach (Vector2Int pos2 in other.TilePositions)
                {
                    if ((pos1 + Position) == (pos2 + other.Position))
                    {
                        collisions.Add((pos1 - Center));
                        break; // -> each tile can collide just once with 1 other room
                    }
                }
            }
            return (collisions.Count > 0);
        }*/

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

    }

}

