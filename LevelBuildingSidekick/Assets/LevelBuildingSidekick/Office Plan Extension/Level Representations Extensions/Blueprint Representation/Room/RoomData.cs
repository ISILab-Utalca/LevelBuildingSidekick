using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelBuildingSidekick.Blueprint
{
    public class RoomData : Data
    {
        public RoomCharacteristics room;


        public Vector2Int position;
        public Vector2Int bounds;
        public bool[,] surface;


        public override Type ControllerType => typeof(RoomController);


    }
}

