using ISILab.LBS.Components;
using UnityEngine;

namespace ISILab.LBS
{
    [QuestNodeActionTag("report")]
    public class QuestTriggerReport : QuestTrigger
    {
        public DataReport dataReport;
        public GameObject objectToReport;

        public override void Init()
        {
            base.Init();
            SetDataNode(dataReport);
        }

        public override void SetDataNode(BaseQuestNodeData baseData)
        {
            dataReport =  (DataReport)baseData;
            var objectiveTrigger = objectToReport.AddComponent<GenericObjectiveTrigger>();
            objectiveTrigger.Setup(this);
        }

        protected override void OnTriggerEnter(Collider other)
        {
            if (!IsPlayer(other)) return;
            
            // Use the "objectToReport" reference to start a dialogue/report
            //CheckComplete();
        }
            
    }

}