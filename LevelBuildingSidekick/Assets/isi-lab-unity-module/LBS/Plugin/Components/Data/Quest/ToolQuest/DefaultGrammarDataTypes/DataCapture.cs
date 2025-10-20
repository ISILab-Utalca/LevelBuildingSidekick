using System;
using System.Collections.Generic;
using ISILab.LBS.Modules;
using LBS.Components;
using UnityEngine;

namespace ISILab.LBS.Components
{

    [Serializable]
    public class DataCapture : BaseQuestNodeData
    {
        [SerializeField] public float captureTime = 5f;
        [SerializeField] public bool resetTimeOnExit = true;

        public DataCapture(QuestNode ownerNode, string tag) : base(ownerNode, tag)
        {
        }

        public override void Clone(BaseQuestNodeData data)
        {
            base.Clone(data);
            if (data is not DataCapture captureData) return;
            captureTime = captureData.captureTime;
            resetTimeOnExit = captureData.resetTimeOnExit;
        }

        public override bool Equals(BaseQuestNodeData other)
        {
            var captureOther = other as DataCapture;
            if(captureOther == null) return false;

            return Mathf.Approximately(captureOther.captureTime, captureTime);
        }

        public override bool IsValid()
        {
            return true;
        }

        public override void SetDataByTiles(List<LBSLayer> layers, List<TileBundleGroup> tiles)
        {
            // stub
        }
    }
}