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
    public class DataReport : BaseQuestNodeData
    {
        /// <summary>
        /// Character to report to
        /// </summary>
        [SerializeField] public BundleGraph bundleReportTo;
        
        private readonly HashSet<LBSETag.Type> validToReportTags = new()
        {
            LBSETag.Type.Character,
            LBSETag.Type.Ally
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

        public override bool IsValid()
        {
            return bundleReportTo.Valid();
        }

        public override void SetDataByTiles(List<LBSLayer> layers, List<TileBundleGroup> suggestionKey)
        {
            foreach (var suggestionTile in suggestionKey)
            {
                var GraphData = LBSLayerHelper.GetBundleTileByMouse(suggestionTile.AreaRect.position.ToInt(), layers);
                if (GraphData is null) continue;
                var bundle = GraphData.Item2.BundleData.Bundle;
                if (!bundle.GetHasAnyTagCharacteristics(LBSETag.GetTags(validToReportTags))) continue;

                bundleReportTo = new BundleGraph(this, GraphData.Item1,  GraphData.Item2);
                if (bundleReportTo is not null) break;
            }
        }
    }
}