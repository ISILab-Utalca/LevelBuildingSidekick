using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LBSQuestManager
{

    [SerializeField, JsonRequired, SerializeReference]
    private List<LBSQuestGraph> quests = new List<LBSQuestGraph>();

    [JsonIgnore]
    public LBSQuestGraph selectedQuest;

    [JsonIgnore]
    public Action OnQuestSelect;

    [JsonIgnore]
    public List<LBSQuestGraph> Quests => quests;


    public void AddQuest(LBSQuestGraph quest)
    {
        quests.Add(quest);
    }

    public LBSQuestGraph RemoveQuestAt(int index)
    {
        var q = quests[index];
        quests.RemoveAt(index);
        return q;
    }
}
