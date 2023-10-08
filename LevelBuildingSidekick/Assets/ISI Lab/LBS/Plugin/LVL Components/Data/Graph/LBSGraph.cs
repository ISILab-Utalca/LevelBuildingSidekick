using LBS.Behaviours;
using LBS.Components;
using LBS.Components.Graph;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LBSGraph : LBSBehaviour
{

    [JsonRequired, SerializeReference]
    List<LBSNode> nodes = new List<LBSNode>();
    [JsonRequired, SerializeReference]
    List<LBSEdge> edges = new List<LBSEdge>();

    [JsonIgnore]
    public List<LBSNode> Nodes => new List<LBSNode>(nodes);
    [JsonIgnore]
    public List<LBSEdge> Edges => new List<LBSEdge>(edges);


    public LBSGraph(Texture2D icon, string name) : base(icon, name)
    {
    }

    public void AddNode(LBSNode node)
    {

    }

    public void RemoveNode(LBSNode node)
    {

    }


    public void AddEdge(LBSNode first, LBSNode second)
    {

    }

    public void RemoveEdge(LBSEdge edge)
    {

    }


    public override object Clone()
    {
        throw new System.NotImplementedException();
    }

    public override void OnAttachLayer(LBSLayer layer)
    {
        throw new System.NotImplementedException();
    }

    public override void OnDetachLayer(LBSLayer layer)
    {
        throw new System.NotImplementedException();
    }
}

[System.Serializable]
public class LBSEdge
{
    [JsonRequired]
    LBSNode first;
    [JsonRequired]
    LBSNode second;

    public LBSNode First
    {
        get => first; 
        set => first = value;
    }

    public LBSNode Second
    {
        get => second; 
        set => second = value;
    }

    LBSEdge(LBSNode first, LBSNode second) 
    {
        this.first = first;
        this.second = second;
    }
}
