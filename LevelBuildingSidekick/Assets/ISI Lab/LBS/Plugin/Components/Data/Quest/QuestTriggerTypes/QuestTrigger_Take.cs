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
            if (other.CompareTag("Player"))
            {
                CheckComplete();
            }
        }
            
    }

}