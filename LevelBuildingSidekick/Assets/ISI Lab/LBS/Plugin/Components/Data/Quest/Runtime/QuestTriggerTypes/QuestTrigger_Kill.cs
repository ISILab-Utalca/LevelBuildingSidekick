using System;
using System.Collections.Generic;
using System.Linq;
using ISILab.LBS.Components;
using UnityEngine;

namespace ISILab.LBS
{
    /** should rework this to bind an event to the enemies (Destroy) function call
     * instead of using OnTriggerStay.
     */
    [QuestNodeActionTag("kill")]
    public class QuestTriggerKill : QuestTrigger
    {
        public DataKill dataKill;
        public List<GameObject> objectsToKill = new();

        public override void Init()
        {
            base.Init();
            SetDataNode(dataKill);
        }

        public override void SetDataNode(BaseQuestNodeData baseData)
        {
            dataKill = (DataKill)baseData;
        }

        public void Start()
        {
            foreach (var obj in objectsToKill)
            {
                var destroyer = obj.GetComponent<DestroyNotifier>();
                destroyer.OnDestroyed += (obj)=>
                {
                  
                    objectsToKill.Remove(obj);
                    CheckComplete();
                };
            }
        }

        protected override bool CompleteCondition()
        {
            // if the list is empty all enemies were killed
            return !objectsToKill.Any();
        }
    }

}