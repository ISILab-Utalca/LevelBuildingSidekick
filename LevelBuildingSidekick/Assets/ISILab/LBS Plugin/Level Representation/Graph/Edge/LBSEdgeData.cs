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
    //[JsonIgnore]
    //public LBSNodeData firstNode;
    //[JsonIgnore]
    //public LBSNodeData secondNode;

    private string firstNodeLabel;
    private string secondNodeLabel;

    public string FirstNodeLabel => firstNodeLabel;
    public string SecondNodeLabel => secondNodeLabel;

    /*
    public string FirstNodeLabel
    {
        get
        {
            if (firstNode != null)
            {
                firstNodeLabel = firstNode.Label;
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
                secondNodeLabel = secondNode.Label;
            }
            return secondNodeLabel;
        }
        set
        {
            secondNodeLabel = value; // estas asignaciones con referencias intermedias pueden traer problemas (!!!)
        }
    }
    */

    [JsonIgnore]
    public override Type ControllerType => throw new NotImplementedException();

    [JsonIgnore]
    public float thikness = 5; // -> static (??)

    /// <summary>
    /// Empty constructor, necessary for serialization with json.
    /// </summary>
    public LBSEdgeData() { }

    public LBSEdgeData(LBSNodeData n1, LBSNodeData n2)
    {
        Debug.Log(n1 + ", " + n1.Label + ", " + n2 + "," + n2.Label);
        //this.firstNode = n1;
        this.firstNodeLabel = n1.Label;
        //this.secondNode = n2;
        this.secondNodeLabel = n2.Label;
    }


    public bool Contains(string nodeID)
    {
        return firstNodeLabel == nodeID || firstNodeLabel == nodeID;
        //return FirstNodeLabel == nodeID || SecondNodeLabel == nodeID;
    }
}
