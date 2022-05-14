using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick;


[System.Serializable]
public class NodeData: Data
{
    string label;

    [Header("View Data")]

    Vector2 position;
    float radius;
}
