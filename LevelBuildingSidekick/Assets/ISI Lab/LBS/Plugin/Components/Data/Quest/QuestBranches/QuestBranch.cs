using System;
using System.Collections.Generic;
using ISILab.LBS.Components;
using UnityEngine;
using UnityEngine.Serialization;

namespace ISILab.LBS
{
    [DisallowMultipleComponent]
    [Serializable]
    public abstract class QuestBranch : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> childTriggers = new();

        [SerializeField]
        private GameObject destinationTrigger;

        [SerializeField, SerializeReference]
        public GraphNode graphNode;
        
        public List<GameObject> ChildTriggers => childTriggers;
        public GameObject DestinationTrigger => destinationTrigger;

        /// <summary>
        /// Set child triggers directly
        /// </summary>
        public void SetChildTriggers(List<GameObject> triggers)
        {
            childTriggers = triggers;
        }

        /// <summary>
        /// Set the destination trigger to activate when branch condition is satisfied
        /// </summary>
        public void SetDestinationTrigger(GameObject trigger)
        {
            destinationTrigger = trigger;
        }

        /// <summary>
        /// Returns true if branch conditions are satisfied and destination can be activated
        /// </summary>
        public abstract bool CanAdvance();
    }
    
}