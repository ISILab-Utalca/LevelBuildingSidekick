using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
//[CreateAssetMenu(menuName = "LevelBuildingSidekick/Tools/GraphTools/Connect Nodes")]
public class ConnectNodesData : ToolData
{
    public float thikness = 2;

    public override Type ControllerType => typeof(ConnectNodesController);

}
