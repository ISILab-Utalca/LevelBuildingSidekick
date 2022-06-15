using LevelBuildingSidekick;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelBuildingSidekick.Blueprint
{
    [CreateAssetMenu(menuName = "LevelBuildingSidekick/Level Represetation/Blueprint Representation/Level")]
    public class BlueprintData : LevelRepresentationData
    {
        public int[,] tilemap;
        public int step;
        public int stride;
        public Vector2Int size;
        public List<RoomData> rooms;



        public override Type ControllerType => typeof(BlueprintController);

    }
}

