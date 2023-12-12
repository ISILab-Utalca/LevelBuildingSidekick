using LBS.Components.Graph;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LBSQuestGraph : ICloneable
{

    [SerializeField, JsonRequired]
    string grammarName;

    [SerializeField, JsonRequired]
    string name;

    [JsonIgnore]
    public string Name
    {
        get => name; 
        set => name = value;
    }

    [SerializeField]
    List<QuestNode> questNodes = new List<QuestNode>();



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
    public bool IsVisible { get; set; }


    public LBSQuestGraph()
    {
        IsVisible = true;
    }

    public QuestNode GetQuesNode(Vector2 position)
    {
        return questNodes.Find(x => x.Position == position);
    }

    public void AddNode(string id, Vector2 position, string action)
    {
        var data = new QuestNode(id, position, action);
        questNodes.Add(data);
    }

    public void AddNode(QuestNode node)
    {
        questNodes.Add(node);
    }

    public bool IsEmpty()
    {
        return questNodes.Count == 0;
    }


    public void RemoveQuestNode(QuestNode node)
    {
        questNodes.Remove(node);
    }

    public object Clone()
    {
        throw new NotImplementedException();
    }

    public void AddConnection(QuestNode first, QuestNode second)
    {
        throw new NotImplementedException();
    }

    public void RemoveEdge(Vector2Int endPosition, int v)
    {
        throw new NotImplementedException();
    }
}

