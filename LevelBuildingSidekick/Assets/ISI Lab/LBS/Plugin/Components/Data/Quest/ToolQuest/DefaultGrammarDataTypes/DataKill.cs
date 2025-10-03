using System;
using System.Collections.Generic;
using System.Linq;
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
        public class DataKill : BaseQuestNodeData
        {
            /// <summary>
            /// Objects that must be killed
            /// </summary>
            [SerializeField] public List<BundleGraph> bundlesToKill;

            private readonly HashSet<Bundle.EElementFlag> requiredKillTags = new()
            {
                Bundle.EElementFlag.Enemy
            }; 
            
            public DataKill(QuestNode ownerNode, string tag) : base(ownerNode, tag)
            {
                iconGuid = FoeIcon;
                color = LBSSettings.Instance.view.colorKill;
                bundlesToKill = new List<BundleGraph>();
            }
            
            public override void Clone(BaseQuestNodeData data)
            {
                base.Clone(data);
                if (data is not DataKill killData) return;
                bundlesToKill = new List<BundleGraph>(killData.bundlesToKill);
            }

            public override List<string> ReferencedLayerNames()
            {
                return bundlesToKill.Select(bundleGraph => bundleGraph.GetLayerName()).ToList();
            }
            
            public override void Resize()
            {
                ResizeToFitBundles(bundlesToKill);
            }

            public override bool Equals(BaseQuestNodeData other)
            {
                var killOther = other as DataKill;
                if(killOther == null) return false;
                
                HashSet<BundleGraph> killHash = new HashSet<BundleGraph>();
                foreach (var observer in bundlesToKill)
                {
                    killHash.Add(observer);
                }
                HashSet<BundleGraph> otherKillHash = new HashSet<BundleGraph>();
                foreach (var observer in killOther.bundlesToKill)
                {
                    otherKillHash.Add(observer);
                }
            
                if (killHash.Count != otherKillHash.Count) return false;
            
                return killHash.SetEquals(otherKillHash);
            }

            public override bool IsValid()
            {
                return bundlesToKill.Any();
            }

            public override void SetDataByTiles(List<LBSLayer> layers, List<TileBundleGroup> tiles)
            {
                TrySetBundleGraphList(layers,  tiles, ref bundlesToKill, requiredKillTags);
            }
        }
}