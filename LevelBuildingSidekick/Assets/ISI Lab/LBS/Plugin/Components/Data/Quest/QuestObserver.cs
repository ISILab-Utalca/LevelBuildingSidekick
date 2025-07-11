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
        #region FIELDS
        
        [SerializeField] 
        private QuestGraph questGraph;
        [SerializeField, SerializeReference] 
        private Dictionary<QuestNode, QuestTrigger> nodeTriggerMap = new();
        [SerializeField] 
        public UnityEvent onQuestCompleteEvent;
        
        #endregion

        #region PROPERTIES

        public bool QuestComplete { get; private set; }
        public Dictionary<QuestNode, QuestTrigger> NodeTriggerMap => nodeTriggerMap;
        public event Action OnQuestAdvance;
        #endregion


        private void Awake()
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
            // get the current node
            var currentNode = nodeTriggerMap.FirstOrDefault(pair => pair.Value == trigger).Key;
            if (currentNode == null) return false;
            
            // If the current node is the last one, the quest is complete
            if (questGraph.QuestEdges.LastOrDefault()?.Second == currentNode)
            {
                OnQuestAdvance?.Invoke();
                return true;
            }


            // Activate the next node and trigger
            foreach (var edge in questGraph.QuestEdges)
            {
                if (edge.First != currentNode) continue;

                var nextNode = edge.Second;
                nextNode.QuestState = QuestState.Active;

                if (!nodeTriggerMap.TryGetValue(nextNode, out var nextTrigger)) continue;
                
                nextTrigger.gameObject.SetActive(true);
                nextTrigger.OnTriggerCompleted += HandleTriggerCompleted;
            }
            
            OnQuestAdvance?.Invoke();
            return false;
        }



        /// <summary>
        /// Initializes the quest with a given graph and node-trigger mapping.
        /// </summary>
        /// <param name="graph">The quest graph defining flow and logic.</param>
        public void Init(QuestGraph graph)
        {
            questGraph = graph;
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
            foreach (var child in childTriggers)
            {
                
                if(child is null) continue;
                child.Init();
                if(child.Node is null)  continue;
                
                foreach (var questNode in questGraph.QuestNodes)
                {
                    if (child.NodeID != questNode.ID) continue;
                    child.Node = questNode;
                }
                
                var node = child.Node;
                
                // activate the first trigger only
                if(node.ID == questGraph.Root.ID) 
                {
                    child.gameObject.SetActive(true); 
                    node.QuestState =  QuestState.Active;
                }
                else
                {
                    child.gameObject.SetActive(false); 
                    node.QuestState = QuestState.Blocked;
                }
                
                nodeTriggerMap.TryAdd(node, child);
                child.OnTriggerCompleted += HandleTriggerCompleted;
                
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