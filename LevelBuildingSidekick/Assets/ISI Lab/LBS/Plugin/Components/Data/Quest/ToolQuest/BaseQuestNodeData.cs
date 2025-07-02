using System;
using System.Collections.Generic;
using System.Linq;
using ISILab.LBS.Modules;
using ISILab.LBS.Settings;
using ISILab.Macros;
using LBS.Components;
using Newtonsoft.Json;
using UnityEngine;

namespace ISILab.LBS.Components
{

    #region Targets
    [Serializable]
    public abstract class LayerTarget
    {
        [SerializeReference][SerializeField]protected LBSLayer layer;
        
        public LBSLayer GetLayer() => layer;
        
        public string GetLayerName()
        {
            return layer?.Name ?? "";
        }

        public abstract string GetGuid();
        public abstract bool Valid();

    }
    
    /// <summary>
    /// Saves the bundle guid and the position in the graph to get in the scene
    /// </summary>
    /// 
    [Serializable]
    public class BundleGraph : LayerTarget
    {
        [SerializeReference] [SerializeField] private TileBundleGroup tileBundle;
        [SerializeField] private BaseQuestNodeData _nodeData;
        // must be assigned on all bundleGraphs to the Resize Function
        
        public BundleGraph(BaseQuestNodeData nodeData, LBSLayer layer = null, TileBundleGroup tileBundle = null)
        {
            this.layer = layer;
            this.tileBundle = tileBundle;
            _nodeData = nodeData;
            
            if(this.tileBundle is null) return;
            if(_nodeData is null) return;
            this.tileBundle!.OnRemoved += ClearTileBundle;
        }

        private void ClearTileBundle()
        {
            tileBundle = null;
        }

        public Vector2Int Position => new((int)Area.x, (int)Area.y);
        public Rect Area
        {
            get
            {   
                if(tileBundle is null) return Rect.zero;
                return tileBundle.AreaRect;
            }
        }

        public override bool Valid() => GetGuid() != string.Empty;
        public override string GetGuid()
        {
            if(tileBundle?.BundleData?.Bundle is null) return string.Empty;
            return LBSAssetMacro.GetGuidFromAsset(tileBundle.BundleData.Bundle);
        }
    }
    
    
    /// <summary>
    /// Saves the bundle type
    /// </summary>
    [Serializable]
    public class BundleType : LayerTarget
    {
        [SerializeField]private string guid;
     
        public BundleType(
            LBSLayer layer = null, 
            TileBundleGroup tileBundle = null)
        {
            this.layer = layer;
            if (tileBundle != null) guid = LBSAssetMacro.GetGuidFromAsset(tileBundle.BundleData.Bundle);
        }

        public override string GetGuid()
        {
            return guid;
        }

        public override bool Valid()
        {
            return GetGuid()!= string.Empty;
        }
    }
    
    #endregion
    
    # region Factory
    /// <summary>
    /// Factory to create QuestNodeData based on actions.
    /// </summary>
    public static class QuestNodeDataFactory
    {
        private static readonly Dictionary<string, Type> TagDataTypes = new()
        {
            { " go to ", typeof(DataGoto) },
            { " explore ", typeof(DataExplore) },
            { " kill ", typeof(DataKill) } ,
            { " stealth ", typeof(DataStealth) },
            { " take ",typeof(DataTake) },
            { " read ", typeof(DataRead) },
            { " exchange ",typeof(DataExchange) },
            { " give ",typeof(DataGive) },
            { " report ",typeof(DataReport) },
            { " gather ", typeof(DataGather) },
            { " spy ",  typeof(DataSpy) },
            { " capture ", typeof(DataCapture) },
            { " listen ", typeof(DataListen) },
            { " empty ", null }
        };
        
        public static BaseQuestNodeData CreateByTag(string tag, QuestNode owner)
        {
            if (!TagDataTypes.TryGetValue(tag, out var dataClass))
            {
                return null;
            }

            var nodeData = (BaseQuestNodeData)Activator.CreateInstance(dataClass, owner, tag);
            return nodeData;
        }
    }
    
    #endregion

    #region QuestNodeData
    [Serializable]
    public abstract class BaseQuestNodeData
    {
        #region FIELDS
        [SerializeField, JsonRequired]
        protected QuestNode owner;
        
        [SerializeField, JsonRequired]
        protected string tag;

        [SerializeField, JsonRequired] 
        protected Rect area;
        
        [SerializeField, JsonRequired] 
        protected Color color =  LBSSettings.Instance.view.behavioursColor;
        
        #endregion

        #region PROPERTIES
        public QuestNode Owner => owner;
        public string Tag => tag;

        public Rect Area
        {
            get => area;
            set => area = value;
        }

        public Color Color => color;
        #endregion

        protected BaseQuestNodeData(QuestNode owner, string tag)
        {
            this.owner = owner;
            this.tag = tag;

            if (owner?.Graph?.OwnerLayer == null) return;
  
            var pos = owner.Graph.OwnerLayer.ToFixedPosition(owner.Position);
            area = new Rect(pos.x, pos.y, 1, 1);
        }


