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
        public class DataRead : BaseQuestNodeData
        {
            [SerializeField] public BundleGraph bundleToRead;
            
            private readonly HashSet<Bundle.EElementFlag> validToReadTags = new()
            {
                Bundle.EElementFlag.Item
            }; 
            
            public DataRead(QuestNode ownerNode, string tag) : base(ownerNode, tag)
            {
                bundleToRead = new BundleGraph(this);
                color = LBSSettings.Instance.view.colorRead;
                iconGuid = ObjectIcon;
            }
            
            public override void Clone(BaseQuestNodeData data)
            {
                base.Clone(data);
                if (data is not DataRead readData) return;
                bundleToRead = readData.bundleToRead;
            }
            
            public override List<string> ReferencedLayerNames()
            {
                List<string> list = new List<string> { bundleToRead.GetLayerName() };
                return list;
            }
            
            public override void Resize()
            {
                if (bundleToRead.Valid())area = bundleToRead.Area;
            }

            public override bool Equals(BaseQuestNodeData other)
            {
                var readOther = other as DataRead;
                if(readOther == null) return false;
                
                return bundleToRead.Equals(readOther.bundleToRead);
            }

            public override bool IsValid()
            {
                return bundleToRead.Valid();
            }

            public override void SetDataByTiles(List<LBSLayer> layers, List<TileBundleGroup> tiles)
            {
                TrySetBundleGraph(layers, tiles, ref bundleToRead, validToReadTags);
            }
        }
}