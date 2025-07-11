using System;
using ISILab.LBS.Components;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace ISILab.LBS
{

    [DisallowMultipleComponent]
    [Serializable]
    public class QuestTrigger : MonoBehaviour
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
        
        public QuestNode Node
        {
            get => node;
            set => node = value;
        }

        #endregion
       
        #region EVENTS
        
        public event Action<QuestTrigger> OnTriggerCompleted;
        [SerializeField, SerializeReference]
        public UnityEvent onCompleteEvent = new();
        
        #endregion
        
        #region METHODS

        protected void EnsureCollider()
        {
            if (BoxCollider != null) return;
            BoxCollider = GetComponent<BoxCollider>();
            if (BoxCollider != null) return;
            BoxCollider = gameObject.AddComponent<BoxCollider>();
            BoxCollider.isTrigger = true;
            BoxCollider.size = Vector3.one;
        }


        
        /// <summary>
        /// Call to set SetTypedData from Runtime Function
        /// </summary>
        public virtual void Init()
        {
            EnsureCollider();
        }

        /// <summary>
        /// Replace and cast the incoming parameter to the required data type
        /// </summary>
        /// <param name="baseData"></param>
        public virtual void SetDataNode(BaseQuestNodeData baseData)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set the required values for each class type based on the Containers within the node data class.
        /// Always call base from overwrites as base sets the ID that quest observer uses on start 
        /// </summary>
        /// <param name="paramNode">incoming node with data</param>
        public void SetData(QuestNode paramNode)
        {
            node = paramNode;
            nodeID = paramNode.ID;
        }
        
        /// <summary>
        /// All triggers require a size by initialization.
        /// </summary>
        /// <param name="size"></param>
        public void SetSize(Vector3 size)
        {
            size.x = Mathf.Abs(size.x);
            size.y = Mathf.Abs(size.y);
            size.z = Mathf.Abs(size.z);
            
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
            Completed();

        }

        private void Completed()
        {
            isCompleted = true;
            onCompleteEvent?.Invoke();
            
            if(node is not null) node.QuestState = QuestState.Completed;
            gameObject.SetActive(false); // Deactivate after completion to avoid trigger calls.
            
            OnTriggerCompleted?.Invoke(this);
        }
        
        
#if UNITY_EDITOR
        /// <summary>
        /// Right click the cog icon in the inspector of the Script
        /// </summary>
        [ContextMenu("Force Complete")]
        private void ForceComplete()
        {
            Completed();
        }
#endif
        
        #endregion
    }
    
    /// <summary>
    /// Generic class to add box collider to the a gameObject.
    /// By default its OnTriggerEnter will check for quest completion.
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class GenericObjectiveTrigger : MonoBehaviour
    {
        private QuestTrigger _questTrigger;
        private const float SizeFactor = 1f; // TODO: Maybe add as value in LBSTool(node data)
        public void Setup(QuestTrigger trigger)
        {
            _questTrigger = trigger;
    
            var boxCollider = GetComponent<BoxCollider>() ?? gameObject.AddComponent<BoxCollider>();
    
            boxCollider.isTrigger = true;
            boxCollider.size = Vector3.one * SizeFactor; 
            boxCollider.center = Vector3.zero;
    
        }
    
        protected void OnTriggerEnter(Collider other)
        {
            if (_questTrigger.IsPlayer(other)) _questTrigger.CheckComplete();
        }
    }

}

