using LevelBuildingSidekick;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueprintData : LevelRepresentationData
{
    public int[,] tilemap;
    public int tileSize;

    public override Type ControllerType => typeof(BlueprintController);

}
