using ISILab.LBS.Components;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Experimental;

namespace ISILab.LBS
{
    [QuestNodeActionTag("explore")]
    public class QuestTriggerExplore : QuestTrigger
    {
        public DataExplore dataExplore;
        private int requiredExplored;

        public override void SetTypedData(BaseQuestNodeData baseData)
        {
            dataExplore = (DataExplore)baseData;
            if (dataExplore.findRandomPosition)
            {
                requiredExplored = 1;
                
                // get random position
                Vector3 randomPoint = GetRandomPointInBounds(BoxCollider);
                
                // New GameObject for this position
                GameObject triggerObject = new GameObject("RandomTrigger");
                triggerObject.transform.SetParent(transform);
                triggerObject.transform.position = randomPoint;
                
                var triggerGoto = triggerObject.AddComponent<QuestTriggerGoTo>();
                triggerGoto.onCompleteEvent.AddListener(() => requiredExplored--);
            }
            else
            {
                requiredExplored = dataExplore.subdivisions;
                
                // Create subdivisions based on the main trigger
                Vector3 mainSize = BoxCollider.size;
                Vector3 mainCenter = BoxCollider.center;
                float subdivisionSizeX = mainSize.x / dataExplore.subdivisions;
  
                for (int i = 0; i < dataExplore.subdivisions; i++)
                {
                    // Create a new GameObject for each subdivided trigger
                    GameObject triggerObject = new GameObject($"SubTrigger_{i}");
                    triggerObject.transform.SetParent(transform);
                    
                    // Calculate position for the new collider
                    float offsetX = (i - (dataExplore.subdivisions - 1) / 2f) * subdivisionSizeX;
                    Vector3 localPosition = mainCenter + new Vector3(offsetX, 0, 0);
                    triggerObject.transform.localPosition = localPosition;

                    // Add and configure BoxCollider
                    var triggerGoto = triggerObject.AddComponent<QuestTriggerGoTo>();
                    triggerGoto.onCompleteEvent.AddListener(() => requiredExplored--);

                }
            }
        }

        protected override void OnTriggerEnter(Collider other)
        {
            if (IsPlayer(other))
            {
                CheckComplete();
            }
        }

        private Vector3 GetRandomPointInBounds(BoxCollider collider)
        {
            Bounds bounds = collider.bounds;
            return new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z)
            );
        }

        protected override bool CompleteCondition()
        {
            return requiredExplored <= 0;
        }
        
        private void OnDestroy()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}