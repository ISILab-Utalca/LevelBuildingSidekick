using LevelBuildingSidekick;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelBuildingSidekick.Schema
{
    //[CreateAssetMenu(menuName = "LevelBuildingSidekick/Level Represetation/Schema Representation/Level")]
    public class SchemaData : LevelRepresentationData
    {
        //public int[,] tilemap;
        public int tileSize;
        public int x;
        public int y;
        public List<RoomData> rooms;
        //public Dictionary<int, HashSet<int>> connections;

        [JsonIgnore]
        public override Type ControllerType => typeof(SchemaController);

        public SchemaData Clone()
        {
            var schema = new SchemaData();
            schema.tileSize = tileSize;
            schema.x = x;
            schema.y = y;
            schema.rooms = new List<RoomData>();
            foreach(RoomData r in rooms)
            {
                schema.rooms.Add(r.Clone());
            }
            return schema;
        }
    }
}

