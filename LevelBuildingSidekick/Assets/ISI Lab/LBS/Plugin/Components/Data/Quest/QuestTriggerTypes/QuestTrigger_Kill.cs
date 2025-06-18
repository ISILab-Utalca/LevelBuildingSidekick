using System.Collections.Generic;
using ISILab.LBS.Components;
using UnityEngine;

namespace ISILab.LBS
{
    [QuestNodeActionTag("kill")]
    public class QuestTriggerKill : QuestTrigger
    {
        public DataKill dataKill;
        public List<GameObject> objectsToKill = new();
        public override void SetTypedData(BaseQuestNodeData baseData)
        {
            dataKill = (DataKill)baseData;
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