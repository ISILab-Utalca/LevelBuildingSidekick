using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick;
using System;

[System.Serializable]
public class NodeData : Data
{
    string label;

    [Header("View Data")]

    Vector2 position;
    float radius;


    public override Type ControllerType => throw new NotImplementedException();
}
