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
        //public Vector2Int bounds = Vector2Int.one;
        //public Vector2Int outerBounds = Vector2Int.one;
        public HashSet<Vector2Int> tilePositions = new HashSet<Vector2Int>();
        public int[,] surface = new int[1,1];


        public override Type ControllerType => typeof(RoomController);

        public RoomData Clone()
        {
            var r = new RoomData();
            r.room = room;
            r.position = position;
            foreach(Vector2Int v in tilePositions)
            {
                r.tilePositions.Add(v);
            }
            return r;
        }
    }
}

