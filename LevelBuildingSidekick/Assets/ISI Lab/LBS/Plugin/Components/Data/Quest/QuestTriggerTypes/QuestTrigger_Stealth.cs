using ISILab.LBS;
using ISILab.LBS.Components;
using UnityEngine;

namespace ISILab.LBS
{
    [ISILab.LBS.QuestNodeActionTag(" stealth ")]
    public class QuestTriggerStealth : QuestTrigger
    {
        public QuestTriggerStealth() : base()
        {
                
        }
            
        public override void SetData(QuestNode node)
        {
            base.SetData(node);
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