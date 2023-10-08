using LBS.Behaviours;
using LBS.Components;
using LBS.Components.Graph;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public LBSGraph(Texture2D icon, string name, List<LBSNode> nodes, List<LBSEdge> edges) : base(icon, name)
    {
        this.nodes = nodes;
        this.edges = edges;
    }

    public void AddNode(LBSNode node)
    {
        if (nodes.Contains(node))
            return;
        nodes.Add(node);
    }

    public void RemoveNode(LBSNode node)
    {
        if (!nodes.Contains(node))
            return;
        nodes.Remove(node);
        var toR = edges.Where(e => e.First.Equals(node) || e.Second.Equals(node));

        foreach(var e in toR)
        {
            edges.Remove(e);
        }
               
    }


    public void AddEdge(LBSNode first, LBSNode second)
    {
        if(!nodes.Contains(first) || !nodes.Contains(second))
            return;
        
        if (edges.Any(e => e.First.Equals(first) && e.Second.Equals(second) || e.First.Equals(second) || e.Second.Equals(first)))
            return;

        edges.Add(new LBSEdge(first, second));
    }

    public void RemoveEdge(LBSEdge edge)
    {
        if(!edges.Contains(edge))
            return; 
        edges.Remove(edge);
    }


    public override object Clone()
    {
        var nodes = this.nodes.Clone();
        var edges = this.edges.Clone();
        return new LBSGraph(this.Icon, this.Name, nodes, edges);
        

    }

    public override void OnAttachLayer(LBSLayer layer)
    {
    }

    public override void OnDetachLayer(LBSLayer layer)
    {
    }
}

[System.Serializable]
public class LBSEdge : ICloneable
{
    [JsonRequired, SerializeReference]
    LBSNode first;
    [JsonRequired, SerializeReference]
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

    public LBSEdge(LBSNode first, LBSNode second) 
    {
        this.first = first;
        this.second = second;
    }

    public object Clone()
    {
        return new LBSEdge(CloneRefs.Get(first) as LBSNode, CloneRefs.Get(second) as LBSNode);
    }
}
