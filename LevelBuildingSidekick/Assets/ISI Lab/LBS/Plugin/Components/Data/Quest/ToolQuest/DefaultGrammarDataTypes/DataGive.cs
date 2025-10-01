using System;
using System.Collections.Generic;
using ISILab.Extensions;
using ISILab.LBS.Modules;
using ISILab.LBS.Settings;
using ISILab.Macros;
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
        
        private readonly HashSet<LBSETag.Type> validGiveTags = new()
        {
            LBSETag.Type.Item, 
            LBSETag.Type.Resource
        }; 
        
        private readonly HashSet<LBSETag.Type> validToGiveTags = new()
        {
            LBSETag.Type.Character,
            LBSETag.Type.Ally
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

        public override bool IsValid()
        {
            return bundleGive.Valid() && bundleGiveTo.Valid();
        }

        public override void SetDataByTiles(List<LBSLayer> layers, List<TileBundleGroup> suggestionKey)
        {
            TrySetBundleGraph(layers, suggestionKey, ref bundleGiveTo, validToGiveTags);
            TrySetBundleType(layers, suggestionKey, ref bundleGive, validGiveTags);
            
        }
    }
}