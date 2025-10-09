using System;
using System.Collections.Generic;
using ISILab.Extensions;
using ISILab.LBS.Modules;
using ISILab.LBS.Settings;
using ISILab.Macros;
using LBS.Bundles;
using LBS.Components;
using UnityEngine;

namespace ISILab.LBS.Components
{
    [Serializable]
    public class DataListen : BaseQuestNodeData
    {
        /// <summary>
        /// Character or objects that gets listened to
        /// </summary>
        [SerializeField] public BundleGraph bundleListenTo;
        private readonly HashSet<Bundle.EElementFlag> validListenTo = new()
        {
            Bundle.EElementFlag.Ally
        }; 
        public DataListen(QuestNode ownerNode, string tag) : base(ownerNode, tag)
        {
            iconGuid = StarIcon;
            bundleListenTo = new BundleGraph(this);
            color = LBSSettings.Instance.view.colorListen;
        }
            
        public override void Clone(BaseQuestNodeData data)
        {
            base.Clone(data);
            if (data is not DataListen listenData) return;
            bundleListenTo = listenData.bundleListenTo;
        }
            
        public override List<string> ReferencedLayerNames()
        {
            List<string> list = new List<string> { bundleListenTo.GetLayerName() };
            return list;
        }
            
        public override void Resize()
        {
            if (bundleListenTo.Valid())area = bundleListenTo.Area;
        }

        public override bool Equals(BaseQuestNodeData other)
        {
            var listenOther = other as DataListen;
            if(listenOther == null) return false;
            
            return Equals(listenOther.bundleListenTo, bundleListenTo);
        }

        public override bool IsValid()
        {
            return bundleListenTo.Valid();
        }

        public override void SetDataByTiles(List<LBSLayer> layers, List<TileBundleGroup> tiles)
        {
            TrySetBundleGraph(layers, tiles, ref bundleListenTo, validListenTo);
        }
    }
}