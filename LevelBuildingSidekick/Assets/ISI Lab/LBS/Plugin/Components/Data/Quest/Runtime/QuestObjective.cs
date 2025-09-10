using System.Collections.Generic;
using System.Linq;
using ISILab.LBS.Components;
using UnityEditor;
using UnityEngine;

namespace ISILab.LBS
{
    public class QuestObjective
    {
        private QuestTrigger objectiveTrigger;
        private QuestTrigger trigger;
        private bool inBranch;
        
        
        private Dictionary<QuestTriggerBranch, List<QuestTrigger>> subObjectives = new();

        public QuestTrigger Trigger => trigger;
           
            
        public QuestObjective(QuestTrigger trigger)
        {
            this.trigger = trigger;
        }
            
        public List<QuestTrigger> GetSubObjectives(QuestTriggerBranch triggerBranch)
        {
            return  subObjectives.GetValueOrDefault(triggerBranch);
        }

        public List<QuestTriggerBranch> GetBranches()
        {
            return subObjectives.Keys.ToList();
        }

        public void SetSubobjectives(QuestTriggerBranch triggerBranch)
        {
            if (triggerBranch is null) return;
                
            List<QuestTrigger> triggers = new List<QuestTrigger>();

            foreach (var go in triggerBranch.ChildObjects)
            {
                var triggerComp = go.GetComponent<QuestTrigger>();
                if (triggerComp is not null)
                {
                    triggers.Add(triggerComp);
                    // assign to the trigger the branch node it belongs to
                    triggerComp.OwnerBranchNode = triggerBranch.BranchNode;
                }
            }
                
            subObjectives.Add(triggerBranch, triggers);
        }
    }

}