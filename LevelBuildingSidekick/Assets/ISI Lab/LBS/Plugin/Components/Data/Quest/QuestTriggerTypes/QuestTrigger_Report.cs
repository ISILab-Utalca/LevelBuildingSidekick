using ISILab.LBS.Components;
using UnityEngine;

namespace ISILab.LBS
{
    [QuestNodeActionTag("report")]
    public class QuestTriggerReport : QuestTrigger
    {
        public DataReport dataReport;
        public GameObject objectToReport;
        
        public override void SetTypedData(BaseQuestNodeData baseData)
        {
            dataReport =  (DataReport)baseData;
        }

        protected override void OnTriggerEnter(Collider other)
        {
            if (!IsPlayer(other)) return;
            
            // Use the "objectToListen" reference to start a dialogue/report
            CheckComplete();
        }
            
    }

}