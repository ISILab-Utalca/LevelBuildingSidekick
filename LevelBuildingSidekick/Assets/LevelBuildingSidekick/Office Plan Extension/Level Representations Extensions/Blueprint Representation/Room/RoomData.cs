using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelBuildingSidekick.Blueprint
{
    public class RoomData : Data
    {
        public RoomCharacteristics room;


        public Vector2Int position = Vector2Int.zero;
        public Vector2Int bounds = Vector2Int.one;
        public Vector2Int outerBounds = Vector2Int.one;
        public List<Vector2Int> tilePositions = new List<Vector2Int>();
        //public bool[,] surface;


        public override Type ControllerType => typeof(RoomController);


    }
}

