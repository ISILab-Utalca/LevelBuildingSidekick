using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
//[CreateAssetMenu(menuName = "LevelBuildingSidekick/Tools/GraphTools/Create Node")]
public class CreateNodeData : ToolData
{
    [JsonIgnore]
    public override Type ControllerType => typeof(CreateNodeController);
}
