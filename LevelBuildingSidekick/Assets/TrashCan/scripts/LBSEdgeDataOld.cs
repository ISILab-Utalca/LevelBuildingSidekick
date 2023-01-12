using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS;
using System;
using LBS.Graph;
using Newtonsoft.Json;

[System.Serializable]
public class LBSEdgeDataOld
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

    /// <summary>
    /// The direction of the edge.
    /// </summary>
    public EdgeDirection Direction { get; set; }

    /// <summary>
    /// Empty constructor, necessary for serialization with json.
    /// </summary>
    public LBSEdgeDataOld() { }

    /// <summary>
    /// Constructor for the LBSEdgeData class, which creates an edge between two nodes.
    /// </summary>
    /// <param name="n1">First node data object in the edge</param>
    /// <param name="n2">Second node data object in the edge</param>
    public LBSEdgeDataOld(LBSNodeDataOld n1, LBSNodeDataOld n2)
    {
        this.firstNodeLabel = n1.Label;
        this.secondNodeLabel = n2.Label;
    }

    /// <summary>
    /// Determines whether this edge contains the given node.
    /// </summary>
    /// <param name="nodeID"> The identifier of the node to check for.</param>
    /// <returns> True if the edge contains the given node, false otherwise.</returns>
    public bool Contains(string nodeID)
    {
        return firstNodeLabel == nodeID || secondNodeLabel == nodeID;
    }
}

/// <summary>
///  Enumeration of posibles edges directions.
/// </summary>
public enum EdgeDirection
{
    BIDIRECTIONAL,
    FORWARD,
    BACKWARDS
}
