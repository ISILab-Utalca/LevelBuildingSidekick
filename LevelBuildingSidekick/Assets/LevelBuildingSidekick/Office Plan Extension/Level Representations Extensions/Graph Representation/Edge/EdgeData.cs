using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick;
using System;

[System.Serializable]
public class EdgeData: Data
{
    public NodeData node1;
    public NodeData node2;
    public float thikness = 5; // -> static?

    public override Type ControllerType => typeof(EdgeController);
}
