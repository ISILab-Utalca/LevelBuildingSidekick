using LBS.Behaviours;
using LBS.Components;
using LBS.Components.Graph;
using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
[RequieredModule(typeof(LBSGraph), typeof (LBSGrammarGraph))]
public class QuestBehaviour : LBSBehaviour
{
    private LBSGraph graph => Owner.GetModule<LBSGraph>();
    private LBSGrammarGraph questGraph => Owner.GetModule<LBSGrammarGraph>();

    public List<LBSQuest> Quests => questGraph.Quests;

    LBSQuest selectedQuest;

    #region CONSTRUCTORS
    public QuestBehaviour(Texture2D icon, string name) : base(icon, name) { }


    #endregion

    public void AddNode(LBSNode n, QuestStep a)
    {
        graph.AddNode(n);
        questGraph.AddNode(n, a);
    }

    public void RemoveNode(NodeActionPair nodePair)
    {
        graph.RemoveNode(nodePair.Node);
        questGraph.RemovePair(nodePair);
    }

    public void RemoveNode(LBSNode node)
    {
        graph.RemoveNode(node);
        questGraph.RemovePair(node);
    }

    public override object Clone()
    {
        return new QuestBehaviour(this.Icon, this.Name);
    }

    public override void OnAttachLayer(LBSLayer layer)
    {
        Owner = layer;

        var graph = layer.GetModule<LBSGraph>();

        var nodes = GetNodes();

        graph.OnRemoveNode += (g, n) =>
        {
            var pair = nodes.FindAll(p => p.Node.Equals(n));
            pair.ForEach(pair => questGraph.RemovePair(pair));
        };
    }

    public override void OnDetachLayer(LBSLayer layer)
    {

    }

    public void AddConnection(LBSNode first, LBSNode second)
    {
        graph.AddEdge(first, second);
    }

    public void RemoveEdge(Vector2 pos, float dist)
    {
        graph.RemoveEdge(pos, dist);

    }

    public List<NodeActionPair> GetNodes()
    {
        return questGraph.Quests.SelectMany(q => q.QuestNodes).ToList();
    }

    public List<LBSEdge> GetEdges()
    {
        return graph.Edges;
    }

    public void AddQuest(LBSQuest quest)
    {
        questGraph.AddQuest(quest);
    }
}
