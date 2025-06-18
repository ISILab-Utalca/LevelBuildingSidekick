using ISILab.LBS.Components;
using UnityEngine;

namespace ISILab.LBS
{
    [QuestNodeActionTag("read")]
    public class QuestTriggerRead : QuestTrigger
    {
        public DataRead readData;
        public GameObject objectToRead;
        public override void SetTypedData(BaseQuestNodeData baseData)
        {
            readData = (DataRead)baseData;
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