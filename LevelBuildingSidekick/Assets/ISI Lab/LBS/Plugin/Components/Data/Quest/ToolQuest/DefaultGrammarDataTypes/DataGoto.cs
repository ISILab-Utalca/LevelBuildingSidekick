using System;
using System.Collections.Generic;
using ISILab.LBS.Modules;
using LBS.Components;

namespace ISILab.LBS.Components
{
    [Serializable]
    public class DataGoto : BaseQuestNodeData
    {
        public DataGoto(QuestNode ownerNode, string tag) : base(ownerNode, tag)
        {
        }

        public override bool Equals(BaseQuestNodeData other)
        {
            return Area == other.Area;
        }

        public override bool IsValid()
        {
            return true;
        }

        public override void SetDataByTiles(List<LBSLayer> layers, List<TileBundleGroup> tiles)
        {
            // stub
        }
        
        
    }
}
