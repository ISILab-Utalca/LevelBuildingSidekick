using System;
using ISILab.LBS.Modules;
using Newtonsoft.Json;
using UnityEngine;

namespace ISILab.LBS.Components
{
    
    public enum QuestState
    {
        Blocked, Active, Completed, Failed
    }

    // parent class for actions(ors) and(ands)
    public abstract class GraphNode : ICloneable
    {
        #region FIELDS
        [SerializeField, JsonRequired] 
        protected internal bool validGrammar;

        [SerializeField, HideInInspector, JsonRequired]
        protected int x;

        [SerializeField, HideInInspector, JsonRequired]
        protected int y;

        [SerializeField, JsonRequired, SerializeReference]
        protected QuestGraph graph;
        #endregion
        
        #region PROPERTIES
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
        public bool ValidGrammar
        {
            get => validGrammar;
            set => validGrammar = value;
        }
        
        public Rect NodeViewPosition { get; set; }
        
        #endregion
        public object Clone()
        {
            return null!;
        }

        public abstract override string ToString();
    }

    [Serializable]
    public class OrNode : GraphNode
    {
        public OrNode(Vector2 position, QuestGraph graph)
        {
            x = (int)position.x;
            y = (int)position.y;
            this.graph = graph;
            validGrammar = false;
        }
        
        public new object Clone()
        {
            var node = new OrNode(Position, graph)
            {
                validGrammar = validGrammar
            };
            return node;
        }

        public override string ToString()
        {
            return "Or";
        }
    }

    [Serializable]
    public class AndNode : GraphNode
    {
        public AndNode(Vector2 position, QuestGraph graph)
        {
            x = (int)position.x;
            y = (int)position.y;
            this.graph = graph;
            validGrammar = false;
        }
        public new object Clone()
        {
            var node = new OrNode(Position, graph)
            {
                validGrammar = validGrammar
            };
            return node;
        }

        public override string ToString()
        {
            return "And";
        }
    }
    [Serializable]
    public class QuestNode : GraphNode
    {
        public enum ENodeType
        {
            Start, Middle, Goal
        }
        
        #region FIELD
        
        [SerializeField, SerializeReference][JsonRequired]
        private BaseQuestNodeData nodeData;
        
        [SerializeField, JsonRequired]
        private string id = "";

        [SerializeField, JsonRequired]
        private string questAction = "";
        
        [SerializeField, JsonRequired]
        private ENodeType nodeType;
        
        [SerializeField, JsonRequired]
        private QuestState questState = QuestState.Blocked;
        
        #endregion;
        
        #region PROPERTIES
        [JsonIgnore]
        public BaseQuestNodeData NodeData
        {
            get => nodeData;
            set => nodeData = value;
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
        public ENodeType NodeType
        {
            get => nodeType;
            set => nodeType = value;
        }


        [JsonIgnore]
        public QuestState QuestState { get; set; }

        #endregion

        #region CONSTRUCTOR
        QuestNode() { }

        public QuestNode(string id, Vector2 position, string action, QuestGraph graph)
        {
            this.id = id;
            x = (int)position.x;
            y = (int)position.y;
            questAction = action;
            
            this.graph = graph;
            validGrammar = false;
            
            InstanceDataByAction(action);
        }

        private void InstanceDataByAction(string action)
        {
            if (action == string.Empty) return;
            nodeData = QuestNodeDataFactory.CreateByTag(action, this);
        }

        #endregion
        
        public new object Clone()
        {
            var node = new QuestNode(ID, Position, QuestAction, graph);
            if(NodeData is not null )node.NodeData.Clone(NodeData);
            node.NodeViewPosition = NodeViewPosition;
            node.validGrammar = validGrammar;
            return node;
        }

        public override string ToString()
        {
            return QuestAction;
        }
    }
    
}