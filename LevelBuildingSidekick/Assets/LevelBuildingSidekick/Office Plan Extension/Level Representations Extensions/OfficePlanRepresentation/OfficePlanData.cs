using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick;
using System;
using LevelBuildingSidekick.Graph;
using LevelBuildingSidekick.Blueprint;


namespace LevelBuildingSidekick.OfficePlan
{
    [CreateAssetMenu(menuName = "LevelBuildingSidekick/Level Represetation/OfficePlan Representation/Level")]
    public class OfficePlanData : LevelRepresentationData
    {
        public GraphData graph;
        public SchemaData schema;
        public TeselationType type;
        public ToolkitData toolkit;
        public GameObject floor;
        public GameObject wall;
        public GameObject door;

        public override Type ControllerType => typeof(OfficePlanController);

    }

    public enum TeselationType
    {
        DOWNSCALE,
        SIZE,
    }
}

