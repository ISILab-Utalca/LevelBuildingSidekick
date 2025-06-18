using System;
using System.Collections.Generic;
using System.Linq;
using ISILab.LBS.Components;
using ISILab.LBS.Modules;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace ISILab.LBS
{
    [Serializable]
    public class QuestObserver : MonoBehaviour
    {
        [SerializeField] private QuestGraph questGraph;

        [SerializeField, SerializeReference] private Dictionary<QuestNode, QuestTrigger> nodeTriggerMap = new();

        [SerializeField] public UnityEvent onQuestCompleteEvent;

        public bool QuestComplete { get; private set; }

        private void Start()
        {
            StartQuest();
        }

        private void OnEnable()
        {
            StartQuest();
        }

        private void OnDisable()
        {
            foreach (var trigger in nodeTriggerMap)
            {
                trigger.Value.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Handles the completion of a trigger and advances the quest.
        /// </summary>
        /// <param name="trigger">The trigger that was completed.</param>
        private void HandleTriggerCompleted(QuestTrigger trigger)
        {
            if (!AdvanceQuest(trigger) || QuestComplete) return;
            QuestComplete = true;
            onQuestCompleteEvent.Invoke();
            
        }

        /// <summary>
        /// Advances the quest to the next step based on the provided trigger.
        /// Returns true if the quest can't be advanced anymore i.e. its completed.
        /// </summary>
        /// <param name="trigger">The trigger that completed the current step.</param>
        public bool AdvanceQuest(QuestTrigger trigger)
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
                return false; // quest cant be completed
            }

            // Deactivate the current trigger
            if (nodeTriggerMap.ContainsKey(currentNode))
            {
                nodeTriggerMap[currentNode].gameObject.SetActive(false);
            }

            // Get the next nodes via edges
            var edges = questGraph.QuestEdges;
 
            // the quest has been completed!
            if (edges.Last().Second == currentNode) return true;
            
            // find the next node and correspondent trigger
            foreach (var edge in edges)
            {
                if(edge.First != currentNode) continue;
                
                // Activate next trigger
                var nextNode = edge.Second;
                var newTrigger = nodeTriggerMap[nextNode];
                newTrigger.gameObject.SetActive(true);
                newTrigger.OnTriggerCompleted += HandleTriggerCompleted;
            }
            
            return false; // quest in progress
        }

        /// <summary>
        /// Initializes the quest with a given graph and node-trigger mapping.
        /// </summary>
        /// <param name="graph">The quest graph defining flow and logic.</param>
        public void Init(QuestGraph graph)
        {
            questGraph = graph;
            StartQuest();
        }

        /// <summary>
        /// Starts the quest from the first node after the root in the graph.
        /// </summary>
        private void StartQuest()
        {
            if (questGraph?.Root == null)
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

            List<QuestTrigger> childTriggers = (from Transform child in transform select child.GetComponent<QuestTrigger>()).ToList();


            // subscribe all triggers to call advance quest when completed
            foreach (var node in questGraph.QuestNodes)
            {
                foreach (var child in childTriggers)
                {
                    if(!child) continue;
                    if (node.ID != child.NodeID) continue;
                    if(!nodeTriggerMap.TryAdd(node, child)) continue;
                    
                    child.OnTriggerCompleted += HandleTriggerCompleted;
                    if(node == questGraph.Root) child.gameObject.SetActive(true); // activate the first trigger only
                }
                
            }
        }
    }

    /// <summary>
    /// Attribute to tag QuestTrigger by tag.
    /// The string must be in lower case and without ony empty spaces.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class QuestNodeActionTag : Attribute
    {
        public string Tag { get; }
        public QuestNodeActionTag(string tag)
        {
            Tag = tag;
        }
    }


    /// <summary>
    /// Static registry that maps tag names to the expected data types of triggers.
    /// </summary>
  
    [InitializeOnLoad]
    public static class QuestTagRegistry
    {
        private static readonly Dictionary<string, Type> TagDataTypes;

        static QuestTagRegistry()
        {
            TagDataTypes = new Dictionary<string, Type>();

            var allTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a =>
                {
                    try { return a.GetTypes(); }
                    catch { return Array.Empty<Type>(); }
                })
                .Where(t =>
                    typeof(QuestTrigger).IsAssignableFrom(t) &&
                    !t.IsAbstract &&
                    t.GetCustomAttributes(typeof(QuestNodeActionTag), false).Length > 0
                );

            foreach (var type in allTypes)
            {
                var attributes = type.GetCustomAttributes(typeof(QuestNodeActionTag), false)
                    .Cast<QuestNodeActionTag>();

                foreach (var attr in attributes)
                {
                    var tag = attr.Tag.Trim().ToLowerInvariant();

                    if (!TagDataTypes.ContainsKey(tag))
                    {
                        TagDataTypes.Add(tag, type);
                    }
                    else
                    {
                        Debug.LogWarning($"[QuestTagRegistry] Duplicate tag '{tag}' found on {type.Name} and {TagDataTypes[tag].Name}.");
                    }
                }
            }
        }

        public static Type GetTriggerTypeForTag(string tag)
        {
            var trimmed = tag.Trim().ToLowerInvariant();
            return TagDataTypes.GetValueOrDefault(trimmed);
        }
    }
}