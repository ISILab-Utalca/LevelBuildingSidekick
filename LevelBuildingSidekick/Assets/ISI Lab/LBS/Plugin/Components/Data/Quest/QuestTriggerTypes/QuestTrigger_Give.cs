using System;
using System.Linq;
using ISILab.LBS.Components;
using ISILab.Macros;
using LBS.Bundles;
using UnityEngine;

namespace ISILab.LBS
{
    [QuestNodeActionTag("give")]
    public class QuestTriggerGive : QuestTrigger
    {
        public DataGive dataGive;
        [SerializeField]
        private string _giveObjectType;
        /// <summary>
        /// Reference to the npc in the map, in case a dialogue/interaction wants to be triggered
        /// </summary>
        public GameObject objectToGiveTo;
        private LBSInventory _playerInventory;

        public override void Init()
        {
            base.Init();
            SetDataNode(dataGive);
        }

        public override void SetDataNode(BaseQuestNodeData baseData)
        {
            dataGive = (DataGive)baseData;
            _giveObjectType = dataGive.bundleGive.GetGuid();
        }

        protected override void OnTriggerEnter(Collider other) 
        {
            if (!IsPlayer(other)) return;
            _playerInventory = other.gameObject.GetComponent<LBSInventory>();
          
        }

        private void OnTriggerStay(Collider other)
        {
            if (_playerInventory is null) return;
            CheckComplete();
        }

        protected override bool CompleteCondition()
        {
            return !_playerInventory.HasType(_giveObjectType);
        }
    }

}