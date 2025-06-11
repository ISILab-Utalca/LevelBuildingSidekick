using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ISILab.LBS.Components;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;

namespace ISILab.LBS
{

    [DisallowMultipleComponent]
    [System.Serializable]
    public abstract class QuestTrigger : MonoBehaviour
    {
        #region FIELDS

        [SerializeField] 
        private QuestNode node;
        
        [SerializeField]
        private string nodeID;
        
        protected BoxCollider boxCollider;
        
        [SerializeField]
        protected bool isCompleted;
       
        #endregion

        #region PROPERTIES
        public string NodeID => nodeID;
        public bool IsCompleted
        {
            get => isCompleted;
            set => isCompleted = value;
        }

        #endregion
       
        #region EVENTS
        public event Action<QuestTrigger> OnTriggerCompleted;
        [SerializeField]
        public UnityEvent OnCompleteEvent;
        
        #endregion
        

        #region METHODS

        /// <summary>
        /// Set the required values for each class type based on the Containers within the node data class.
        /// Always call base from overwrites as base sets the ID that quest observer uses on start 
        /// </summary>
        /// <param name="data"></param>
        public virtual void SetData(QuestNode node)
        {
            this.node = node;
            nodeID = node.NodeData.Owner.ID;
        }
        
        /// <summary>
        /// All triggers require a size by initialization.
        /// </summary>
        /// <param name="size"></param>
        public void SetSize(Vector3 size)
        {
            boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;
            boxCollider.size = size;
        }

        /// <summary>
        /// Check condition when entering by default.
        /// Add different checks depending on quest action.
        /// For example, capture should check on STAY TRIGGER.
        /// </summary>
        /// <param name="other"></param>
        protected virtual void OnTriggerEnter(Collider other)
        {
            CheckComplete();
        }
        
        /// <summary>
        /// Override for unique conditions based on quest action.
        /// TRUE by default, when entering an area the condition is met.
        /// </summary>
        /// <returns></returns>
        protected virtual bool CompleteCondition()
        {
            return true; 
        }

        protected void CheckComplete()
        {
            if (!CompleteCondition()) return;
            isCompleted = true;
            OnCompleteEvent?.Invoke();
            OnTriggerCompleted.Invoke(this);
        }
        
        #endregion
    }
}