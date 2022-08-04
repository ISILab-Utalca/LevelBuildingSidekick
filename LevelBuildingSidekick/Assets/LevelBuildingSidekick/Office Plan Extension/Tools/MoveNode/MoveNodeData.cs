using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
//[CreateAssetMenu(menuName = "LevelBuildingSidekick/Tools/GraphTools/Move Node")]
public class MoveNodeData : ToolData
{
    [JsonIgnore]
    public override Type ControllerType => typeof(MoveNodeController);
}
