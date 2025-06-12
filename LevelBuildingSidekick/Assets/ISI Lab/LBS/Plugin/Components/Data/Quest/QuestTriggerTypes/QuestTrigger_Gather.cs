using ISILab.LBS;
using ISILab.LBS.Components;
using UnityEngine;

namespace ISILab.LBS
{
    [ISILab.LBS.QuestNodeActionTag(" gather ")]
    public class QuestTriggerGather : QuestTrigger
    {
        public QuestTriggerGather() : base()
        {
                
        }
            
        public override void SetData(QuestNode node)
        {
            base.SetData(node);
            Debug.Log("GOTO DATA SET");
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