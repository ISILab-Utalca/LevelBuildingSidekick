using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick;
using System;
using LevelBuildingSidekick.Graph;
using Newtonsoft.Json;

[System.Serializable]
public class EdgeData: Data
{
    public int firstNodeID; //Should be ID (!!!)
    public int secondNodeID; //Should be ID (!!!)

    [JsonIgnore]
    public float thikness = 5; // -> static (??)

    [JsonIgnore]
    public override Type ControllerType => typeof(EdgeController);

    public EdgeData() { }

    public EdgeData(int _firstNodeID, int _secondNodeID)
    {
        firstNodeID = _firstNodeID;
        secondNodeID = _secondNodeID;
    }
}
