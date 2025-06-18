using System.Collections.Generic;
using ISILab.LBS.Components;
using UnityEngine;

namespace ISILab.LBS
{
    [QuestNodeActionTag("stealth")]
    public class QuestTriggerStealth : QuestTrigger
    {
        public DataStealth dataStealth;
        public List<GameObject> objectsObservers= new();
        public override void SetTypedData(BaseQuestNodeData baseData)
        {
            dataStealth  = (DataStealth)baseData;
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