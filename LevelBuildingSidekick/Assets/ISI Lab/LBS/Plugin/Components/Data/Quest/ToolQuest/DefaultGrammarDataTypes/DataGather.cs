using System;
using System.Collections.Generic;
using ISILab.Extensions;
using ISILab.LBS.Modules;
using ISILab.Macros;
using LBS.Bundles;
using LBS.Components;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ISILab.LBS.Components
{
    [Serializable]
    public class DataGather : BaseQuestNodeData
    {
        /// <summary>
        /// material that must be gathered
        /// </summary>
        [SerializeField] public BundleType bundleGatherType;
        
        private readonly HashSet<Bundle.EElementFlag> validGatherType = new()
        {
            Bundle.EElementFlag.Item
        }; 
        
        [SerializeField, JsonRequired] public int gatherAmount;
        public DataGather(QuestNode ownerNode, string tag) : base(ownerNode, tag)
        {
        }
          
        public override void Clone(BaseQuestNodeData data)
        {
            base.Clone(data);
            if (data is not DataGather gatherData) return;
            bundleGatherType = gatherData.bundleGatherType;
            gatherAmount = gatherData.gatherAmount;
        }

        public override bool Equals(BaseQuestNodeData other)
        {
            var gatherOther = other as DataGather;
            if(gatherOther == null) return false;
            
            return Equals(gatherOther.bundleGatherType, bundleGatherType);
        }

        public override bool IsValid()
        {
            return bundleGatherType is not null && bundleGatherType.Valid();
        }

        public override void SetDataByTiles(List<LBSLayer> layers, List<TileBundleGroup> tiles)
        {
            TrySetBundleType(layers, tiles,  ref bundleGatherType, validGatherType);
            if(bundleGatherType is null) return;
            
            var BundleType = bundleGatherType.GetGuid();
            if(BundleType is null) return;
            
            var maxCount = 0;
            foreach (var tbg in tiles)
            {
                if (tbg.GetGuid() == BundleType)
                {
                    maxCount++;
                }
            }
            var rnd = new System.Random();
            gatherAmount = rnd.Next(1, maxCount);
        }
    }
}