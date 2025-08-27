using System.Collections.Generic;
using UnityEngine;

namespace ISILab.LBS
{
    public abstract class QuestBranch : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> childTriggers = new();

        [SerializeField]
        private GameObject destinationTrigger;

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