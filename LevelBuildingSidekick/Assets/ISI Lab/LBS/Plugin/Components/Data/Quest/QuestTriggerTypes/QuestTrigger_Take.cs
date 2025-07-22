using ISILab.LBS.Components;
using UnityEngine;

namespace ISILab.LBS
{
    [QuestNodeActionTag("take")]
    public class QuestTriggerTake : QuestTrigger
    {
        public DataTake dataTake;
        public GameObject objectToTake;

        public override void Init()
        {
            base.Init();
            SetDataNode(dataTake);
        }

        public override void SetDataNode(BaseQuestNodeData baseData)
        {
            dataTake = (DataTake)baseData;
            var objectiveTrigger = objectToTake.AddComponent<GenericObjectiveTrigger>();
            objectiveTrigger.Setup(this);
        }

        protected override void OnTriggerEnter(Collider other) 
        {
            if (!IsPlayer(other)) return;
            // Use the "objectToTake" reference and add it to player controller
            // CheckComplete();
        }
        
    }

}