using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick;
using System.Linq;


namespace LevelBuildingSidekick.Blueprint
{
    public class RoomController : Controller
    {
        public Vector2Int Position
        {
            get
            {
                return (Data as RoomData).position;
            }
        }
        public bool[,] Surface
        {
            get
            {
                return (Data as RoomData).surface;
            }
        }
        //Change to length of surface
        public Vector2Int Bounds
        {
            get
            {
                return (Data as RoomData).bounds;
            }
            set
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
        }
        public ProportionType ProportionType
        {
            get
            {
                return (Data as RoomData).room.proportionType;
            }
        }


        public RoomController(Data data) : base(data)
        {
        }

        public override void LoadData()
        {
        }

        public override void Update()
        {
        }

        public bool AddTilePositions(Vector2Int pos)
        {
            HashSet<Vector2Int> set = (Data as RoomData).tilePositions;
            return set.Add(pos);
        }

        public bool RemoveTilePosition(Vector2Int pos)
        {
            HashSet<Vector2Int> set = (Data as RoomData).tilePositions;
            return set.Remove(pos);
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
    }


}

