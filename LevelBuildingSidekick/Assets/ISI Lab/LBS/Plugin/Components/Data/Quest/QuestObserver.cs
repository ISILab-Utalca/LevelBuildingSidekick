using System;
using System.Collections.Generic;
using System.Linq;
using ISILab.LBS.Components;
using ISILab.LBS.Modules;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using System.Runtime.CompilerServices;
namespace ISILab.LBS
{
    [Serializable]
    public class QuestObserver : MonoBehaviour
    {
        [SerializeField][SerializeReference] private QuestGraph questGraph;
        [SerializeField] private Dictionary<QuestNode, QuestTrigger> _nodeTriggerMap = new();
        [SerializeField] private Dictionary<GraphNode, QuestBranch> _branchMap = new();
        [SerializeField] public UnityEvent onQuestCompleteEvent;

        public bool questComplete { get; private set; }
        public Dictionary<QuestNode, QuestTrigger> nodeTriggerMap => _nodeTriggerMap;
        public Dictionary<GraphNode, QuestBranch> branchMap => _branchMap;
        public event Action OnQuestAdvance;

        private void Awake() => StartQuest();

        private void OnDisable()
        {
            foreach (var trigger in _nodeTriggerMap.Values)
                trigger.gameObject.SetActive(false);
            foreach (var branch in _branchMap.Values)
                branch.gameObject.SetActive(false);
        }

        public void Init(QuestGraph graph) => questGraph = graph;

        private void StartQuest()
        {
            if (questGraph?.Root == null)
            {
                Debug.LogWarning("QuestGraph or Root is null.");
                return;
            }

            // --- Rebind all QuestTriggers to cloned nodes ---
            foreach (var trigger in GetComponentsInChildren<QuestTrigger>())
            {
                if (trigger.Node == null) continue;

                // Find the cloned node with the same ID
                var matchingGraphNode = questGraph.GraphNodes
                    .OfType<QuestNode>()
                    .FirstOrDefault(n => n.ID == trigger.Node.ID);

                // Assign runtime ref
                trigger.Node = matchingGraphNode;
            }

            // --- Initialize triggers ---
            foreach (var trigger in GetComponentsInChildren<QuestTrigger>())
            {
                trigger.Init();
                if (trigger.Node == null) continue;

                _nodeTriggerMap.TryAdd(trigger.Node, trigger);
                trigger.OnTriggerCompleted += OnTriggerCompleted;

                bool isRoot = trigger.Node == questGraph.Root;
                
                trigger.gameObject.SetActive(isRoot);
                trigger.Node.QuestState = isRoot ? QuestState.Active : QuestState.Blocked;
            }

            // --- Initialize branches ---
            foreach (var branch in GetComponentsInChildren<QuestBranch>())
            {
                _branchMap.TryAdd(branch.graphNode, branch);
                branch.gameObject.SetActive(false);
            }
        }


        private void OnTriggerCompleted(QuestTrigger trigger)
        {
            if (trigger == null) return;

            var outgoingEdges = questGraph.GetBranches(trigger.Node);

            // outgoing has the edges with the completed quest node
            // first try to see if the next node is part of a branch
            var branchingNodes = new HashSet<GraphNode>();
            foreach (var edge in outgoingEdges)
            {
                // if it is a branch node
                if (edge.To is not QuestNode)
                {
                    branchingNodes.Add(edge.To);
                }
            }
            
            foreach (var branchingNode in branchingNodes)
            {
                if (!_branchMap.TryGetValue(branchingNode, out var branch)) continue;
                
                // Activate the branch object
                branch.gameObject.SetActive(true);
                
                foreach (var qt in branch.ChildTriggers.Select(
                             childTrigger => childTrigger.GetComponent<QuestTrigger>())
                             .Where(qt => qt is not null))
                {
                    ActivateTrigger(qt);
                }
            }
            
            // Have we finished the quest?
            bool isLastNode = questGraph.GraphEdges.LastOrDefault()?.To == trigger.Node;
            if (isLastNode)
            {
                questComplete = true;
                onQuestCompleteEvent?.Invoke();
            }

            OnQuestAdvance?.Invoke();
        }

        private void ActivateTrigger(QuestTrigger trigger)
        {
            trigger.gameObject.SetActive(true);
            if (trigger.Node != null)
                trigger.Node.QuestState = QuestState.Active;
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