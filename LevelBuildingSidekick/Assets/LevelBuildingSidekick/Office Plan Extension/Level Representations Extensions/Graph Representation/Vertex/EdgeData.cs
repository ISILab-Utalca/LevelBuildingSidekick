using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick;
using System;

[System.Serializable]
public class EdgeData: Data
{
    NodeController inNode;
    NodeController outNode;


    public override Type ControllerType => throw new NotImplementedException();
}
