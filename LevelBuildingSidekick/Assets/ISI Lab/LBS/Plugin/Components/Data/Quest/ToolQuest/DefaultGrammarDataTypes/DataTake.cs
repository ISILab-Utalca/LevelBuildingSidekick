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
    public class DataTake : BaseQuestNodeData
    {
       [SerializeField] public BundleGraph bundleToTake;
       private readonly HashSet<LBSETag.Type> validToTakeTags = new()
       {
           LBSETag.Type.Item,
           LBSETag.Type.Resource
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

       public override bool IsValid()
       {
           return bundleToTake.Valid();
       }

       public override void SetDataByTiles(List<LBSLayer> layers, List<TileBundleGroup> suggestionKey)
       {
           TrySetBundleGraph(layers, suggestionKey, ref bundleToTake, validToTakeTags);
       }
    }
}