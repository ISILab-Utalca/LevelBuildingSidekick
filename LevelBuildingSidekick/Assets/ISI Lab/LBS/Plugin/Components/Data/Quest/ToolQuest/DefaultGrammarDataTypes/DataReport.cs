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
    public class DataReport : BaseQuestNodeData
    {
        /// <summary>
        /// Character to report to
        /// </summary>
        [SerializeField] public BundleGraph bundleReportTo;
        
        private readonly HashSet<Bundle.EElementFlag> validToReportTags = new()
        {
            Bundle.EElementFlag.Ally
        }; 
        
        public DataReport(QuestNode ownerNode, string tag) : base(ownerNode, tag)
        {
            iconGuid = StarIcon;
            bundleReportTo = new BundleGraph(this);
            color = LBSSettings.Instance.view.colorReport;
        }
           
        public override void Clone(BaseQuestNodeData data)
        {
            base.Clone(data);
            if (data is not DataReport reportData) return;
            bundleReportTo = reportData.bundleReportTo;
        }
           
        public override List<string> ReferencedLayerNames()
        {
            List<string> list = new List<string> { bundleReportTo.GetLayerName() };
            return list;
        }
           
        public override void Resize()
        {
            if (bundleReportTo.Valid()) area = bundleReportTo.Area;
        }

        public override bool Equals(BaseQuestNodeData other)
        {
            var reportOther = other as DataReport;
            if(reportOther == null) return false;
            
            return Equals(bundleReportTo, reportOther.bundleReportTo);
        }

        public override bool IsValid()
        {
            return bundleReportTo.Valid();
        }

        public override void SetDataByTiles(List<LBSLayer> layers, List<TileBundleGroup> tiles)
        {
            TrySetBundleGraph(layers,  tiles, ref bundleReportTo, validToReportTags);
        }
    }
}