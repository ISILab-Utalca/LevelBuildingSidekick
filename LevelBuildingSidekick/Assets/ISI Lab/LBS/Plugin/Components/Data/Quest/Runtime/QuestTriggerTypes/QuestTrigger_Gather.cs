using ISILab.LBS.Components;
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
            if (_playerInventory is not null)
            {
                _playerInventory.OnItemAdded += (itemGuid, quantity) =>
                {
                    if (itemGuid == _bundleGuid)
                    {
                        CheckComplete();
                    }
                };
            }
        }
        
        protected override bool CompleteCondition()
        {
            return _playerInventory.GetTypeAmount(_bundleGuid) >= dataGather.gatherAmount;
        }
    }

}