using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "LevelBuildingSidekick/Tools/GraphTools/Delete Node")]
public class DeleteNodeData : ToolData
{
    public override Type ControllerType => typeof(DeleteNodeController);
}
