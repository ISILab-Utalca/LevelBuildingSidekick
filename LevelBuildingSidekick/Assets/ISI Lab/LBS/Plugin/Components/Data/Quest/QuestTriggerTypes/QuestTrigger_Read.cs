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
            var objectiveTrigger = objectToRead.AddComponent<GenericObjectiveTrigger>();
            objectiveTrigger.Setup(this);
        }

        protected override void OnTriggerEnter(Collider other)
        {
            if (!IsPlayer(other)) return;
            // Use the "objectToRead" reference to start an interaction
            // CheckComplete();
        }
            
    }

}