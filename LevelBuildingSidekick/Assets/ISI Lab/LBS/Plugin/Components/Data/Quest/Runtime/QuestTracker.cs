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
    public class QuestTracker : MonoBehaviour
    {
        [SerializeField][SerializeReference] 
        private QuestGraph questGraph;
        
        private List<QuestObjective> _objectives = new();
        
        [SerializeField] 
        public UnityEvent onQuestCompleteEvent;

        public bool questComplete { get; private set; }

        public List<QuestObjective> Objectives => _objectives;
        public event Action OnQuestAdvance;

        private void Awake() => StartQuest();

        private void OnDisable()
        {

        }

        public void Init(QuestGraph graph) => questGraph = graph;

        private void StartQuest()
        {
            if (questGraph?.Root == null)
            {
                Debug.LogWarning("QuestGraph or Root is null.");
                return;
            }

            // Rebind nodes editor to runtime generated classes
            foreach (var trigger in GetComponentsInChildren<QuestTrigger>())
            {
                if (trigger.Node == null) continue;

                // Find the cloned node with the same ID
                var matchingGraphNode = questGraph.GraphNodes
                    .OfType<QuestNode>()
                    .FirstOrDefault(n => n.ID == trigger.Node.ID);

                // Assign ref
                trigger.Node = matchingGraphNode;
            }

            // init triggers
            foreach (var trigger in GetComponentsInChildren<QuestTrigger>())
            {
                trigger.Init();
                if (trigger.Node == null) continue;

                trigger.OnTriggerCompleted += OnTriggerCompleted;
                trigger.Node.QuestState = QuestState.Blocked;
                trigger.gameObject.SetActive(false);
                
                _objectives.Add(new QuestObjective(trigger));
            }

            // init branches
            foreach (var branch in GetComponentsInChildren<QuestTriggerBranch>())
            {
                branch.gameObject.SetActive(false);
                
                foreach (var questObjective in _objectives)
                {
                    var destTrigger = branch.DestinationObject.GetComponent<QuestTrigger>();
                    if (destTrigger == questObjective.Trigger)
                    {
                        questObjective.SetSubobjectives(branch);
                    }
                }
            }
            
        }


        private void OnTriggerCompleted(QuestTrigger trigger)
        {
            if (trigger == null) return;

            foreach (var qObjectives in Objectives)
            {

                if (qObjectives.Trigger == trigger)
                {
                  //  qObjectives.
                }
            }
            
            var triggersToActivate = new HashSet<QuestTrigger>();
            var nextNodes = new HashSet<QuestNode>();
            foreach (var branch in questGraph.GetBranches(trigger.Node))
            {
                foreach (var questObjective in _objectives)
                {
                    if (questObjective.Trigger.Node == branch.To)
                    {
                        triggersToActivate.Add(questObjective.Trigger);
                    }

                    var ownerBranch = questObjective.Trigger.OwnerBranchNode;
                    if (ownerBranch is not null)
                    {
                        /*
                        _branchMap.TryGetValue(ownerBranch, out var questBranch);
                        if (questBranch is not null)
                        {
                            foreach (var triggerChild in questBranch.ChildTriggers)
                            {
                                if (triggerChild.GetComponent<QuestTrigger>())
                                {
                                    
                                }
                            }
                        }*/
                    }
                }
            }
            
            // outgoing has the edges with the completed quest node
            // first try to see if the next node is part of a branch
            var branchingNodes = new HashSet<GraphNode>();
            
            /*
            foreach (var branch in _branchMap.Values)
            {
                branchingNodes.Add(branch.graphNode);
            }
            
            foreach (var branchingNode in branchingNodes)
            {
                if (!_branchMap.TryGetValue(branchingNode, out var branch)) continue;
                foreach (var triggerToActivate in triggersToActivate)
                {
                    if (branch.ChildTriggers.Contains(triggerToActivate.gameObject))
                    {
                        // Activate the branch object
                        branch.gameObject.SetActive(true);
                        ActivateTrigger(triggerToActivate);
                    }
                }

                // Set quest visually as active
                ActivateTrigger(branch.DestinationObject.GetComponent<QuestTrigger>());
                // disable, only enable after the branch conditons are met
                trigger.gameObject.SetActive(false);
            }
            */
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