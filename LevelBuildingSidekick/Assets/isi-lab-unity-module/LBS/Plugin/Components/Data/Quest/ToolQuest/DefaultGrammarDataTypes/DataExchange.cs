using System;
using System.Collections.Generic;
using ISILab.LBS.Modules;
using ISILab.LBS.Settings;
using ISILab.Macros;
using LBS.Bundles;
using LBS.Components;
using Newtonsoft.Json;
using UnityEngine;

namespace ISILab.LBS.Components
{
    [Serializable]
    public class DataExchange : BaseQuestNodeData
    {
        [SerializeField] public BundleType bundleGiveType;
        [SerializeField, JsonRequired] public int requiredAmount = 1;
        /// <summary>
        /// Receive guid must be set from editor panel
        /// </summary>a
        [SerializeField] public BundleType bundleReceiveType;
        [SerializeField] public int receiveAmount = 1;
        
        private readonly HashSet<Bundle.EElementFlag> validExchangeTags = new()
        {
            Bundle.EElementFlag.Item
        }; 
        
        public DataExchange(QuestNode ownerNode, string tag) : base(ownerNode, tag)
        {
            color = LBSSettings.Instance.view.colorExchange;
        }
        
        public override void Clone(BaseQuestNodeData data)
        {
            base.Clone(data);
            if (data is not DataExchange exchangeData) return;
            bundleGiveType = exchangeData.bundleGiveType;
            requiredAmount = exchangeData.requiredAmount;
            bundleReceiveType = exchangeData.bundleReceiveType;
            receiveAmount = exchangeData.receiveAmount;
        }

        public override bool Equals(BaseQuestNodeData other)
        {
            var exchangeOther = other as DataExchange;
            if(exchangeOther == null) return false;
            
            return Equals(exchangeOther.bundleGiveType, bundleGiveType) && 
                   Equals(exchangeOther.bundleReceiveType, bundleReceiveType);
        }

        public override bool IsValid()
        {
            if (bundleGiveType is null || bundleReceiveType is null)
            {
                return false;
            }
            return   bundleGiveType.Valid() && bundleReceiveType.Valid();
        }

        public override void SetDataByTiles(List<LBSLayer> layers, List<TileBundleGroup> tiles)
        {
            TrySetBundleType(layers, tiles, ref bundleReceiveType, validExchangeTags);
            var bundleReceive = bundleReceiveType.GetGuid();

            int recieveCounter = 0;
            foreach (var tbg in tiles)
            {
                if(tbg.GetGuid() == bundleReceive) recieveCounter++;
            }
            receiveAmount = recieveCounter;
            
            TrySetBundleType(layers, tiles,  ref bundleGiveType, validExchangeTags);
            var bundleGive = bundleGiveType.GetGuid();
            int giveCounter = 0;
            foreach (var tbg in tiles)
            {
                if(tbg.GetGuid() == bundleGive) giveCounter++;
            }
            receiveAmount = giveCounter;
        }
    }
} 