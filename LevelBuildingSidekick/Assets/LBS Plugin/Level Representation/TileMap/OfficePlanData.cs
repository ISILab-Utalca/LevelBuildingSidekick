using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick;
using System;
using LevelBuildingSidekick.Graph;
using LevelBuildingSidekick.Schema;
using Newtonsoft.Json;

namespace LevelBuildingSidekick.OfficePlan
{
    //[CreateAssetMenu(menuName = "LevelBuildingSidekick/Level Represetation/OfficePlan Representation/Level")]
    public class OfficePlanData : LBSRepesentationData
    {
        public LBSGraphData graph;
        public SchemaData schema;
        public List<RoomCharacteristics> characteristics;

        //public ToolkitData toolkit;
        //public GameObject floor;
        //public GameObject wall;
        //public GameObject door;

        [JsonIgnore]
        public override Type ControllerType => typeof(OfficePlanController);

    }

    public enum TeselationType // Delete (??)
    {
        DOWNSCALE,
        SIZE,
    }
}

