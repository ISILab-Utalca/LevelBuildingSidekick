using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS;
using System;
using LBS.Graph;
using Newtonsoft.Json;

[System.Serializable]
public class LBSEdgeData
{
    //[JsonIgnore]
    //public LBSNodeData firstNode;
    //[JsonIgnore]
    //public LBSNodeData secondNode;

    [SerializeField,JsonRequired]
    private string firstNodeLabel;
    [SerializeField,JsonRequired]
    private string secondNodeLabel;

    [HideInInspector,JsonIgnore]
    public string FirstNodeLabel => firstNodeLabel;

    [HideInInspector,JsonIgnore]
    public string SecondNodeLabel => secondNodeLabel;

    [JsonIgnore]
    public float thikness = 5; // -> static (??)

    public EdgeDirection Direction { get; set; }

    /// <summary>
    /// Empty constructor, necessary for serialization with json.
    /// </summary>
    public LBSEdgeData() { }

    public LBSEdgeData(LBSNodeData n1, LBSNodeData n2)
    {
        this.firstNodeLabel = n1.Label;
        this.secondNodeLabel = n2.Label;
    }


    public bool Contains(string nodeID)
    {
        return firstNodeLabel == nodeID || secondNodeLabel == nodeID;
    }
}

public enum EdgeDirection
{
    BIDIRECTIONAL,
    FORWARD,
    BACKWARDS
}
