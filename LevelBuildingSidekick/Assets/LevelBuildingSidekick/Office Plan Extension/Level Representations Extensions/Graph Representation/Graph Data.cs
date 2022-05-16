using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick;
using System;

[System.Serializable]
public class GraphData : Data
{
    List<NodeController> nodes;
    List<VertexController> vertexes;


    public override Type ControllerType => throw new NotImplementedException();
}
