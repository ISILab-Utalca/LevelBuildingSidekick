using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick;
using System;
using LevelBuildingSidekick.Graph;
using Newtonsoft.Json;

[System.Serializable]
public class EdgeData: Data
{
    [JsonIgnore]
    public NodeData firstNode;
    [JsonIgnore]
    public NodeData secondNode;

    private string firstNodeLabel;
    private string secondNodeLabel;
    public string FirstNodeLabel
    {
        get
        {
            if (firstNode != null)
            {
                firstNodeLabel = firstNode.room.label;
            }
            return firstNodeLabel;
        }
        set
        {
            firstNodeLabel = value;
        }
    }
    public string SecondNodeLabel
    {
        get
        {
            if(secondNode != null)
            {
                secondNodeLabel = secondNode.room.label;
            }
            return secondNodeLabel;
        }
        set
        {
            secondNodeLabel = value;
        }
    }

    [JsonIgnore]
    public float thikness = 5; // -> static (??)

    [JsonIgnore]
    public override Type ControllerType => typeof(EdgeController);

    public EdgeData() { }

    public EdgeData(NodeData _firstNode, NodeData _secondNode)
    {
        firstNode = _firstNode;
        secondNode = _secondNode;
    }
}
