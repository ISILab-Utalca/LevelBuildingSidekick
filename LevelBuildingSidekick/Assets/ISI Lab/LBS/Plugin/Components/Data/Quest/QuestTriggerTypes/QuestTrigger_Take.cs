using ISILab.LBS.Components;
using UnityEngine;

namespace ISILab.LBS
{
    [QuestNodeActionTag("take")]
    public class QuestTriggerTake : QuestTrigger
    {
        public DataTake dataTake;
        public GameObject objectToTake;
        
        public override void SetTypedData(BaseQuestNodeData baseData)
        {
            dataTake = (DataTake)baseData;
        }

        protected override void OnTriggerEnter(Collider other) 
        {
            if (!IsPlayer(other)) return;
            
            // Use the "objectToTake" reference and add it to player controller
            var playerInventory = other.GetComponent<LBSInventory>();
            if (playerInventory)
            {
                playerInventory.AddItems(objectToTake.GetType(),1);
                objectToTake.gameObject.SetActive(false);
            }
            CheckComplete();
        }
            
    }

}