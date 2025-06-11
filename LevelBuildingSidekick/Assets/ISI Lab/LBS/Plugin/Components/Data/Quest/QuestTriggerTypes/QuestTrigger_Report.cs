using ISILab.LBS;
using ISILab.LBS.Components;
using UnityEngine;

namespace ISILab.LBS
{
    [ISILab.LBS.QuestNodeActionTag(" report ")]
    public class QuestTriggerReport : QuestTrigger
    {
        public QuestTriggerReport() : base()
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