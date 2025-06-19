using System;
using System.Collections.Generic;
using System.Linq;
using ISILab.LBS.Settings;
using LBS.Components;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;

namespace ISILab.LBS.Components
{

    /// <summary>
    /// Saves the bundle guid and the position in the graph to get in the scene
    /// </summary>
    /// 
    [Serializable]
    public struct BundleGraph
    {
        [SerializeField]public List<Vector2Int> tilePositions;
        [SerializeField]public string layerID;
        [SerializeField]public string guid;
        [SerializeField]public Vector2Int position;
        
        
        public BundleGraph(
            LBSLayer layer = null, 
            List<Vector2Int> tilePositions = null, 
            string guid = "", 
            Vector2Int position = new Vector2Int())
        {
            layerID = layer?.ID;
            this.tilePositions = tilePositions;
            this.guid = guid;
            this.position = position;
        }

        public Vector2 GetElementSize()
        {
            Vector2 size = Vector2.one;
            if (tilePositions.Count <= 0) return size;
            
            // find bound borders
            int minX = tilePositions.Min(p => p.x);
            int maxX = tilePositions.Max(p => p.x);
            int minY = tilePositions.Min(p => p.y);
            int maxY = tilePositions.Max(p => p.y);

            // get size of bound box
            int width = maxX - minX + 1;
            int height = maxY - minY + 1;
            // actual used space in the grid
            size = new Vector2(width, height);

            return size;
        }
        
        public bool Valid() => guid != string.Empty;
    }
    
    /// <summary>
    /// Saves the bundle type
    /// </summary>
    [Serializable]
    public struct BundleType
    {
        [SerializeField]public string guid;
    }
    
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
                    return new BaseQuestNodeData(owner, tag);
                }

                var nodeData = (BaseQuestNodeData)Activator.CreateInstance(dataClass, owner, tag);
                return nodeData;
            }
        }

        [Serializable]
        public class BaseQuestNodeData
        {
            #region FIELDS
            [SerializeField, JsonRequired]
            protected QuestNode owner;
            
            [SerializeField, JsonRequired]
            protected string tag;
          
            [SerializeField, JsonRequired] 
            protected Vector2Int position = Vector2Int.zero;
           
            [SerializeField, JsonRequired] 
            protected float size = 1;
            
            [SerializeField, JsonRequired] 
            protected Color color =  LBSSettings.Instance.view.behavioursColor;
            
            #endregion

            #region PROPERTIES
            public QuestNode Owner => owner;
            public string Tag => tag;
            public Vector2Int Position
            {
                get => position;
                set => position = value;
            }

            public float Size
            {
                get => size;
                set => size = value;
            }

            public Color Color => color;
            #endregion

            public BaseQuestNodeData(QuestNode owner, string tag)
            {
                this.tag = tag;
            }

            public virtual void Clone(BaseQuestNodeData data)
            {
                owner = data.owner;
                tag = data.tag;
                position = data.position;
                size = data.size;
            }
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
        }
        [Serializable]
        public class DataTake : BaseQuestNodeData
        {
           [SerializeField] public BundleGraph bundleToTake;
           public DataTake(QuestNode owner, string tag) : base(owner, tag)
           {
               bundleToTake = new BundleGraph();
               color = LBSSettings.Instance.view.colorTake;
           }
           
           public override void Clone(BaseQuestNodeData data)
           {
               base.Clone(data);
               if (data is not DataTake takeData) return;
               bundleToTake = takeData.bundleToTake;
           }
        }
        [Serializable]
        public class DataRead : BaseQuestNodeData
        {
            [SerializeField] public BundleGraph bundleToRead;
            public DataRead(QuestNode owner, string tag) : base(owner, tag)
            {
                bundleToRead = new BundleGraph();
                color = LBSSettings.Instance.view.colorRead;
            }
            
            public override void Clone(BaseQuestNodeData data)
            {
                base.Clone(data);
                if (data is not DataRead readData) return;
                bundleToRead = readData.bundleToRead;
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
                bundleGiveTo = new BundleGraph();
                color = LBSSettings.Instance.view.colorGive;
            }
            
            public override void Clone(BaseQuestNodeData data)
            {
                base.Clone(data);
                if (data is not DataGive giveData) return;
                bundleGive = giveData.bundleGive;
                bundleGiveTo = giveData.bundleGiveTo;
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
               bundleReportTo = new BundleGraph();
               color = LBSSettings.Instance.view.colorReport;
           }
           
           public override void Clone(BaseQuestNodeData data)
           {
               base.Clone(data);
               if (data is not DataReport reportData) return;
               bundleReportTo = reportData.bundleReportTo;
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
        }
        [Serializable]
        public class DataSpy : BaseQuestNodeData
        {
            [SerializeField] public BundleGraph bundleToSpy;
            [SerializeField] public float spyTime = 5f;
            [SerializeField] public bool resetTimeOnExit = true;
          public DataSpy(QuestNode owner, string tag) : base(owner, tag)
          {
              bundleToSpy = new BundleGraph();
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
                bundleListenTo = new BundleGraph();
                color = LBSSettings.Instance.view.colorListen;
            }
            
            public override void Clone(BaseQuestNodeData data)
            {
                base.Clone(data);
                if (data is not DataListen listenData) return;
                bundleListenTo = listenData.bundleListenTo;
            }
        }

        

        #endregion

} 