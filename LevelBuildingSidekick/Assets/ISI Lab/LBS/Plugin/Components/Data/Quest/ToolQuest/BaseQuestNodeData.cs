using System;
using System.Collections.Generic;
using ISILab.LBS.Settings;
using LBS.Components;
using Newtonsoft.Json;
using UnityEngine;

namespace ISILab.LBS.Components
{
    
    /// <summary>
    /// Saves the bundle guid and the position in the graph to get in the scene
    /// </summary>
    public struct BundleGraph
    {
        
        public LBSLayer Layer;
        public string Guid;
        public Vector2Int Position;
        
        public BundleGraph(LBSLayer layer, string guid, Vector2Int position)
        {
            this.Layer = layer;
            this.Guid = guid;
            this.Position = position;
        }

        public bool Valid() => Guid != string.Empty;
    }
    
    /// <summary>
    /// Saves the bundle type
    /// </summary>
    public struct BundleType
    {
        public string Guid;
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

            public void Clone(BaseQuestNodeData data)
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
            [SerializeField, JsonRequired] public int subdivisions = 4;
    
            
            // if find random position is true, then upon generation a random position is created and that's what the 
            // player must trigger
            [SerializeField, JsonRequired] public bool findRandomPosition;

            public DataExplore(QuestNode owner, string tag) : base(owner, tag)
            {
            }
        }
        [Serializable]
        public class DataKill : BaseQuestNodeData
        {
            /// <summary>
            /// Objects that must be killed
            /// </summary>
            [JsonRequired] public List<BundleGraph> BundlesToKill;

            public DataKill(QuestNode owner, string tag) : base(owner, tag)
            {
                color = LBSSettings.Instance.view.colorKill;
                BundlesToKill = new List<BundleGraph>();
            }
        }
        [Serializable]
        public class DataStealth : BaseQuestNodeData
        {
            [SerializeField, JsonRequired] public Vector2Int objective = Vector2Int.zero;
            /// <summary>
            /// Objects with a default trigger that will stop catch the player
            /// </summary>
            [JsonRequired] public List<BundleGraph> BundlesObservers;

            public DataStealth(QuestNode owner, string tag) : base(owner, tag)
            {
                color = LBSSettings.Instance.view.colorStealth;
                BundlesObservers = new List<BundleGraph>();
            }
        }
        [Serializable]
        public class DataTake : BaseQuestNodeData
        {
            [JsonRequired] public BundleGraph BundleToTake;
           public DataTake(QuestNode owner, string tag) : base(owner, tag)
           {
               BundleToTake = new BundleGraph(null, string.Empty, Vector2Int.zero);
               color = LBSSettings.Instance.view.colorTake;
           }
        }
        [Serializable]
        public class DataRead : BaseQuestNodeData
        {
            [JsonRequired] public BundleGraph BundleToRead;
            public DataRead(QuestNode owner, string tag) : base(owner, tag)
            {
                BundleToRead = new BundleGraph(null, string.Empty, Vector2Int.zero);
                color = LBSSettings.Instance.view.colorRead;
            }
        }
        [Serializable]
        public class DataExchange : BaseQuestNodeData
        {
            [JsonRequired] public BundleType BundleGiveType;
            [SerializeField, JsonRequired] public int requiredAmount = 1;
            /// <summary>
            /// Receive guid must be set from editor panel
            /// </summary>a
            [JsonRequired] public BundleType BundleReceiveType;
            [SerializeField, JsonRequired] public int receiveAmount = 1;
            public DataExchange(QuestNode owner, string tag) : base(owner, tag)
            {
                color = LBSSettings.Instance.view.colorExchange;
            }
        }
        [Serializable]
        public class DataGive : BaseQuestNodeData
        {
            [JsonRequired] public BundleGraph BundleGive;
            /// <summary>
            /// Character to give to 
            /// </summary>
            [JsonRequired] public BundleGraph BundleGiveTo;
            public DataGive(QuestNode owner, string tag) : base(owner, tag)
            {
                BundleGive = new BundleGraph(null, string.Empty, Vector2Int.zero);
                BundleGiveTo = new BundleGraph(null, string.Empty, Vector2Int.zero);
                color = LBSSettings.Instance.view.colorGive;
            }
        }
        [Serializable]
        public class DataReport : BaseQuestNodeData
        {
            /// <summary>
            /// Character to report to
            /// </summary>
            [JsonRequired] public BundleGraph BundleReportTo;
           public DataReport(QuestNode owner, string tag) : base(owner, tag)
           {
               BundleReportTo = new BundleGraph(null,string.Empty, Vector2Int.zero);
               color = LBSSettings.Instance.view.colorReport;
           }
        }
        [Serializable]
        public class DataGather : BaseQuestNodeData
        {
            /// <summary>
            /// material that must be gathered
            /// </summary>
            [JsonRequired] public BundleType BundleGatherType;
            [SerializeField, JsonRequired] public int gatherAmount;
          public DataGather(QuestNode owner, string tag) : base(owner, tag)
          {
          }
        }
        [Serializable]
        public class DataSpy : BaseQuestNodeData
        {
            [JsonRequired] public BundleGraph BundleToSpy;
            [SerializeField, JsonRequired] public float spyTime = 5f;
            [SerializeField, JsonRequired] public bool resetTimeOnExit = true;
          public DataSpy(QuestNode owner, string tag) : base(owner, tag)
          {
              BundleToSpy = new BundleGraph(null, string.Empty, Vector2Int.zero);
              color = LBSSettings.Instance.view.colorSpy;
          }
        }
        [Serializable]
        public class DataCapture : BaseQuestNodeData
        {
            [SerializeField, JsonRequired] public float captureTime = 5f;
            [SerializeField, JsonRequired] public bool resetTimeOnExit = true;

            public DataCapture(QuestNode owner, string tag) : base(owner, tag)
            {
            }
        }
        [Serializable]
        public class DataListen : BaseQuestNodeData
        {
            /// <summary>
            /// Character or objects that gets listened to
            /// </summary>
            [JsonRequired] public BundleGraph BundleListenTo;

            public DataListen(QuestNode owner, string tag) : base(owner, tag)
            {
                BundleListenTo = new BundleGraph(null, string.Empty, Vector2Int.zero);
                color = LBSSettings.Instance.view.colorListen;
            }
        }

        

        #endregion

} 