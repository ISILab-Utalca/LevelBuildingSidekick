using System;
using System.Collections.Generic;
using System.Linq;
using ISILab.LBS.Components;
using ISILab.Macros;
using LBS.Bundles;
using UnityEngine;

namespace ISILab.LBS
{
    [QuestNodeActionTag("gather")]
    public class QuestTriggerGather : QuestTrigger
    {
        public DataGather dataGather;

        private string _bundleGuid = string.Empty;
        private LBSInventory _playerInventory;

        public override void Init()
        {
            base.Init();
            SetDataNode(dataGather);
        }

        public override void SetDataNode(BaseQuestNodeData baseData)
        {
            dataGather = (DataGather) baseData;
            _bundleGuid = dataGather.bundleGatherType.GetGuid();

        }

        protected override void OnTriggerEnter(Collider other)
        {
            if (!IsPlayer(other)) return;
            _playerInventory = other.gameObject.GetComponent<LBSInventory>();
        }

        private void OnTriggerStay(Collider other)
        {
            if (_playerInventory is null) return;
            if (_playerInventory.HasType(_bundleGuid)) CheckComplete();
        }

        protected override bool CompleteCondition()
        {
            return _playerInventory.GetTypeAmount(_bundleGuid) >= dataGather.gatherAmount;
        }
    }

}