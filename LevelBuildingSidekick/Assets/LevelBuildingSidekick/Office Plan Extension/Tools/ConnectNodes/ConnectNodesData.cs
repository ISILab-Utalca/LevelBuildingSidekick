using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
//[CreateAssetMenu(menuName = "LevelBuildingSidekick/Tools/GraphTools/Connect Nodes")]
public class ConnectNodesData : ToolData
{
    [JsonIgnore]
    public float thikness = 2;

    [JsonIgnore]
    public override Type ControllerType => typeof(ConnectNodesController);

}
