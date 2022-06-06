using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick;
using System;

public class OfficePlanData : LevelRepresentationData
{
    GraphData graph;
    BlueprintData blueprint;

    public override Type ControllerType => typeof(OfficePlanController);

}
