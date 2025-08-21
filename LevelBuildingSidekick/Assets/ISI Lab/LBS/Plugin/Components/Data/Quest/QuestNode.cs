using System;
using System.Collections.Generic;
using System.Linq;
using ISILab.LBS.Modules;
using Newtonsoft.Json;
using UnityEngine;

namespace ISILab.LBS.Components
{

    public enum NodeType
    {
        Start, Middle, Goal
    }

    public enum QuestState
    {
        Blocked, Active, Completed, Failed
    }
    

[Serializable]
    public class QuestNode : ICloneable
    {
        #region FIELD
        
        [SerializeField, SerializeReference][JsonRequired]
        private BaseQuestNodeData nodeData;
        
        [SerializeField, HideInInspector, JsonRequired]
        private int x, y;

        [SerializeField, JsonRequired]
        private string id = "";

        [SerializeField, JsonRequired]
        private string questAction = "";

        [SerializeField, JsonRequired]
        private bool validValidGrammar;
        
        [SerializeField, JsonRequired]
        private NodeType nodeType;
        
        [SerializeField, JsonRequired]
        private QuestState questState = QuestState.Blocked;
        
        #endregion

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
            get => new(x, y);

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
            set => id = value;
        }


        [JsonIgnore]
        public string QuestAction
        {
            get => questAction;
            set => questAction = value;
        }

        [JsonIgnore]
        public bool ValidGrammar
        {
            get => validValidGrammar;
            set => validValidGrammar = value;
        }
        
        [JsonIgnore]
        public NodeType NodeType
        {
            get => nodeType;
            set => nodeType = value;
        }


        [JsonIgnore]
        public QuestState QuestState { get; set; }
        
        public Rect NodeViewPosition { get; set; }

        #endregion

        #region CONSTRUCTOR
        QuestNode() { }

        public QuestNode(string id, Vector2 position, string action, QuestGraph graph, bool validValidGrammar = false)
        {
            this.id = id;
            x = (int)position.x;
            y = (int)position.y;
            questAction = action;
            
            this.graph = graph;
            this.validValidGrammar = validValidGrammar;
            
            InstanceDataByAction(action);
        }

        private void InstanceDataByAction(string action)
        {
            if (action == string.Empty) return;
            nodeData = QuestNodeDataFactory.CreateByTag(action, this);
        }

        #endregion
        
        public object Clone()
        {
            var node = new QuestNode(ID, Position, QuestAction, graph, ValidGrammar);
            if(NodeData is not null )node.NodeData.Clone(NodeData);
            node.NodeViewPosition = NodeViewPosition;
            return node;
        }
        
        public bool IsValidFrom()
        {
            var found = graph.QuestEdges.FirstOrDefault(e => e.From.Equals(this));
            return found == null;
        }
        
        public bool IsValidTo()
        {
            var found = graph.QuestEdges.FirstOrDefault(e => e.To.Equals(this));
            return found == null;
        }
    }
    
}