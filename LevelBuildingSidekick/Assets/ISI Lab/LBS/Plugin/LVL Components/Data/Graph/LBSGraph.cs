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
public class LBSGraph : LBSModule
{

    [JsonRequired, SerializeReference]
    List<LBSNode> nodes = new List<LBSNode>();
    [JsonRequired, SerializeReference]
    List<LBSEdge> edges = new List<LBSEdge>();

    [JsonIgnore]
    public List<LBSNode> Nodes => new List<LBSNode>(nodes);
    [JsonIgnore]
    public List<LBSEdge> Edges => new List<LBSEdge>(edges);


    public event Action<LBSGraph, LBSNode> OnRemoveNode;

    public LBSGraph() 
    {
    }

    public LBSGraph(List<LBSNode> nodes, List<LBSEdge> edges) 
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

        OnRemoveNode?.Invoke(this, node);
        nodes.Remove(node);
        var toR = edges.Where(e => e.First.Equals(node) || e.Second.Equals(node)).ToList();

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

    public LBSEdge RemoveEdge(Vector2 position, float delta)
    {
        foreach (var e in edges)
        {
            var dist = position.DistanceToLine(e.First.Position, e.Second.Position);
            //Debug.Log(e.First.Position + " - " + e.Second.Position + " - " + position);
            //Debug.Log(dist + " / " + delta);
            if (dist < delta)
            {
                edges.Remove(e);
                return e;
            }
            
        }
        return null;
    }

    public override object Clone()
    {
        var nodes = this.nodes.Clone();
        var edges = this.edges.Clone();
        return new LBSGraph(nodes, edges);
    }

    public override void Print()
    {
    }

    public override void Clear()
    {
        nodes.Clear();
        edges.Clear();
    }

    public override bool IsEmpty()
    {
        return nodes.Count == 0;
    }

    public override Rect GetBounds()
    {
        throw new NotImplementedException();
    }

    public override void Rewrite(LBSModule other)
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
