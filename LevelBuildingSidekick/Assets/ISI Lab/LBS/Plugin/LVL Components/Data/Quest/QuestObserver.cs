using System;
using System.Collections;
using System.Collections.Generic;
using ISILab.LBS.Components;
using ISILab.LBS.Modules;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace ISILab.LBS
{
    [System.Serializable]
    public class QuestObserver : MonoBehaviour
    {
        [SerializeField, HideInInspector]
        private QuestGraph questGraph;

        [SerializeField, HideInInspector]
        private List<QuestStep> questTriggers;

        [SerializeField, SerializeReference, HideInInspector]
        private List<QuestTrigger> activeTriggers = new List<QuestTrigger>();

        public Action<QuestNode, QuestTrigger> OnQuestComplete;

        bool questComplete = false;

        public bool QuestComplete => questComplete;

        private Stack<QuestNode> fulfilledSteps = new Stack<QuestNode>();

        private void Start()
        {
        }

        private void Update()
        {
            if (!questComplete)
            {
                foreach (var trigger in activeTriggers)
                {
                    if (trigger.IsCompleted?.Invoke() == true)
                    {
                        Debug.Log("Triggered");
                        ClearTriggers();
                        AdvanceQuest(trigger);
                        break;
                    }
                }
            }
        }

        private void ClearTriggers()
        {
            foreach (var trigger in activeTriggers)
            {
                trigger.gameObject.SetActive(false);
            }

            activeTriggers.Clear();
        }

        public void AdvanceQuest(QuestTrigger trigger)
        {
            activeTriggers.Clear();

            var t = questTriggers.Find(t => t.Trigger.Equals(trigger));
            var node = t.Node;
            fulfilledSteps.Push(node);

            var branches = questGraph.GetBranches(node);

            if (branches.Count == 0)
            {
                questComplete = true;
                OnQuestComplete?.Invoke(node, trigger);
            }

            foreach (var edge in branches)
            {
                var newTrigger = questTriggers.Find(t => t.Node == edge.Second).Trigger;
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
            var trigger = questTriggers.Find(t => t.Node == step).Trigger;
            AdvanceQuest(trigger);
        }

        public void Init(QuestGraph graph, List<QuestStep> triggers)
        {
            questGraph = graph;
            questTriggers = triggers;
            StartQuest();
        }

        private void StartQuest()
        {
            var branches = questGraph.GetBranches(questGraph.Root);

            var firstNode = branches[0].Second;
            var trigger = questTriggers.Find(t => t.Node == firstNode).Trigger;

            trigger.gameObject.SetActive(true);
            activeTriggers.Add(trigger);
        }
    }

    [System.Serializable]
    public struct QuestStep
    {
        [SerializeField, SerializeReference, HideInInspector]
        QuestNode node;
        [SerializeField, SerializeReference, HideInInspector]
        QuestTrigger trigger;

        public QuestNode Node => node;
        public QuestTrigger Trigger => trigger;

        public QuestStep(QuestNode node, QuestTrigger trigger)
        {
            this.node = node;
            this.trigger = trigger;
        }
    }
}