        public virtual void Clone(BaseQuestNodeData data)
        {
            owner = data.owner;
            tag = data.tag;
            area = data.area;
        }

        // by default there are no references to other layers.
        public virtual List<string> ReferencedLayerNames()
        {
            return null;
        }

        // by default no resize. Implement if using bundleGraph fields
        public virtual void Resize()
        {
           
        }
        protected void ResizeToFitBundles(IEnumerable<BundleGraph> bundles)
        {
            var validRects = bundles
                .Where(b => (b is not null) && b.Valid())
                .Select(b => b.Area)
                .ToList();

            if (validRects.Count == 0)
                return;

            // Only use the rects' x and y positions
            float minX = validRects.Min(r => r.x);
            float maxX = validRects.Max(r => r.x + r.width);
            float minY = validRects.Min(r => r.y - r.height);
            float maxY = validRects.Max(r => r.y ); // subtract height beacause of inverted Y in graph

            float width = maxX - minX;
            float height = maxY - minY;

            // Inverted Y origin: anchor from maxY going downward
            area = new Rect(minX, maxY, Mathf.Abs(width), Mathf.Abs(height));
        }


        public abstract bool IsValid();
    }

        /// <summary>
        /// ----------------------- FOR LBS USER ----------------------------------------------
        /// 
        /// Data containers for the default grammar. If another grammar is used or modify,
        /// Remember to update the QuestNodeAction dictionary with your own data.
        ///
        /// To assign data, must create your own panel on uxml QuestNodeBehaviourEditor.uxml
        ///
        /// -----------------------------------------------------------------------------------
        /// </summary>
        #region DATA CONTAINERS DEFAULT GRAMMAR
        
        [Serializable]
        public class DataGoto : BaseQuestNodeData
        {
            public DataGoto(QuestNode owner, string tag) : base(owner, tag)
            {
            }

            public override bool IsValid()
            {
                return true;
            }
        }
        [Serializable]
        public class DataExplore : BaseQuestNodeData
        {
            [SerializeField] public int subdivisions = 4;
    
            
            // if find random position is true, then upon generation a random position is created and that's what the 
            // player must trigger
            [SerializeField] public bool findRandomPosition;

            public DataExplore(QuestNode owner, string tag) : base(owner, tag)
            {
            }
            
            public override void Clone(BaseQuestNodeData data)
            {
                base.Clone(data);
                if (data is not DataExplore exploreData) return;
                subdivisions = exploreData.subdivisions;
                findRandomPosition = exploreData.findRandomPosition;
            }

            public override bool IsValid()
            {
                return true;
            }
        }
        [Serializable]
        public class DataKill : BaseQuestNodeData
        {
            /// <summary>
            /// Objects that must be killed
            /// </summary>
            [SerializeField] public List<BundleGraph> bundlesToKill;

            public DataKill(QuestNode owner, string tag) : base(owner, tag)
            {
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

            public override bool IsValid()
            {
                return bundlesToKill.Any();
            }
        }
        [Serializable]
        public class DataStealth : BaseQuestNodeData
        {
            [SerializeField]public Vector2Int objective = Vector2Int.zero;
            /// <summary>
            /// Objects with a default trigger that will stop catch the player
            /// </summary>
            [SerializeField]public List<BundleGraph> bundlesObservers;

            public DataStealth(QuestNode owner, string tag) : base(owner, tag)
            {
                color = LBSSettings.Instance.view.colorStealth;
                bundlesObservers = new List<BundleGraph>();
            }
            
            public override void Clone(BaseQuestNodeData data)
            {
                base.Clone(data);
                if (data is not DataStealth stealthData) return;
                objective = stealthData.objective;
                bundlesObservers = new List<BundleGraph>(stealthData.bundlesObservers);
            }
            
            public override List<string> ReferencedLayerNames()
            {
                return bundlesObservers.Select(bundleGraph => bundleGraph.GetLayerName()).ToList();
            }
            
            public override void Resize()
            {
                ResizeToFitBundles(bundlesObservers);
            }

            public override bool IsValid()
            {
                return bundlesObservers.Any();
            }
        }
        [Serializable]
        public class DataTake : BaseQuestNodeData
        {
           [SerializeField] public BundleGraph bundleToTake;
           public DataTake(QuestNode owner, string tag) : base(owner, tag)
           {
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
        }
        [Serializable]
        public class DataRead : BaseQuestNodeData
        {
            [SerializeField] public BundleGraph bundleToRead;
            public DataRead(QuestNode owner, string tag) : base(owner, tag)
            {
                bundleToRead = new BundleGraph(this);
                color = LBSSettings.Instance.view.colorRead;
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

            public override bool IsValid()
            {
                return bundleToRead.Valid();
            }
        }
        [Serializable]
        public class DataExchange : BaseQuestNodeData
        {
            [SerializeField] public BundleType bundleGiveType;
            [SerializeField, JsonRequired] public int requiredAmount = 1;
            /// <summary>
            /// Receive guid must be set from editor panel
            /// </summary>a
            [SerializeField] public BundleType bundleReceiveType;
            [SerializeField] public int receiveAmount = 1;
            public DataExchange(QuestNode owner, string tag) : base(owner, tag)
            {
                color = LBSSettings.Instance.view.colorExchange;
            }
            
