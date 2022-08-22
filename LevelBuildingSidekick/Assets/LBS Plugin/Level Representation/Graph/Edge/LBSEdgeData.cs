using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick;
using System;
using LevelBuildingSidekick.Graph;
using Newtonsoft.Json;

[System.Serializable]
public class LBSEdgeData: Data
{
    [JsonIgnore]
    public LBSNodeData firstNode;
    [JsonIgnore]
    public LBSNodeData secondNode;

    private string firstNodeLabel;
    private string secondNodeLabel;

    public string FirstNodeLabel
    {
        get
        {
            if (firstNode != null)
            {
                firstNodeLabel = firstNode.label;
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
                secondNodeLabel = secondNode.label;
            }
            return secondNodeLabel;
        }
        set
        {
            secondNodeLabel = value;
        }
    }

    [JsonIgnore]
    public override Type ControllerType => throw new NotImplementedException();

    [JsonIgnore]
    public float thikness = 5; // -> static (??)


    public LBSEdgeData() { }

    public LBSEdgeData(LBSNodeData firstNode, LBSNodeData secondNode)
    {
        this.firstNode = firstNode;
        this.secondNode = secondNode;
    }


    public bool Contains(string nodeID)
    {
        return FirstNodeLabel == nodeID || SecondNodeLabel == nodeID;
    }
}
