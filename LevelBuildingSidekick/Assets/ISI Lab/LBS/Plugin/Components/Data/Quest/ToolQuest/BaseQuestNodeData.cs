using System;
using System.Collections.Generic;
using System.Linq;
using LBS.Bundles;
using Newtonsoft.Json;
using UnityEngine;

namespace ISILab.LBS.Components
{
    
        [AttributeUsage(AttributeTargets.Class, Inherited = false)]
        public class QuestTagAttribute : Attribute
        {
            public string Tag { get; }

            public QuestTagAttribute(string tag)
            {
                Tag = tag;
            }
        }

    
        /// <summary>
        /// Custom attribute to specify action tag and required data types.
        /// </summary>
        [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
        public class QuestNodeActionTag : Attribute
        {
            private readonly string _tag;
            public string Tag => _tag;

            public QuestNodeActionTag(string tag)
            {
                _tag = tag;
            }

            // Static mapping of tags to required data types
            public static readonly Dictionary<string, Type> TagDataTypes = new()
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
        }

        /// <summary>
        /// Factory to create QuestNodeData based on tags.
        /// </summary>
        public static class QuestNodeDataFactory
        {
            public static BaseQuestNodeData CreateByTag(string tag, QuestNode owner)
            {
                if (!QuestNodeActionTag.TagDataTypes.TryGetValue(tag, out var dataClass))
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
            public Vector2Int position = Vector2Int.zero;
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
                _tag = data._tag;
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
            [SerializeField, JsonRequired] public float size = 1;

            public DataGoto(QuestNode owner, string tag) : base(owner, tag)
            {
            }
        }
        [Serializable]
        public class DataExplore : BaseQuestNodeData
        {
            [SerializeField, JsonRequired] public int subdivisions = 4;
            [SerializeField, JsonRequired] public float size = 1;
            
            // if find random position is true, then upon generation a random position is created and that's what the 
            // player must trigger
            [SerializeField, JsonRequired] public bool findRandomPosition = false;

            public DataExplore(QuestNode owner, string tag) : base(owner, tag)
            {
            }
        }
        [Serializable]
        public class DataKill : BaseQuestNodeData
        {
            [SerializeField, JsonRequired] public int counter = 1;
            [SerializeField, JsonRequired] public string bundleKillGuid;

            public DataKill(QuestNode owner, string tag) : base(owner, tag)
            {
            }
        }
        [Serializable]
        public class DataStealth : BaseQuestNodeData
        {
            [SerializeField, JsonRequired] public Vector2Int target = Vector2Int.zero;
            [SerializeField, JsonRequired] public float size = 1;
            /// <summary>
            /// Objects with a default trigger that will stop catch the player
            /// </summary>
            [SerializeField, JsonRequired] public List<string> bundleObserversGuids = new();

            public DataStealth(QuestNode owner, string tag) : base(owner, tag)
            {
            }
        }
        [Serializable]
        public class DataTake : BaseQuestNodeData
        {
            [SerializeField, JsonRequired] public string bundleToTakeGuid;
           public DataTake(QuestNode owner, string tag) : base(owner, tag)
           {
           }
        }
        [Serializable]
        public class DataRead : BaseQuestNodeData
        {
            [SerializeField, JsonRequired] public string objectToReadGuid;
            public DataRead(QuestNode owner, string tag) : base(owner, tag)
            {
            }
        }
        [Serializable]
        public class DataExchange : BaseQuestNodeData
        {
            [SerializeField, JsonRequired] public string bundleGiveGuid;
            [SerializeField, JsonRequired] public int requiredAmount = 1;
            /// <summary>
            /// Receive guid must be set from editor panel
            /// </summary>
            [SerializeField, JsonRequired] public string bundleReceiveGuid;
            [SerializeField, JsonRequired] public int receiveAmount = 1;
            public DataExchange(QuestNode owner, string tag) : base(owner, tag)
            {
            }
        }
        [Serializable]
        public class DataGive : BaseQuestNodeData
        {
            [SerializeField, JsonRequired] public string bundleGiveGuid;
            /// <summary>
            /// Character to give to 
            /// </summary>
            [SerializeField, JsonRequired] public string bundleToGiveGuid;
            public DataGive(QuestNode owner, string tag) : base(owner, tag)
            {
            }
        }
        [Serializable]
        public class DataReport : BaseQuestNodeData
        {
            [SerializeField, JsonRequired] public float areaSize = 1;
            /// <summary>
            /// Character to report to
            /// </summary>
            [SerializeField, JsonRequired] public string bundleToReport;
           public DataReport(QuestNode owner, string tag) : base(owner, tag)
           {
           }
        }
        [Serializable]
        public class DataGather : BaseQuestNodeData
        {
            /// <summary>
            /// material that must be gathered
            /// </summary>
            [SerializeField, JsonRequired] public string material;
            [SerializeField, JsonRequired] public int requiredAmount;
          public DataGather(QuestNode owner, string tag) : base(owner, tag)
          {
          }
        }
        [Serializable]
        public class DataSpy : BaseQuestNodeData
        {
            [SerializeField, JsonRequired] public string characterToSpy;
            [SerializeField, JsonRequired] public float areaSize = 1;
            [SerializeField, JsonRequired] public float spyTime = 5f;
            [SerializeField, JsonRequired] public bool resetTimeOnExit = true;
          public DataSpy(QuestNode owner, string tag) : base(owner, tag)
          {
          }
        }
        [Serializable]
        public class DataCapture : BaseQuestNodeData
        {
            [SerializeField, JsonRequired] public float areaSize = 1;
            [SerializeField, JsonRequired] public float captureTime = 5f;
            [SerializeField, JsonRequired] public bool resetTimeOnExit = true;

            public DataCapture(QuestNode owner, string tag) : base(owner, tag)
            {
            }
        }
        [Serializable]
        public class DataListen : BaseQuestNodeData
        {
            [SerializeField, JsonRequired] public float size = 1;
            /// <summary>
            /// Character or objects that gets listened to
            /// </summary>
            [SerializeField, JsonRequired] public string bundleToListenFGuid;

            public DataListen(QuestNode owner, string tag) : base(owner, tag)
            {
            }
        }

        

        #endregion

} 