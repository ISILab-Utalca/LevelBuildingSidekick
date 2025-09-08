using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using ISILab.LBS.Components;
using ISILab.LBS.Modules;

namespace ISILab.LBS
{
    [Serializable]
    public class QuestTracker : MonoBehaviour
    {
        #region FIELDS
        /// <summary>
        /// Graph assigned in the QuestRuleGeneration script - assigns the LBS Quest Layer Graph
        /// </summary>
        [SerializeField, SerializeReference]
        private QuestGraph questGraph; 

        /// <summary>
        /// Custom event to add logic, for the completion of the quest
        /// </summary>
        [SerializeField]
        private UnityEvent onQuestCompleteEvent; 

        private readonly List<QuestObjective> _objectives = new();
        #endregion

        #region PROPERTIES
        /// <summary>
        /// Objectives(tracking the progress and node advancement)
        /// </summary>
        public IReadOnlyList<QuestObjective> Objectives => _objectives;
        public bool QuestComplete { get; private set; } 
        
        /// <summary>
        /// Effect called whenever a trigger(actionNode) is completed. Updates UI
        /// </summary>
        public event Action OnQuestAdvance; 
        #endregion


        #region METHODS
        private void Awake()
        {
            StartQuest();
        }

        public void Init(QuestGraph graph)
        {
            if (graph == null)
            {
                Debug.LogWarning("Cannot initialize with null QuestGraph.");
                return;
            }
            questGraph = graph;
        }

        private void StartQuest()
        {
            if (questGraph?.Root == null)
            {
                Debug.LogWarning("QuestGraph or Root is null.");
                return;
            }

            InitializeTriggers();
            InitializeBranches();
            ActivateRootTrigger();
        }

        private void InitializeTriggers()
        {
            // Iterates through all QuestTrigger components in children
            foreach (var trigger in GetComponentsInChildren<QuestTrigger>())
            {
                if (trigger.Node == null) continue;

                // Match the questGraph data to the triggers generated from LBS.
                var matchingNode = questGraph.GraphNodes
                    .OfType<QuestNode>()
                    .FirstOrDefault(n => n.ID == trigger.Node.ID);

                if (matchingNode == null) continue;

                trigger.Node = matchingNode;
                
                // Add all destination nodes from graph edges
                trigger.Destinations.AddRange(questGraph.GetBranches(matchingNode).Select(edge => edge.To));
                trigger.Init();
                trigger.OnTriggerCompleted += OnTriggerCompleted;
                trigger.Node.QuestState = QuestState.Blocked;
                trigger.gameObject.SetActive(false);

                _objectives.Add(new QuestObjective(trigger));
            }
        }

        private void InitializeBranches()
        {
            foreach (var branch in GetComponentsInChildren<QuestTriggerBranch>())
            {
                if (branch.BranchNode == null) continue;
                
                var matchingNode = questGraph.GraphNodes
                    .FirstOrDefault(n => n.ID == branch.BranchNode.ID);

                if (matchingNode is QuestNode) continue;

                branch.SetNode(matchingNode);
                branch.gameObject.SetActive(false);

                // Link branches to its root objectives(action nodes).
                foreach (var objective in _objectives)
                {
                    var destTrigger = branch.DestinationObject?.GetComponent<QuestTrigger>();
                    if (destTrigger == objective.Trigger)
                    {
                        objective.SetSubobjectives(branch);
                    }
                }

                // Set destination triggers for each branch edges
                foreach (var edge in questGraph.GetBranches(branch.BranchNode))
                {
                    if (edge.To != null)
                    {
                        branch.SetDestinationTrigger(GetTrigger(edge.To)?.gameObject);
                    }
                }
            }
        }

        private void ActivateRootTrigger()
        {
            var rootObjective = _objectives.FirstOrDefault(o => o.Trigger.Node == questGraph.Root);
            
            if (rootObjective == null) return;
            
            ActivateTrigger(rootObjective.Trigger);
        }

        private QuestTrigger GetTrigger(GraphNode node)
        {
            // Finds a trigger associated with a specific graph node (branch or questnode)
            return _objectives.FirstOrDefault(o => o.Trigger?.Node == node)?.Trigger;
        }

        private void OnTriggerCompleted(QuestTrigger trigger)
        {
            if (trigger == null) return;

            // have we reached a goal node?
            if (trigger.Node.NodeType == QuestNode.ENodeType.Goal)
            {
                CompleteQuest();
                return;
            }

            ProcessTriggerProgress(trigger);
        }

        private void CompleteQuest()
        {
            // failed all other active nodes as failed
            foreach (var objective in _objectives)
            {
                if (objective.Trigger == null) continue;
                if (objective.Trigger.Node.QuestState != QuestState.Active) continue;
                objective.Trigger.Node.QuestState = QuestState.Failed;
            }
            
            QuestComplete = true;
            onQuestCompleteEvent?.Invoke();
            OnQuestAdvance?.Invoke();
        }

        private void ProcessTriggerProgress(QuestTrigger trigger)
        {
            // Handles progression for both branched and branchless objectives
            foreach (var objective in _objectives)
            {
                ProcessBranchProgress(objective, trigger);
                ProcessBranchlessProgress(objective, trigger);
            }
            OnQuestAdvance?.Invoke();
        }

        private void ProcessBranchProgress(QuestObjective objective, QuestTrigger trigger)
        {
            // Checks if the trigger is part of a branch and progresses if complete
            foreach (var branch in objective.GetBranches())
            {
                if (!branch.ChildObjects.Contains(trigger.gameObject)) continue;

                if (!branch.IsComplete()) continue;
                branch.OnProgress();
                
                var destTrigger = branch.DestinationObject?.GetComponent<QuestTrigger>();
                if (destTrigger == null) continue;
                ActivateTrigger(destTrigger);
            }
        }

        private void ProcessBranchlessProgress(QuestObjective objective, QuestTrigger trigger)
        {
            // Handles progression for objectives without branches
            if (objective.Trigger != trigger) return;

            foreach (var destination in trigger.Destinations)
            {
                var nextTrigger = GetTrigger(destination);
                if (nextTrigger == null) continue;
                ActivateTrigger(nextTrigger);
            }
        }

        private void ActivateTrigger(QuestTrigger trigger)
        {
            if (trigger == null) return;

            trigger.gameObject.SetActive(true);
            if (trigger.Node == null) return;
            
            trigger.Node.QuestState = QuestState.Active;
        }
        #endregion
    }
}