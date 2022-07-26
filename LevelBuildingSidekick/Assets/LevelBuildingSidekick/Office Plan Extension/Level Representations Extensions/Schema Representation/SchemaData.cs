using LevelBuildingSidekick;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelBuildingSidekick.Blueprint
{
    //[CreateAssetMenu(menuName = "LevelBuildingSidekick/Level Represetation/Schema Representation/Level")]
    public class SchemaData : LevelRepresentationData
    {
        //public int[,] tilemap;
        public int tileSize;
        public Vector2Int size;
        public List<RoomData> rooms;
        //public Dictionary<int, HashSet<int>> connections;
        public override Type ControllerType => typeof(SchemaController);

        public SchemaData Clone()
        {
            var schema = new SchemaData();
            schema.tileSize = tileSize;
            schema.size = size;
            schema.rooms = new List<RoomData>();
            foreach(RoomData r in rooms)
            {
                schema.rooms.Add(r.Clone());
            }
            return schema;
        }
    }
}

