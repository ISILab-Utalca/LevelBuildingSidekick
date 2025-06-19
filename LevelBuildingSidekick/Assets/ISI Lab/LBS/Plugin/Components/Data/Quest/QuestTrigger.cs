using System;
using ISILab.LBS.Components;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace ISILab.LBS
{

    [DisallowMultipleComponent]
    [Serializable]
    public abstract class QuestTrigger : MonoBehaviour
    {
        #region FIELDS

        [SerializeField][HideInInspector]
        protected QuestNode node;
        
        [SerializeField, ReadOnly] 
        private string nodeID;
        
        protected BoxCollider BoxCollider;
        
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
        public UnityEvent onCompleteEvent;
        
        #endregion
        
        #region METHODS

        /// <summary>
        /// Replace and cast the incoming parameter to the required data type
        /// </summary>
        /// <param name="baseData"></param>
        public abstract void SetTypedData(BaseQuestNodeData baseData);

        /// <summary>
        /// Set the required values for each class type based on the Containers within the node data class.
        /// Always call base from overwrites as base sets the ID that quest observer uses on start 
        /// </summary>
        /// <param name="paramNode">incoming node with data</param>
        public void SetData(QuestNode paramNode)
        {
            this.node = paramNode;
            nodeID = paramNode.ID;
        }
        
        /// <summary>
        /// All triggers require a size by initialization.
        /// </summary>
        /// <param name="size"></param>
        public void SetSize(Vector3 size)
        {
            BoxCollider = gameObject.AddComponent<BoxCollider>();
            BoxCollider.isTrigger = true;
            BoxCollider.size = size;
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
        /// Checks the other for the Player Tag
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsPlayer(Collider other)  {return other.CompareTag("Player");}
        protected bool IsPlayer(GameObject other)  {return other.CompareTag("Player");}
        
        /// <summary>
        /// Override for unique conditions based on quest action.
        /// TRUE by default, when entering an area the condition is met.
        /// </summary>
        /// <returns></returns>
        protected virtual bool CompleteCondition()
        {
            return true; 
        }

        public void CheckComplete()
        {
            if (!CompleteCondition()) return;
            isCompleted = true;
            onCompleteEvent?.Invoke();
            OnTriggerCompleted?.Invoke(this);
            
            gameObject.SetActive(false); // Deactivate after completion to avoid trigger calls.
        }
        
        #endregion
    }

}