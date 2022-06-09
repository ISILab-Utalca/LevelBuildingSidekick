using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick;


namespace LevelBuildingSidekick.Blueprint
{
    public class RoomController : Controller
    {
        public Vector2Int Position => (Data as RoomData).position;
        public bool[,] Surface => (Data as RoomData).surface;
        public Vector2Int Bounds => (Data as RoomData).bounds;
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
        public ProportionType ProportionType => (Data as RoomData).room.proportionType;


        public RoomController(Data data) : base(data)
        {
        }

        public override void LoadData()
        {
        }

        public override void Update()
        {
        }

        private void Expand(int x, int y)
        {
        }

    }


}

