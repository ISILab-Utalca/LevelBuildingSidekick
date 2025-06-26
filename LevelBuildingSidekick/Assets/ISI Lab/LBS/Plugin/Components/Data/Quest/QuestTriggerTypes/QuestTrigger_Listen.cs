using ISILab.LBS.Components;
using UnityEngine;

namespace ISILab.LBS
{
    [QuestNodeActionTag("listen")]
    public class QuestTriggerListen : QuestTrigger
    {
        public DataListen dataListen;
        public GameObject objectToListen;
        public override void SetTypedData(BaseQuestNodeData baseData)
        {
            dataListen = (DataListen)baseData;
        }

        protected override void OnTriggerEnter(Collider other)
        {
            if (!IsPlayer(other)) return;
            
            // Use the "objectToListen" reference to start a dialogue
            CheckComplete();
        }
        
    }

}