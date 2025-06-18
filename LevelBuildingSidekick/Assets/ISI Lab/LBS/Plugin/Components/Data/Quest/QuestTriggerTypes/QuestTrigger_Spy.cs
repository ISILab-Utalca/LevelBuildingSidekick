using ISILab.LBS.Components;
using UnityEngine;

namespace ISILab.LBS
{
    [QuestNodeActionTag("spy")]
    public class QuestTriggerSpy : QuestTrigger
    {
        public DataSpy dataSpy;
        public GameObject objectToSpy;
        
        public override void SetTypedData(BaseQuestNodeData baseData)
        {
            dataSpy =  (DataSpy)baseData;
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