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
    public class DataTake : BaseQuestNodeData
    {
       [SerializeField] public BundleGraph bundleToTake;
       private readonly HashSet<Bundle.EElementFlag> validToTakeTags = new()
       {
           Bundle.EElementFlag.Item
       }; 
       public DataTake(QuestNode ownerNode, string tag) : base(ownerNode, tag)
       {
           iconGuid = ObjectIcon;
           bundleToTake = new BundleGraph(this);
           color = LBSSettings.Instance.view.colorTake;
       }
       
       public override void Clone(BaseQuestNodeData data)
       {
           base.Clone(data);
           if (data is not DataTake takeData) return;
           bundleToTake = takeData.bundleToTake;
       }
       
       public override List<string> ReferencedLayerNames()
       {
            List<string> list = new List<string> { bundleToTake.GetLayerName() };
            return list;
       }
       
       public override void Resize()
       {
           if (bundleToTake.Valid()) area = bundleToTake.Area;
       }

       public override bool Equals(BaseQuestNodeData other)
       {
           var takeOther =  other as DataTake;
           if (takeOther is null) return false;
           
           return Equals(takeOther.bundleToTake, bundleToTake);
       }

       public override bool IsValid()
       {
           return bundleToTake.Valid();
       }

       public override void SetDataByTiles(List<LBSLayer> layers, List<TileBundleGroup> tiles)
       {
           TrySetBundleGraph(layers, tiles, ref bundleToTake, validToTakeTags);
       }
    }
}