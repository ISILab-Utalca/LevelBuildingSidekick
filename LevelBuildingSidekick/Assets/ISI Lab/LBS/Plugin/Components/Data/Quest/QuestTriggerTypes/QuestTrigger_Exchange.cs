using System;
using System.Linq;
using ISILab.LBS.Components;
using ISILab.Macros;
using LBS.Bundles;
using UnityEngine;

namespace ISILab.LBS
{
    
    
    /*
        Feel free to implement your own give and receive logic, but this example works under the implication of having an inventory
        of class LbsInventory.
    */
    
    [QuestNodeActionTag("exchange")]
    public class QuestTriggerExchange : QuestTrigger
    {
        public DataExchange dataExchange;
        [SerializeField]
        private string _giveType;
        [SerializeField]
        private string _receiveType;
        
        private LBSInventory _playerInventory;
        
        public int givenAmount;

        public override void Init()
        {
            base.Init();
            SetDataNode(dataExchange);
        }

        public override void SetDataNode(BaseQuestNodeData baseData)
        {
            dataExchange =  (DataExchange)baseData;
            _giveType = dataExchange.bundleGiveType.GetGuid();
            _receiveType =dataExchange.bundleReceiveType.GetGuid();
        }
        
        protected override bool CompleteCondition()
        {
            if (_playerInventory.HasType(_giveType))  givenAmount += _playerInventory.GetTypeAmount(_giveType);
            if (givenAmount < dataExchange.requiredAmount) return false;
            
            ReceiveObjects();
            return true;
        }
        
        private void ReceiveObjects()
        {
            _playerInventory.AddItems(_receiveType, dataExchange.receiveAmount);
          
        }

        protected override void OnTriggerEnter(Collider other) 
        {
            if (!IsPlayer(other)) return;
            _playerInventory = other.gameObject.GetComponent<LBSInventory>();

            CheckComplete();
        }
            
    }

}