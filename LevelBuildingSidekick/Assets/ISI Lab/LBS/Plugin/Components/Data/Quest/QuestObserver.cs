using System;
using System.Collections.Generic;
using System.Linq;
using ISILab.LBS.Components;
using ISILab.LBS.Modules;
using UnityEngine;
using UnityEngine.Events;

namespace ISILab.LBS
{
    [System.Serializable]
    public class QuestObserver : MonoBehaviour
    {
        [SerializeField]
        private QuestGraph questGraph;

        [SerializeField, SerializeReference]
        private Dictionary<QuestNode, QuestTrigger> nodeTriggerMap = new Dictionary<QuestNode, QuestTrigger>();

        [SerializeField, SerializeReference]
        private List<QuestTrigger> activeTriggers = new List<QuestTrigger>();
        
        public UnityEvent OnQuestCompleteEvent;

        private bool questComplete = false;
        
        public bool QuestComplete => questComplete;

        private void OnEnable()
        {
            StartQuest();
        }

        private void OnDisable()
        {
            ClearTriggers();
        }

        /// <summary>
        /// Deactivates all current active triggers and unsubscribes from their events.
        /// </summary>
        private void ClearTriggers()
        {
            foreach (var trigger in activeTriggers)
            {
                trigger.OnTriggerCompleted -= HandleTriggerCompleted;
                trigger.gameObject.SetActive(false);
            }
            activeTriggers.Clear();
        }

        /// <summary>
        /// Handles the completion of a trigger and advances the quest.
        /// </summary>
        /// <param name="trigger">The trigger that was completed.</param>
        private void HandleTriggerCompleted(QuestTrigger trigger)
        {
            if (questComplete)
                return;

            Debug.Log("Triggered: " + trigger.name);
            ClearTriggers();
            AdvanceQuest(trigger);
        }

        /// <summary>
        /// Advances the quest to the next step based on the provided trigger.
        /// </summary>
        /// <param name="trigger">The trigger that completed the current step.</param>
        public void AdvanceQuest(QuestTrigger trigger)
        {
            // Find the current node associated with this trigger
            QuestNode currentNode = null;
            foreach (var pair in nodeTriggerMap)
            {
                if (pair.Value == trigger)
                {
                    currentNode = pair.Key;
                    break;
                }
            }

            if (currentNode == null)
            {
                Debug.LogWarning("No matching node found for trigger: " + trigger.name);
                return;
            }

            // Deactivate the current trigger
            if (nodeTriggerMap.ContainsKey(currentNode))
            {
                nodeTriggerMap[currentNode].gameObject.SetActive(false);
            }

            // Get the next nodes via edges
            var branches = questGraph.GetBranches(currentNode);

            if (branches.Count == 0)
            {
                questComplete = true;
                OnQuestCompleteEvent?.Invoke();
                gameObject.SetActive(false);
                return;
            }

            // Activate triggers for the next nodes
            foreach (var edge in branches)
            {
                var nextNode = edge.Second;
                if (nextNode != null && nodeTriggerMap.ContainsKey(nextNode))
                {
                    var newTrigger = nodeTriggerMap[nextNode];
                    newTrigger.gameObject.SetActive(true);
                    newTrigger.OnTriggerCompleted += HandleTriggerCompleted;
                    activeTriggers.Add(newTrigger);
                }
            }
        }

        /// <summary>
        /// Initializes the quest with a given graph and node-trigger mapping.
        /// </summary>
        /// <param name="graph">The quest graph defining flow and logic.</param>
        /// <param name="triggerMap">Dictionary mapping nodes to their triggers.</param>
        public void Init(QuestGraph graph, Dictionary<QuestNode, QuestTrigger> triggerMap)
        {
            questGraph = graph;
            nodeTriggerMap = triggerMap;
            StartQuest();
        }

        /// <summary>
        /// Starts the quest from the first node after the root in the graph.
        /// </summary>
        private void StartQuest()
        {
            if (questGraph == null || questGraph.Root == null)
            {
                Debug.LogWarning("QuestGraph or Root is null.");
                return;
            }

            var branches = questGraph.GetBranches(questGraph.Root);

            if (branches.Count == 0)
            {
                Debug.LogWarning("QuestGraph has no branches from root.");
                return;
            }

            var root = questGraph.QuestNodes.First();
            if (nodeTriggerMap.ContainsKey(root))
            {
                var trigger = nodeTriggerMap[root];
                trigger.gameObject.SetActive(true);
                trigger.OnTriggerCompleted += HandleTriggerCompleted;
            }
        }
    }

    /// <summary>
    /// Attribute to tag QuestTrigger classes with display labels and required data types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class QuestNodeActionTag : Attribute
    {
        public string Tag { get; }
        public Type[] RequiredDataTypes { get; }

        public QuestNodeActionTag(string tag, params Type[] requiredDataTypes)
        {
            Tag = tag.Trim().ToLowerInvariant();
            RequiredDataTypes = requiredDataTypes;
        }
    }

    /// <summary>
    /// Static registry that maps tag names to the expected data types of triggers.
    /// </summary>
    public static class QuestTagRegistry
    {
        public static readonly Dictionary<string, Type[]> TagDataTypes;

        static QuestTagRegistry()
        {
            TagDataTypes = new Dictionary<string, Type[]>();

            var taggedTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.GetCustomAttributes(typeof(QuestNodeActionTag), false).Length > 0);

            foreach (var type in taggedTypes)
            {
                var attributes = type.GetCustomAttributes(typeof(QuestNodeActionTag), false)
                    .Cast<QuestNodeActionTag>();

                foreach (var attr in attributes)
                {
                    TagDataTypes[attr.Tag] = attr.RequiredDataTypes;
                }
            }
        }
    }
}