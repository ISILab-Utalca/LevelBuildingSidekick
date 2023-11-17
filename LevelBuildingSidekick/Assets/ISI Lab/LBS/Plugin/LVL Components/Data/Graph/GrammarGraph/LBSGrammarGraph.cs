using LBS.Components;
using LBS.Components.Graph;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class LBSGrammarGraph : LBSModule
{
    List<LBSQuest> quests = new List<LBSQuest>();

    LBSQuest selectedQuest = null;

    public List<LBSQuest> Quests => new List<LBSQuest>(quests);

    public LBSGrammarGraph() : base() { ID = GetType().Name; }
    public LBSGrammarGraph(string key, List<LBSQuest> quests) : base(key)
    {
        this.quests = quests;
    }

    public override void Clear()
    {
        quests.Clear();
    }

    public override object Clone()
    {
        return new LBSGrammarGraph(id, quests.Select(n => n.Clone() as LBSQuest).ToList());
    }

    public override Rect GetBounds()
    {
        var nodes = quests.SelectMany(q => q.QuestNodes);

        var x = nodes.Min(n => n.Node.Position.x);
        var y = nodes.Min(n => n.Node.Position.y);
        var with = nodes.Max(n => n.Node.Position.x + n.Node.Width) - x;
        var height = nodes.Max(n => n.Node.Position.y + n.Node.Height) - y;

        return new Rect(x, y, with, height);
    }
    public QuestStep GetQuesStep(LBSQuest quest, LBSNode node)
    {
        return quest.QuestNodes.Find(x => x.Node == node)?.Action;
    }

    public QuestStep GetQuesStep(LBSNode node)
    {
        var nodes = quests.SelectMany(q => q.QuestNodes).ToList();

        return nodes.Find(x => x.Node == node)?.Action;
    }

    public void AddNode(LBSNode node, QuestStep action)
    {
        selectedQuest.AddNode(node, action);
    }

    public override bool IsEmpty()
    {
        return !quests.Any(q => q.IsEmpty() == false);
    }

    public override void OnAttach(LBSLayer layer)
    {
        //var graph = layer.GetModule<LBSGraph>();
        //Verificar posible recursividad
        //OnAddNode += graph.AddNode;
        //OnRemoveNode += graph.RemoveNode;
    }

    public void RemovePair(NodeActionPair pair)
    {
        selectedQuest.RemovePair(pair);
    }

    public void RemovePair(LBSNode node)
    {
        selectedQuest.RemovePair(node);
    }

    public override void OnDetach(LBSLayer layer)
    {
        //var graph = layer.GetModule<LBSGraph>();
        //Verificar posible recursividad
        //OnAddNode -= graph.AddNode;
        //OnRemoveNode -= graph.RemoveNode;
    }

    public override void Print()
    {
        throw new System.NotImplementedException();
    }

    public override void Rewrite(LBSModule module)
    {
        var other = module as LBSGrammarGraph;
        if (other == null)
        {
            throw new Exception("[ISI Lab] Modules have to be of the same type!");
        }
        Clear();
        foreach(var n in other.quests)
        {
            quests.Add(n);
        }

    }

    public override void Reload(LBSLayer layer)
    {
        OnAttach(layer);
    }

    internal void AddQuest(LBSQuest quest)
    {
        quests.Add(quest);
    }
}
