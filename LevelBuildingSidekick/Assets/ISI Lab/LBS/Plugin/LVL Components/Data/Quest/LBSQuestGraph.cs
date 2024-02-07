using ISILab.Extensions;
using LBS.Components;
using LBS.Components.Graph;
using LBS.Settings;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class LBSQuestGraph : LBSModule, ICloneable
{

    [SerializeField, JsonRequired]
    string grammarName;

    [SerializeField, SerializeReference, JsonRequired]
    List<QuestNode> questNodes = new List<QuestNode>();

    [SerializeField, SerializeReference, JsonRequired]
    List<QuestEdge> questEdges = new List<QuestEdge>();

    [SerializeField, SerializeReference, JsonRequired]
    QuestNode root;


    [JsonIgnore]
    public QuestNode Root { get => root; }

    [JsonIgnore]
    private LBSGrammar grammar;

    [JsonIgnore]
    public LBSGrammar Grammar
    {
        get
        {
            if(grammar != null && grammarName != null && grammar.name == grammarName)
                return grammar;
            else if(grammarName != null)
            {
                grammar = LBSAssetsStorage.Instance.Get<LBSGrammar>().Find(g => g.name == grammarName);
                return grammar;
            }
            return null;
        }
        set
        {
            grammar = value;
            grammarName = value.name;
        }
    }

    [JsonIgnore]
    public List<QuestNode> QuestNodes => questNodes;

    [JsonIgnore]
    public List<QuestEdge> QuestEdges => questEdges;

    [JsonIgnore]
    public bool IsVisible { get; set; }


    #region EVENTS
    [JsonIgnore]
    public Action<QuestNode> OnAddNode;
    [JsonIgnore]
    public Action<QuestNode> OnRemoveNode;
    [JsonIgnore]
    public Action<QuestEdge> OnAddEdge;
    [JsonIgnore]
    public Action<QuestEdge> OnRemoveEdge;
    #endregion

    public LBSQuestGraph()
    {
        IsVisible = true;
        root = new QuestNode("Start Node", Vector2.zero, "Start Node");
        questNodes.Add(root);
    }

    public QuestNode GetQuesNode(Vector2 position)
    {
        var size = Owner.TileSize * LBSSettings.Instance.general.TileSize;

        return questNodes.Find(x => (new Rect(x.Position, size)).Contains(position));
    }

    public void AddNode(string id, Vector2 position, string action)
    {
        var data = new QuestNode(id, position, action);
        questNodes.Add(data);
        OnAddNode?.Invoke(data);
    }

    public void AddNode(QuestNode node)
    {
        questNodes.Add(node);
        OnAddNode?.Invoke(node);
    }

    public void RemoveQuestNode(QuestNode node)
    {
        questNodes.Remove(node);

        var edges = questEdges.Where(e => e.First.Equals(node) || e.Second.Equals(node)).ToList();

        for(int i = 0; i < edges.Count; i++) 
        {
            questEdges.Remove(edges[i]);
        }
        OnRemoveNode?.Invoke(node);
    }

    public void AddConnection(QuestNode first, QuestNode second)
    {

        if (first == null || second == null)
        {
            Debug.LogWarning("NullNode");
            Debug.Log("First " + first);
            Debug.Log("Second " + second);
            return;
        }

        if (second.Equals(root))
        {
            Debug.LogWarning("The start node can not be the second element of a connection");
            return;
        }

        if (first.Equals(root))
        {
            if (questEdges.Any(e => e.First.Equals(root)))
            {
                Debug.LogWarning("The start node is already connected");
                return;
            }
        }

        if (questEdges.Any(e => (e.First.Equals(first) && e.Second.Equals(second)) || (e.First.Equals(second) && e.Second.Equals(first))))
        {
            Debug.LogWarning("The connection already exists");
            return;
        }

        var edge = new QuestEdge(first, second);

        if (Looped(edge))
        {
            Debug.LogWarning("Invalid connection, loop detected");
            return;
        }

        questEdges.Add(edge);
        
        OnAddEdge?.Invoke(edge);
    }

    private bool Looped(QuestEdge edge)
    {

        var list = new List<QuestNode>();
        list.Add(edge.Second);

        while (list.Count > 0) 
        {
            if(list.Contains(edge.First))
                return true;

            var candidates = new List<QuestNode>();
            foreach (var node in list) 
            {
                var edges = GetBranches(node);
                if (edges.Count == 0)
                    continue;
                candidates.AddRange(edges.Select(e => e.Second));
            }

            list = candidates;
        }

        return false;
    }

    public List<QuestEdge> GetBranches(QuestNode node)
    {
        if(questEdges.Count == 0)
            return new List<QuestEdge>();
        return questEdges.Where(e => e.First == node).ToList();
    }

    public List<QuestEdge> GetRoots(QuestNode node)
    {
        if (questEdges.Count == 0)
            return new List<QuestEdge>();
        return questEdges.Where(e => e.Second == node).ToList();
    }

    public void RemoveEdge(Vector2Int position, float delta)
    {
        QuestEdge edge = GetEdge(position, delta);
        questEdges.Remove(edge);
        OnAddEdge?.Invoke(edge);
    }

    private QuestEdge GetEdge(Vector2 position, float delta)
    {
        var size = Owner.TileSize * LBSSettings.Instance.general.TileSize;
        foreach (var e in questEdges)
        {
            var c1 = new Rect(e.First.Position, size).center;
            var c2 = new Rect(e.Second.Position, size).center;

            var dist = position.DistanceToLine(c1, c2);
            if (dist < delta)
                return e;
        }
        return null;
    }

    public override void Print()
    {
        throw new NotImplementedException();
    }

    public override void Clear()
    {
        throw new NotImplementedException();
    }

    public override Rect GetBounds()
    {
        throw new NotImplementedException();
    }

    public override void Rewrite(LBSModule other)
    {
        throw new NotImplementedException();
    }

    public override bool IsEmpty()
    {
        return questNodes.Count == 0;
    }

    public override object Clone()
    {
        var clone = new LBSQuestGraph();

        var nodes = questNodes.Select(n => n.Clone()).Cast<QuestNode>();

        foreach (var node in nodes)
        {
            clone.questNodes.Add(node);
        }

        var edgesClone = questEdges.Select(e => e.Clone()).Cast<QuestEdge>();
        foreach (var edge in edgesClone)
        {
            clone.QuestEdges.Add(edge);
        }



        return clone;
    }
}

[System.Serializable]
public class QuestEdge : ICloneable
{
    [SerializeField, SerializeReference, JsonRequired]
    QuestNode first;
    [SerializeField, SerializeReference, JsonRequired]
    QuestNode second;

    [JsonIgnore]
    public QuestNode First
    {
        get => first; 
        set => first = value;
    }

    [JsonIgnore]
    public QuestNode Second
    {
        get => second;
        set => second = value;
    }

    public QuestEdge()
    {

    }

    public QuestEdge(QuestNode first, QuestNode second)
    {
        this.first = first;
        this.second = second;
    }

    public object Clone()
    {
        return new QuestEdge(CloneRefs.Get(first) as QuestNode, CloneRefs.Get(second) as QuestNode);
    }
}

