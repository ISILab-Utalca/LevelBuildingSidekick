using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "LevelBuildingSidekick/Tools/GraphTools/Select Node")]
public class SelectNodeData : ToolData
{
    public override Type ControllerType => typeof(SelectNodeController);
}
