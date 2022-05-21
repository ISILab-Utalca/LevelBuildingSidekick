using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "LevelBuildingSidekick/Tools/GraphTools/Create Node")]
public class CreateNodeData : ToolData
{
    public string label;

    public override Type ControllerType => typeof(CreateNodeController);
}
