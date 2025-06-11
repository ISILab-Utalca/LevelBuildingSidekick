using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace ISILab.LBS.Components
{
    
    /// <summary>
    /// Saves the bundle guid and the position in the graph to get in the scene
    /// </summary>
    public struct bundleGraph
    {
        public bundleGraph(string guid, Vector2Int position)
        {
            this.guid = guid;
            this.position = position;
        }

        public bool Valid()
        {
            return guid != string.Empty;
        } 
        
        public string guid;
        public Vector2Int position;
    }
    
    /// <summary>
    /// Saves the bundle type
    /// </summary>
    public struct bundleType
    {
        public string guid;
    }
    
        /// <summary>
        /// Factory to create QuestNodeData based on actions.
        /// </summary>
        public static class QuestNodeDataFactory
        {
            public static Dictionary<string, Type> TagDataTypes = new()
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
            private QuestNode _owner;
            
            [SerializeField, JsonRequired]
            private string _tag;
          
            [SerializeField, JsonRequired] 
            public Vector2Int _position = Vector2Int.zero;
           
            [SerializeField, JsonRequired] 
            public float _size = 1;
            
            #endregion

            #region PROPERTIES
            public QuestNode Owner => _owner;
            public string Tag => _tag;
            #endregion

            public BaseQuestNodeData(QuestNode owner, string tag)
            {
                _owner = owner ?? throw new ArgumentNullException(nameof(owner));
                _tag = tag;
            }

            public void Clone(BaseQuestNodeData data)
            {
                _owner = data._owner;
                _tag = data._tag;
                _position = data._position;
                _size = data._size;
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
            [SerializeField, JsonRequired] public List<bundleGraph> bundlesToKill = new();

            public DataKill(QuestNode owner, string tag) : base(owner, tag)
            {
            }
        }
        [Serializable]
        public class DataStealth : BaseQuestNodeData
        {
            [SerializeField, JsonRequired] public Vector2Int objective = Vector2Int.zero;
            /// <summary>
            /// Objects with a default trigger that will stop catch the player
            /// </summary>
            [SerializeField, JsonRequired] public List<bundleGraph> bundlesObservers = new();

            public DataStealth(QuestNode owner, string tag) : base(owner, tag)
            {
            }
        }
        [Serializable]
        public class DataTake : BaseQuestNodeData
        {
            [SerializeField, JsonRequired] public bundleGraph bundleToTake;
           public DataTake(QuestNode owner, string tag) : base(owner, tag)
           {
               bundleToTake = new bundleGraph(string.Empty, Vector2Int.zero);
           }
        }
        [Serializable]
        public class DataRead : BaseQuestNodeData
        {
            [SerializeField, JsonRequired] public bundleGraph bundleToRead;
            public DataRead(QuestNode owner, string tag) : base(owner, tag)
            {
                bundleToRead = new bundleGraph(string.Empty, Vector2Int.zero);
            }
        }
        [Serializable]
        public class DataExchange : BaseQuestNodeData
        {
            [SerializeField, JsonRequired] public bundleType bundleGiveType;
            [SerializeField, JsonRequired] public int requiredAmount = 1;
            /// <summary>
            /// Receive guid must be set from editor panel
            /// </summary>
            [SerializeField, JsonRequired] public bundleType bundleReceiveType;
            [SerializeField, JsonRequired] public int receiveAmount = 1;
            public DataExchange(QuestNode owner, string tag) : base(owner, tag)
            {
            }
        }
        [Serializable]
        public class DataGive : BaseQuestNodeData
        {
            [SerializeField, JsonRequired] public bundleGraph bundleGive;
            /// <summary>
            /// Character to give to 
            /// </summary>
            [SerializeField, JsonRequired] public bundleGraph bundleGiveTo;
            public DataGive(QuestNode owner, string tag) : base(owner, tag)
            {
                bundleGive = new bundleGraph(string.Empty, Vector2Int.zero);
                bundleGiveTo = new bundleGraph(string.Empty, Vector2Int.zero);
            }
        }
        [Serializable]
        public class DataReport : BaseQuestNodeData
        {
            /// <summary>
            /// Character to report to
            /// </summary>
            [SerializeField, JsonRequired] public bundleGraph bundleReportTo;
           public DataReport(QuestNode owner, string tag) : base(owner, tag)
           {
               bundleReportTo = new bundleGraph(string.Empty, Vector2Int.zero);
           }
        }
        [Serializable]
        public class DataGather : BaseQuestNodeData
        {
            /// <summary>
            /// material that must be gathered
            /// </summary>
            [SerializeField, JsonRequired] public bundleType bundleGatherType;
            [SerializeField, JsonRequired] public int requiredAmount;
          public DataGather(QuestNode owner, string tag) : base(owner, tag)
          {
          }
        }
        [Serializable]
        public class DataSpy : BaseQuestNodeData
        {
            [SerializeField, JsonRequired] public bundleGraph bundleToSpy;
            [SerializeField, JsonRequired] public float spyTime = 5f;
            [SerializeField, JsonRequired] public bool resetTimeOnExit = true;
          public DataSpy(QuestNode owner, string tag) : base(owner, tag)
          {
              bundleToSpy = new bundleGraph(string.Empty, Vector2Int.zero);
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
            [SerializeField, JsonRequired] public bundleGraph bundleListenTo;

            public DataListen(QuestNode owner, string tag) : base(owner, tag)
            {
                bundleListenTo = new bundleGraph(string.Empty, Vector2Int.zero);
            }
        }

        

        #endregion

} 