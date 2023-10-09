using LBS.Behaviours;
using LBS.Components;
using LBS.Components.Graph;
using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[RequieredModule(typeof(LBSGraph), typeof (LBSGrammarGraph))]
public class QuestBehaviour : LBSBehaviour
{
    private LBSGraph graph => Owner.GetModule<LBSGraph>();
    private LBSGrammarGraph graphPair => Owner.GetModule<LBSGrammarGraph>();

    #region CONSTRUCTORS
    public QuestBehaviour(Texture2D icon, string name) : base(icon, name) { }


    #endregion

    public void AddNode(LBSNode n, QuestStep a)
    {
        graph.AddNode(n);
        graphPair.AddNode(n, a);
    }

    public void RemoveNode(NodeActionPair nodePair)
    {
        graph.RemoveNode(nodePair.Node);
        graphPair.RemovePair(nodePair);
    }

    public override object Clone()
    {
        return new QuestBehaviour(this.Icon, this.Name);
    }

    public override void OnAttachLayer(LBSLayer layer)
    {
        Owner = layer;

        var graph = layer.GetModule<LBSGraph>();

        graph.OnRemoveNode += (g, n) =>
        {
            var pair = graphPair.QuestNodes.FindAll(p => p.Node.Equals(n));
            pair.ForEach(pair => graphPair.RemovePair(pair));
        };
    }

    public override void OnDetachLayer(LBSLayer layer)
    {

    }

    public void AddConnection(NodeActionPair first, NodeActionPair second)
    {
        graph.AddEdge(first.Node, second.Node);
    }

    public void RemoveEdge(Vector2 pos, float dist)
    {
        graph.RemoveEdge(pos, dist);

    }

    public List<NodeActionPair> GetNodes()
    {
        return graphPair.QuestNodes;
    }

    public List<LBSEdge> GetEdges()
    {
        return graph.Edges;
    }   

}
