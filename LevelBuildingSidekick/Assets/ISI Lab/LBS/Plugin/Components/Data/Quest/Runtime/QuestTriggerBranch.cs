using System;
using System.Collections.Generic;
using System.Linq;
using ISILab.LBS.Components;
using UnityEngine;

namespace ISILab.LBS
{
    /// <summary>
    /// Represents a branching condition in the quest system.
    /// Defines child triggers, a destination trigger, and logic
    /// for AND/OR evaluation.
    /// </summary>
    [DisallowMultipleComponent]
    [Serializable]
    public class QuestTriggerBranch : MonoBehaviour
    {
        #region FIELDS

        [SerializeField] 
        private List<GameObject> childObjects = new();

        [SerializeField] 
        private GameObject destinationObject;

        [SerializeField, SerializeReference] 
        private GraphNode branchNode;

        #endregion

        #region PROPERTIES

        public GraphNode BranchNode => branchNode;
        public List<GameObject> ChildObjects => childObjects;
        public GameObject DestinationObject => destinationObject;

        #endregion

        #region METHODS

        public bool IsAnd() => branchNode is AndNode;
        public bool IsOr() => branchNode is OrNode;
        public void SetNode(GraphNode node) => branchNode = node;

        #region Setters

        /// <summary>Assigns child triggers directly.</summary>
        public void SetChildTriggers(List<GameObject> triggersGo) => childObjects = triggersGo;

        /// <summary>Assigns the destination trigger to activate when branch conditions are satisfied.</summary>
        public void SetDestinationTrigger(GameObject triggerGo) => destinationObject = triggerGo;

        #endregion

        #region Getters

        /// <summary>Gets all child triggers from stored GameObjects.</summary>
        public List<QuestTrigger> GetChildTriggers()
        {
            return childObjects
                .Select(child => child.GetComponent<QuestTrigger>())
                .Where(component => component != null)
                .ToList();
        }

        /// <summary>Gets the destination trigger.</summary>
        public QuestTrigger GetDestinationTrigger() => destinationObject?.GetComponent<QuestTrigger>();

        #endregion

        #region Validation

        /// <summary>Returns true if the given trigger is part of this branch.</summary>
        public bool IsChildTrigger(QuestTrigger trigger) => GetChildTriggers().Contains(trigger);

        /// <summary>Returns true if the given GameObject contains a trigger that is part of this branch.</summary>
        public bool IsChildTrigger(GameObject go)
        {
            var trigger = go.GetComponent<QuestTrigger>();
            return trigger != null && IsChildTrigger(trigger);
        }

        /// <summary>
        /// Returns true if branch conditions are satisfied and destination can be activated.
        /// - AND: all children must be completed.
        /// - OR: at least one child must be completed.
        /// </summary>
        public bool IsComplete()
        {
            var triggers = GetChildTriggers();

            if (IsAnd())
                return triggers.All(trigger => trigger.IsCompleted);

            if (IsOr())
                return triggers.Any(trigger => trigger.IsCompleted);

            return false;
        }

        #endregion
        
        #endregion


        public void OnProgress()
        {
            // child (required actions) have been completed
            foreach (var child in ChildObjects)
            {
                var trigger =  child.GetComponent<QuestTrigger>();
                if(trigger is null) continue;
                
                trigger.IsCompleted = true;
                trigger.Node.QuestState = QuestState.Completed;
                child.SetActive(false);
            }
        }
    }
}
