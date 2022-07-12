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
        public int[,] tilemap;
        public int tileSize;
        public Vector2Int size;
        public List<RoomData> rooms;



        public override Type ControllerType => typeof(SchemaController);

    }
}

