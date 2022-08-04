using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelBuildingSidekick.Schema
{
    public class RoomData : Data
    {
        public RoomCharacteristics room;

        public int x;
        public int y;
        //public Vector2Int bounds = Vector2Int.one;
        //public Vector2Int outerBounds = Vector2Int.one;
        public HashSet<Tile> tiles = new HashSet<Tile>(); // Should be list

        [JsonIgnore]
        public int[,] surface = new int[1,1];


        [JsonIgnore]
        public override Type ControllerType => typeof(RoomController);

        public RoomData Clone()
        {
            var r = new RoomData();
            r.room = room;
            r.x = x;
            r.y = y;
            foreach(Tile t in tiles)
            {
                r.tiles.Add(t);
            }
            return r;
        }
    }
}

