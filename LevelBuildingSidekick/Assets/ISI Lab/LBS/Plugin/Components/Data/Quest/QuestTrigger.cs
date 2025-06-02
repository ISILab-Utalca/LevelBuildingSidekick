using System;
using System.Collections;
using System.Collections.Generic;
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
        protected BoxCollider boxCollider;
        [SerializeField]
        protected bool isCompleted;
       
        #endregion

        #region PROPERTIES
        public bool IsCompleted
        {
            get => isCompleted;
            set => isCompleted = value;
        }

        #endregion
       
        #region EVENTS
        [SerializeField]
        public UnityEvent OnCompleteEvent;
        
        public void InvokeCallback()
        {
            OnCompleteEvent?.Invoke();
        }
        
        #endregion
        

        #region METHODS

        /// <summary>
        /// Set the required values for each class type based on the Containers within the node data class
        /// </summary>
        /// <param name="data"></param>
        public abstract void SetData(BaseQuestNodeData data);
        
        /// <summary>
        /// All triggers require a size by initialization.
        /// </summary>
        /// <param name="size"></param>
        public void SetSize(Vector3 size)
        {
           // if (size.x == 0 || size.z == 0) return;
                

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
            if (CompleteCondition())
            {
                Debug.Log("Complete");
                isCompleted = true;
                InvokeCallback();
            }
        }
        
        #endregion
    }
    
    [QuestNodeActionTag("go to")]
    public class QuestTrigger_GoTo : QuestTrigger
    {
        public QuestTrigger_GoTo() : base()
        {
            
        }
        
        public override void SetData(BaseQuestNodeData data)
        {
            Debug.Log("GOTO DATA SET");
        }

        protected override void OnTriggerEnter(Collider other) 
        {
            if (other.CompareTag("Player"))
            {
                CheckComplete();
            }
        }
        
    }

    
    
    
}