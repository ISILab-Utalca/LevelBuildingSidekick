using System;
using System.Collections.Generic;
using ISILab.Extensions;
using ISILab.LBS.Modules;
using ISILab.Macros;
using LBS.Components;
using Newtonsoft.Json;
using UnityEngine;

namespace ISILab.LBS.Components
{
    [Serializable]
    public class DataGather : BaseQuestNodeData
    {
        /// <summary>
        /// material that must be gathered
        /// </summary>
        [SerializeField] public BundleType bundleGatherType;
        
        private readonly HashSet<LBSETag.Type> validGatherType = new()
        {
            LBSETag.Type.Item, 
            LBSETag.Type.Resource
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

        public override bool IsValid()
        {
            return bundleGatherType is not null && bundleGatherType.Valid();
        }

        public override void SetDataByTiles(List<LBSLayer> layers, List<TileBundleGroup> suggestionKey)
        {
            TrySetBundleType(layers, suggestionKey,  ref bundleGatherType, validGatherType);
        }
    }
}