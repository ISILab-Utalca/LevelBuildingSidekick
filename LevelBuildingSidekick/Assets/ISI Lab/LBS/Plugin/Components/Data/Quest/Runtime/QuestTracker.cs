using System;
using System.Collections.Generic;
using System.Linq;
using ISILab.LBS.Components;
using ISILab.LBS.Modules;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using System.Runtime.CompilerServices;
using UnityEditor.Graphs;
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
            foreach (var branch in GetComponentsInChildren<QuestTriggerBranch>())
            {
                if (branch.BranchNode is null) continue;

                var matchingGraphNode = questGraph.GraphNodes.FirstOrDefault(n => n.ID == branch.BranchNode.ID);

                if (matchingGraphNode is QuestNode) continue;

                // Assign ref
                branch.SetNode(matchingGraphNode);

            }
            foreach (var trigger in GetComponentsInChildren<QuestTrigger>())
            {
                if (trigger.Node == null) continue;

                // Find the cloned node with the same ID
                var matchingGraphNode = questGraph.GraphNodes
                    .OfType<QuestNode>()
                    .FirstOrDefault(n => n.ID == trigger.Node.ID);

                // Assign ref
                trigger.Node = matchingGraphNode;

                foreach (var edge in questGraph.GetBranches(matchingGraphNode))
                {
                    trigger.Destinations.Add(edge.To);
                }
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
                foreach(QuestEdge edge in questGraph.GetBranches(branch.BranchNode))
                {
                    if (edge.To is null) continue;
                    branch.SetDestinationTrigger(GetTrigger(edge.To).gameObject);
                }
               
            }

            foreach (var objective in _objectives)
            {
                if (objective.Trigger.Node == questGraph.Root)
                {
                    ActivateTrigger(objective.Trigger);
                }
            }
            
        }


        QuestTrigger GetTrigger(GraphNode node)
        {
            foreach (var objective in _objectives)
            {
                if(objective.Trigger is null) continue;
                if(objective.Trigger.Node == node) return objective.Trigger;
            }    
            return null;
        }
        
        private void OnTriggerCompleted(QuestTrigger trigger)
        {
            if (trigger == null) return;

            // have we finished the quest?
            foreach (var objective in Objectives)
            {
                if(objective.Trigger is null) continue;
                if(objective.Trigger.Node != trigger.Node) continue;
                if (trigger.Node.NodeType != QuestNode.ENodeType.Goal) continue;
                
                questComplete = true;
                onQuestCompleteEvent?.Invoke();
                return;
            }
            
            // continue progressing
            foreach (var qObjectives in Objectives)
            {
                // branch progress
                foreach (var branch in qObjectives.GetBranches())
                {
                    // if there is branch with the trigger as sub
                    if (branch.ChildObjects.Contains(trigger.gameObject))
                    {
                        if (branch.IsComplete())
                        {
                            branch.OnProgress();
                            
                            ActivateTrigger(branch.DestinationObject.GetComponent<QuestTrigger>());
                        }
                    }
                }
                
                
                // branchless progress
                if (qObjectives.Trigger == trigger)
                {
                    foreach (var destination in trigger.Destinations)
                    {
                        ActivateTrigger(GetTrigger(destination));
                    }
                    
                }
            }

            OnQuestAdvance?.Invoke();
        }

        private void ActivateTrigger(QuestTrigger trigger)
        {
            if (trigger is null) return;
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