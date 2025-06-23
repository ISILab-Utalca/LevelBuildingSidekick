using ISILab.LBS.Components;
using UnityEngine;

namespace ISILab.LBS
{
    [QuestNodeActionTag("go to")]
    public class QuestTriggerGoTo : QuestTrigger
    {
        public DataGoto dataGoto;

        public override void SetTypedData(BaseQuestNodeData baseData)
        {
            dataGoto =  (DataGoto)baseData;
        }

        protected override void OnTriggerEnter(Collider other) 
        {
            if (!IsPlayer(other))return;
            
            CheckComplete();
        }
            
    }

}