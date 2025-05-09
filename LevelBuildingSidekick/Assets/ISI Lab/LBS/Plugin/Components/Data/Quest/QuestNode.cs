using System;
using System.Collections;
using System.Collections.Generic;
using ISILab.LBS.Components;
using ISILab.LBS.Modules;
using Newtonsoft.Json;
using UnityEngine;

namespace ISILab.LBS.Components
{

    public enum NodeType
    {
        start, middle, goal
    }

    public enum QuestState
    {
        blocked, active, completed, failed
    }
    

[System.Serializable]
    public class QuestNode : ICloneable
    {

        #region FIELD

        [SerializeField] 
        private string nodeDataJson;
        
        [SerializeField, SerializeReference][JsonRequired]
        private BaseQuestNodeData nodeData;
        
        [SerializeField, HideInInspector, JsonRequired]
        private int x, y;

        [SerializeField, JsonRequired]
        private string id = ""; // "ID" or "name"

        [SerializeField, JsonRequired]
        private string questAction = "";

        [SerializeField, JsonRequired]
        private bool grammarCheck;

        [SerializeField, JsonRequired]
        private bool mapCheck;
        
        [SerializeField, JsonRequired]
        private NodeType nodeType;
        
        [SerializeField, JsonRequired]
        private QuestState questState = Components.QuestState.blocked;
        
        [SerializeField, JsonRequired]
        private bool valid;
        
        #endregion

        [SerializeField, JsonRequired, SerializeReference]
        private QuestTarget target;

        [SerializeField, JsonRequired, SerializeReference]
        private QuestGraph graph;
        
        #region PROPERTIES
        [JsonIgnore]
        public BaseQuestNodeData NodeData
        {
            get => nodeData;
            set => nodeData = value;
        }
        
        [JsonIgnore]
        public QuestGraph Graph
        {
            get => graph;
            set => graph = value;
        }
        
        [JsonIgnore]
        public Vector2Int Position
        {
            get => new Vector2Int(x, y);

            set
            {
                x = value.x;
                y = value.y;
            }
        }

        [JsonIgnore]
        public string ID
        {
            get => id;
            set
            {
                id = value;
            }
        }


        [JsonIgnore]
        public string QuestAction
        {
            get => questAction;
            set
            {
                questAction = value;
            }
        }

        [JsonIgnore]
        public bool GrammarCheck
        {
            get => grammarCheck;
            set => grammarCheck = value;
        }

        [JsonIgnore]
        public bool MapCheck
        {
            get => mapCheck;
            set => mapCheck = value;
        }

        [JsonIgnore]
        public QuestTarget Target
        {
            get => target;
            set => target = value;
        }
        
        
        [JsonIgnore]
        public bool Valid
        {
            get => valid;
            set => valid = value;
        }
        
        [JsonIgnore]
        public NodeType NodeType
        {
            get => nodeType;
            set => nodeType = value;
        }


        [JsonIgnore]
        public QuestState QuestState { get; set; }


        #endregion

        #region CONSTRUCTOR
        QuestNode() { }

        public QuestNode(string id, Vector2 position, string action, QuestGraph graph, bool grammarCheck = false)
        {
            this.id = id;
            x = (int)position.x;
            y = (int)position.y;
            questAction = action;
            
            InstanceDataByAction(action);
            this.graph = graph;
            this.grammarCheck = grammarCheck;
            target = new QuestTarget();
            
            LoadNodeFromJson();
        }

        private void InstanceDataByAction(string action)
        {
            nodeData = QuestNodeDataFactory.CreateByTag(action, this);
            SaveNodeAsJson();
        }

        #endregion

        public bool HasEdges()
        {
            return graph != null && graph.HasRequiredConnection(this);
        }
        
        public object Clone()
        {
            var node = new QuestNode(ID, Position, QuestAction, graph, GrammarCheck)
            {
                target = target.Clone() as QuestTarget
            };

            return node;
        }
        
        /// <summary>
        /// Saves the changes from the dataJson whenever the SetGoal is called (whenever it is changed)
        /// </summary>
        public void SaveNodeAsJson()
        {
            return;
            nodeDataJson =  JsonUtility.ToJson(nodeData);
        }

        /// <summary>
        /// Load from json as it is easier to load polyformism
        /// </summary>
        public void LoadNodeFromJson()
        {
            return;
            if (string.IsNullOrEmpty(nodeDataJson)) return;
            nodeData = JsonUtility.FromJson<BaseQuestNodeData>(nodeDataJson);
   
        }
    }

    [System.Serializable]
    public class QuestTarget : ICloneable
    {
        [SerializeField, JsonRequired, SerializeReference]
        private Rect rect;
        [SerializeField, JsonRequired, SerializeReference]
        private List<LBSTag> tags = new();

        [JsonIgnore]
        public Rect Rect
        {
            get => rect;
            set => rect = value;
        }

        [JsonIgnore]
        public List<LBSTag> Tags => tags;

        public QuestTarget() { }

        public object Clone()
        {
            var target = new QuestTarget();
            target.tags = new List<LBSTag>(tags);
            target.rect = rect;
            return target;
        }
        
        
    }
}