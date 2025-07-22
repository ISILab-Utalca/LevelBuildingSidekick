using System;
using ISILab.LBS.Components;
using UnityEngine;

namespace ISILab.LBS
{
    [QuestNodeActionTag("capture")]
    public class QuestTriggerCapture : QuestTrigger
    {
        public DataCapture dataCapture;
        private float ActiveCaptureTime { get; set; }

        public override void Init()
        {
            base.Init();
            SetTypedData(dataCapture);
        }

        public override void SetDataNode(BaseQuestNodeData baseData)
        {
            dataCapture = (DataCapture)baseData;
        }

        protected override void OnTriggerEnter(Collider other) 
        {
            
        }

        protected void OnTriggerStay(Collider other)
        {
            if (!IsPlayer(other)) return;
            
            ActiveCaptureTime += Time.deltaTime;
            if(ActiveCaptureTime > dataCapture.captureTime) CheckComplete();
        }

        protected void OnTriggerExit(Collider other)
        {
            if (!IsPlayer(other)) return;
            
            if (dataCapture.resetTimeOnExit) ActiveCaptureTime = 0f;
        }
        
        public void SetTypedData(DataCapture data)
        {
            dataCapture = data;
        }
    }

}