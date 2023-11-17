using LBS.Components.Graph;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LBSQuest : ICloneable
{
    [SerializeField, JsonRequired]
    string name;

    [JsonIgnore]
    public string Name
    {
        get => name; 
        set => name = value;
    }

    [SerializeField]
    List<NodeActionPair> questNodes = new List<NodeActionPair>();

    public List<NodeActionPair> QuestNodes => questNodes;


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

    public bool IsEmpty()
    {
        return questNodes.Count == 0;
    }


    public void RemovePair(NodeActionPair pair)
    {
        questNodes.Remove(pair);
    }

    public void RemovePair(LBSNode node)
    {
        var n = questNodes.Find(q => q.Node.Equals(node));
        questNodes.Remove(n);
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

    public object Clone()
    {
        throw new NotImplementedException();
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

