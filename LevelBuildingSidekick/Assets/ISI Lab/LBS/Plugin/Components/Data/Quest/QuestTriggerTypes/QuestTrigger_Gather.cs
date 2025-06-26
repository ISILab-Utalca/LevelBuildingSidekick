using System;
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

        [SerializeField]
        private Type _gatherType;
        
        private LBSInventory _playerInventory;
        
        public override void SetTypedData(BaseQuestNodeData baseData)
        {
            dataGather = (DataGather) baseData;
            _gatherType = LBSAssetMacro.LoadAssetByGuid<Bundle>(dataGather.bundleGatherType.GetGuid()).Assets.FirstOrDefault()?.obj.GetType();
        }

        protected override void OnTriggerEnter(Collider other)
        {
            if (!IsPlayer(other)) return;
            _playerInventory = other.gameObject.GetComponent<LBSInventory>();
        }

        private void OnTriggerStay(Collider other)
        {
            if (_playerInventory is null) return;
            if (!_playerInventory.HasType(_gatherType)) return;
            
            CheckComplete();
        }

        protected override bool CompleteCondition()
        {
            return _playerInventory.GetTypeAmount(_gatherType) >= dataGather.gatherAmount;
        }
    }

}