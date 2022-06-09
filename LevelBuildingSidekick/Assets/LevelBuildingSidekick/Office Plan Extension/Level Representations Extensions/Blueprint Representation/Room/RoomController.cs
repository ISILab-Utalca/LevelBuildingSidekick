using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick;


namespace LevelBuildingSidekick.Blueprint
{
    public class RoomController : Controller
    {
        public Vector2 Position => (Data as RoomData).position;
        public bool[,] Surface => (Data as RoomData).surface;
        public Vector2Int Bounds => (Data as RoomData).bounds;
        public string Label => (Data as RoomData).room.label;
        public Vector2Int Height => (Data as RoomData).room.height;
        public Vector2Int Width => (Data as RoomData).room.width;
        public Vector2Int Ratio => (Data as RoomData).room.aspectRatio;
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