            public override void Clone(BaseQuestNodeData data)
            {
                base.Clone(data);
                if (data is not DataExchange exchangeData) return;
                bundleGiveType = exchangeData.bundleGiveType;
                requiredAmount = exchangeData.requiredAmount;
                bundleReceiveType = exchangeData.bundleReceiveType;
                receiveAmount = exchangeData.receiveAmount;
            }

            public override bool IsValid()
            {
                return bundleGiveType.Valid() && bundleReceiveType.Valid();
            }
        }
        [Serializable]
        public class DataGive : BaseQuestNodeData
        {
            [SerializeField] public BundleType bundleGive;
            /// <summary>
            /// Character to give to 
            /// </summary>
            [SerializeField] public BundleGraph bundleGiveTo;
            public DataGive(QuestNode owner, string tag) : base(owner, tag)
            {
                bundleGive = new BundleType();
                bundleGiveTo = new BundleGraph(this);
                color = LBSSettings.Instance.view.colorGive;
            }
            
            public override void Clone(BaseQuestNodeData data)
            {
                base.Clone(data);
                if (data is not DataGive giveData) return;
                bundleGive = giveData.bundleGive;
                bundleGiveTo = giveData.bundleGiveTo;
            }
            
            public override List<string> ReferencedLayerNames()
            {
                List<string> list = new List<string> { bundleGiveTo.GetLayerName() };
                return list;
            }
            
            public override void Resize()
            {
                if (bundleGiveTo.Valid())  area = bundleGiveTo.Area;
            }

            public override bool IsValid()
            {
                return bundleGive.Valid() && bundleGiveTo.Valid();
            }
        }
        [Serializable]
        public class DataReport : BaseQuestNodeData
        {
            /// <summary>
            /// Character to report to
            /// </summary>
            [SerializeField] public BundleGraph bundleReportTo;
           public DataReport(QuestNode owner, string tag) : base(owner, tag)
           {
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
        }
        [Serializable]
        public class DataGather : BaseQuestNodeData
        {
            /// <summary>
            /// material that must be gathered
            /// </summary>
            [SerializeField] public BundleType bundleGatherType;
            [SerializeField, JsonRequired] public int gatherAmount;
          public DataGather(QuestNode owner, string tag) : base(owner, tag)
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
              return bundleGatherType.Valid() && gatherAmount > 0;
          }
        }
        [Serializable]
        public class DataSpy : BaseQuestNodeData
        {
            [SerializeField] public BundleGraph bundleToSpy;
            [SerializeField] public float spyTime = 5f;
            [SerializeField] public bool resetTimeOnExit = true;
          public DataSpy(QuestNode owner, string tag) : base(owner, tag)
          {
              bundleToSpy = new BundleGraph(this);
              color = LBSSettings.Instance.view.colorSpy;
          }
          
          public override void Clone(BaseQuestNodeData data)
          {
              base.Clone(data);
              if (data is not DataSpy spyData) return;
              bundleToSpy = spyData.bundleToSpy;
              spyTime = spyData.spyTime;
              resetTimeOnExit = spyData.resetTimeOnExit;
          }
          
          public override List<string> ReferencedLayerNames()
          {
              List<string> list = new List<string> { bundleToSpy.GetLayerName() };
              return list;
          }
          
          public override void Resize()
          {
              if (bundleToSpy.Valid())area = bundleToSpy.Area;
          }

          public override bool IsValid()
          {
              return bundleToSpy.Valid();
          }
        }
        [Serializable]
        public class DataCapture : BaseQuestNodeData
        {
            [SerializeField] public float captureTime = 5f;
            [SerializeField] public bool resetTimeOnExit = true;

            public DataCapture(QuestNode owner, string tag) : base(owner, tag)
            {
            }
            
            public override void Clone(BaseQuestNodeData data)
            {
                base.Clone(data);
                if (data is not DataCapture captureData) return;
                captureTime = captureData.captureTime;
                resetTimeOnExit = captureData.resetTimeOnExit;
            }

            public override bool IsValid()
            {
                return true;
            }
        }
        [Serializable]
        public class DataListen : BaseQuestNodeData
        {
            /// <summary>
            /// Character or objects that gets listened to
            /// </summary>
            [SerializeField] public BundleGraph bundleListenTo;

            public DataListen(QuestNode owner, string tag) : base(owner, tag)
            {
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

            public override bool IsValid()
            {
                return bundleListenTo.Valid();
            }
        }

        

        #endregion
        
    #endregion

} 