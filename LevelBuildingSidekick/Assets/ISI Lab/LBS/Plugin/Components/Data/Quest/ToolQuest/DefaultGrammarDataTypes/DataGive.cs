using System;
using System.Collections.Generic;
using ISILab.LBS.Modules;
using ISILab.LBS.Settings;
using LBS.Bundles;
using LBS.Components;
using UnityEngine;

namespace ISILab.LBS.Components
{
    [Serializable]
    public class DataGive : BaseQuestNodeData
    {
        [SerializeField] public BundleType bundleGive;
        /// <summary>
        /// Character to give to 
        /// </summary>
        [SerializeField] public BundleGraph bundleGiveTo;
        
        private readonly HashSet<Bundle.EElementFlag> validGiveTags = new()
        {
            Bundle.EElementFlag.Item
        }; 
        
        private readonly HashSet<Bundle.EElementFlag> validToGiveTags = new()
        {
            Bundle.EElementFlag.Ally
        }; 
        
        public DataGive(QuestNode ownerNode, string tag) : base(ownerNode, tag)
        {
            iconGuid = StarIcon;
            bundleGive = new BundleType();
            bundleGiveTo = new BundleGraph(this);
            color = LBSSettings.Instance.view.colorGive;
        }
        
        public override void Clone(BaseQuestNodeData data)
        {
            base.Clone(data);
            if (data is not DataGive giveData) return;
            bundleGive = giveData.bundleGive;
            bundleGiveTo = giveData.bundleGiveTo;
        }
        
        public override List<string> ReferencedLayerNames()
        {
            List<string> list = new List<string> { bundleGiveTo.GetLayerName() };
            return list;
        }
        
        public override void Resize()
        {
            if (bundleGiveTo.Valid())  area = bundleGiveTo.Area;
        }

        public override bool Equals(BaseQuestNodeData other)
        {
            var giveOther = other as DataGive;
            if(giveOther == null) return false;
            
            return Equals(bundleGive, giveOther.bundleGive) && 
                   Equals(bundleGiveTo, giveOther.bundleGiveTo);
        }

        public override bool IsValid()
        {
            return bundleGive.Valid() && bundleGiveTo.Valid();
        }

        public override void SetDataByTiles(List<LBSLayer> layers, List<TileBundleGroup> tiles)
        {
            TrySetBundleGraph(layers, tiles, ref bundleGiveTo, validToGiveTags);
            TrySetBundleType(layers, tiles, ref bundleGive, validGiveTags);
        }
    }
}