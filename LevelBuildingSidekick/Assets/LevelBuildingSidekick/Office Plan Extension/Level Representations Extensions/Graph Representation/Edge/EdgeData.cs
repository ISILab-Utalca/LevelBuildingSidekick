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
    public NodeData node1; //Should be ID (!!!)
    public NodeData node2; //Should be ID (!!!)
    public float thikness = 5; // -> static (??)

    [JsonIgnore]
    public override Type ControllerType => typeof(EdgeController);
}
