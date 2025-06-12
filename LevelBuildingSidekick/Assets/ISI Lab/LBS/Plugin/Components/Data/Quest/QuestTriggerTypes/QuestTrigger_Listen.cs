using ISILab.LBS;
using ISILab.LBS.Components;
using UnityEngine;

namespace ISILab.LBS
{
    [ISILab.LBS.QuestNodeActionTag(" listen ")]
    public class QuestTriggerListen : QuestTrigger
    {
        public QuestTriggerListen() : base()
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