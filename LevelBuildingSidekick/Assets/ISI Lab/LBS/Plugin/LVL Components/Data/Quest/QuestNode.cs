using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestNode : ICloneable
{

    #region FIELD
    [SerializeField, HideInInspector, JsonRequired]
    private int x, y;

    [SerializeField, JsonRequired]
    private string id = ""; // "ID" or "name"

    [SerializeField, JsonRequired]
    private string questAction = "";

    [SerializeField, JsonRequired]
    private bool grammarCheck;

    [SerializeField, JsonRequired]
    private bool mapCheck;
    #endregion

    #region PROPERTIES
    [JsonIgnore]
    public Vector2Int Position
    {
        get => new Vector2Int(x, y);

        set
        {
            x = value.x;
            y = value.y;
        }
    }

    [JsonIgnore]
    public string ID
    {
        get => id;
        set
        {
            id = value;
        }
    }


    [JsonIgnore]
    public string QuestAction
    {
        get => questAction;
        set
        {
            questAction = value;
        }
    }

    [JsonIgnore]
    public bool GrammarCheck
    {
        get => grammarCheck;
        set => grammarCheck = value;
    }

    [JsonIgnore]
    public bool MapCheck
    {
        get => mapCheck;
        set => mapCheck = value;
    }
    #endregion

    #region CONSTRUCTOR
    QuestNode() { }

    public QuestNode(string id, Vector2 position, string action) 
    {
        this.id = id;
        x = (int)position.x;
        y = (int)position.y;
        this.questAction = action;
    }
    #endregion

    public object Clone()
    {
        return new QuestNode(ID, Position, QuestAction);
    }
}
