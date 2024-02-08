using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[System.Serializable]
public class QuestObserver : MonoBehaviour
{
    [SerializeField, HideInInspector]
    QuestGraph questGraph;

    [SerializeField, SerializeReference]
    List<Tuple<QuestNode, QuestTrigger>> questTriggers;

    private List<QuestTrigger> activeTriggers = new List<QuestTrigger>();

    public Action<QuestNode, QuestTrigger> OnQuestComplete;

    bool questComplete = false;

    public bool QuestComplete => questComplete;

    private Stack<QuestNode> fulfilledSteps = new Stack<QuestNode>();

    private void Update()
    {
        if(!questComplete)
        {
            foreach (var trigger in activeTriggers)
            {
                if (trigger.IsCompleted?.Invoke() == true)
                {
                    ClearTriggers();
                    AdvanceQuest(trigger);
                    break;
                }
            }
        }
    }

    private void ClearTriggers()
    {
        foreach(var trigger in activeTriggers)
        {
            trigger.gameObject.SetActive(false);
        }

        activeTriggers.Clear();
    }

    public void AdvanceQuest(QuestTrigger trigger)
    {
        activeTriggers.Clear();

        var node = questTriggers.Find(t => t.Item2.Equals(trigger)).Item1;
        fulfilledSteps.Push(node);

        var branches = questGraph.GetBranches(node);

        if (branches.Count == 0)
        {
            questComplete = true;
            OnQuestComplete?.Invoke(node, trigger);
        }

        foreach (var edge in branches)
        {
            var newTrigger = questTriggers.Find(t => t.Item1 == edge.Second).Item2;
            newTrigger.gameObject.SetActive(true);
            activeTriggers.Add(newTrigger);
        }
    }

    public void RegressQuest()
    {
        if (fulfilledSteps.Count == 0)
            return;

        fulfilledSteps.Pop();

        ClearTriggers();

        if (fulfilledSteps.Count == 0)
        {
            StartQuest();
            return;
        }

        var step = fulfilledSteps.Peek();
        var trigger = questTriggers.Find(t => t.Item1 == step).Item2;
        AdvanceQuest(trigger);
    }

    public void Init(QuestGraph graph, List<Tuple<QuestNode, QuestTrigger>> triggers)
    {
        questGraph = graph;
        questTriggers = triggers;

        StartQuest();
    }

    private void StartQuest()
    {
        var branches = questGraph.GetBranches(questGraph.Root);

        var firstNode = branches[0].Second;
        var trigger = questTriggers.Find(t => t.Item1 == firstNode).Item2;

        trigger.gameObject.SetActive(true);
        activeTriggers.Add(trigger);
    }
}
