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
    [SerializeField]
    List<NodeActionPair> questNodes = new List<NodeActionPair>();

    public List<NodeActionPair> QuestNodes => questNodes;

    public LBSGrammarGraph() : base() { ID = GetType().Name; }
    public LBSGrammarGraph(string key, List<NodeActionPair> nodes) : base(key)
    {
        this.questNodes = nodes;
    }

    public override void Clear()
    {
        questNodes.Clear();
    }

    public override object Clone()
    {
        return new LBSGrammarGraph(id, questNodes.Select(n => n.Clone() as NodeActionPair).ToList());
    }

    public override Rect GetBounds()
    {
        var x = questNodes.Min(n => n.Node.Position.x);
        var y = questNodes.Min(n => n.Node.Position.y);
        var with = questNodes.Max(n => n.Node.Position.x + n.Node.Width) - x;
        var height = questNodes.Max(n => n.Node.Position.y + n.Node.Height) - y;

        return new Rect(x, y, with, height);
    }

    public QuestStep GetQuesStep(LBSNode node)
    {
        return questNodes.Find(x => x.Node == node)?.Action;
    }

    public void AddNode(LBSNode node, QuestStep action)
    {
        var t = questNodes.Find(p => p.Node.Equals(node));

        if (t == null)
        {
            var data = new QuestStep(action.GrammarElement);
            questNodes.Add(new NodeActionPair(node, data));
        }
        else
        {
            t.Action = new QuestStep(action.GrammarElement);
        }
    }

    public override bool IsEmpty()
    {
        return questNodes.Count == 0;
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
        questNodes.Remove(pair);
    }

    private void RemoveNode(object obj)
    {
        var toR = obj as LBSNode;
        var xx = questNodes.Find(x => x.Node == toR);
        questNodes.Remove(xx);
    }

    private void AddEmpty(object obj)
    {
        var t = obj as LBSNode;
        var xx = questNodes.Find(x => x.Node == t);
        if (xx != null)
        {
            RemovePair(xx);
        }
        questNodes.Add(new NodeActionPair(t, new QuestStep()));
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
        foreach(var n in other.QuestNodes)
        {
            questNodes.Add(n);
        }

    }

    public override void Reload(LBSLayer layer)
    {
        OnAttach(layer);
    }
}

[System.Serializable]
public class NodeActionPair : ICloneable
{
    [SerializeField, SerializeReference, JsonRequired]
    LBSNode node;
    [SerializeField, SerializeReference, JsonRequired]
    QuestStep questStep;

    public LBSNode Node => node;
    public QuestStep Action
    {
        get => questStep;
        set => questStep = value;
    }

    public NodeActionPair(LBSNode node, QuestStep action)
    {
        this.node = node;
        this.questStep = action;
    }

    public object Clone()
    {
        return new NodeActionPair(CloneRefs.Get(node) as LBSNode, CloneRefs.Get(questStep) as QuestStep);
    }
}

