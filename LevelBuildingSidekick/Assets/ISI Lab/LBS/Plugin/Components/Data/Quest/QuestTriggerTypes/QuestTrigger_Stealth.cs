using System;
using System.Collections.Generic;
using ISILab.LBS.Components;
using UnityEngine;

namespace ISILab.LBS
{
    [QuestNodeActionTag("stealth")]
    public class QuestTriggerStealth : QuestTrigger
    {
        public DataStealth dataStealth;
        public List<GameObject> objectsObservers = new();
        /// <summary>
        /// The objective that must be reached to complete the quest
        /// </summary>
        public Vector3 objectivePosition;
        /// <summary>
        /// Tracks if the mission can be completed
        /// </summary>
        private bool _stealthDetected;

        public override void SetTypedData(BaseQuestNodeData baseData)
        {
            dataStealth = (DataStealth)baseData;
            
            foreach (var observer in objectsObservers)
            {
                if (observer is null)continue;
                
                var observerTrigger = observer.AddComponent<StealthObserverTrigger>();
                observerTrigger.Setup(this);
            }

            // Create objective trigger
            var objectiveGameObject = new GameObject("StealthObjectiveTrigger")
            {
                transform = { parent = transform, position = objectivePosition }
            };

            var objectiveTrigger = objectiveGameObject.AddComponent<StealthObjectiveTrigger>();
            objectiveTrigger.Setup(this);
        }
        
        protected override void OnTriggerEnter(Collider other)
        {
            // When entering the trigger area, reset the stealthDetected state
            if (IsPlayer(other)) _stealthDetected = false;
        }

        private void OnTriggerExit(Collider other)
        {
            // When entering the trigger area, reset the stealthDetected state
            if (IsPlayer(other)) _stealthDetected = false;
        }

        public void OnPlayerDetected()
        {
            _stealthDetected = true;
        }

        /// <summary>
        /// Complete if non detectable
        /// </summary>
        /// <returns></returns>
        protected override bool CompleteCondition()
        {
            return _stealthDetected == false;
        }
    }

    [RequireComponent(typeof(SphereCollider))]
    public class StealthObserverTrigger : MonoBehaviour
    {
        private QuestTriggerStealth _questTrigger;
        
        private const float DetectRadius = 5f; //TODO: Maybe add as value in LBSTool(node data)
        
        public void Setup(QuestTriggerStealth trigger)
        {
            _questTrigger = trigger;

            var sphereCollider = GetComponent<SphereCollider>() ?? gameObject.AddComponent<SphereCollider>();

            sphereCollider.isTrigger = true;
            sphereCollider.radius = DetectRadius; 
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_questTrigger.IsPlayer(other)) _questTrigger.OnPlayerDetected();
        }
    }

    [RequireComponent(typeof(BoxCollider))]
    public class StealthObjectiveTrigger : MonoBehaviour
    {
        private QuestTriggerStealth _questTrigger;
        private const float SizeFactor = 2f; // TODO: Maybe add as value in LBSTool(node data)
        public void Setup(QuestTriggerStealth trigger)
        {
            _questTrigger = trigger;

            var boxCollider = GetComponent<BoxCollider>() ?? gameObject.AddComponent<BoxCollider>();

            boxCollider.isTrigger = true;
            boxCollider.size = Vector3.one * SizeFactor; 
            boxCollider.center = Vector3.zero;

        }

        private void OnTriggerEnter(Collider other)
        {
            if (_questTrigger.IsPlayer(other)) _questTrigger.CheckComplete();
        }
    }
}