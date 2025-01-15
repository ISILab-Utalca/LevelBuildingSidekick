using System;
using System.Collections;
using System.Collections.Generic;
using ISILab.LBS.Components;
using ISILab.LBS.Modules;
using Newtonsoft.Json;
using UnityEngine;

namespace ISILab.LBS.Components
{
    [System.Serializable]
    public class QuestNode : ICloneable
    {

        #region FIELD
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
        #endregion

        [SerializeField, JsonRequired, SerializeReference]
        private QuestTarget target;

        private QuestGraph graph;

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
        #endregion

        #region CONSTRUCTOR
        QuestNode() { }

        public QuestNode(string id, Vector2 position, string action, QuestGraph graph)
        {
            this.id = id;
            x = (int)position.x;
            y = (int)position.y;
            this.questAction = action;
            this.graph = graph;
            target = new QuestTarget();
        }
        #endregion

        public object Clone()
        {
            var node = new QuestNode(ID, Position, QuestAction, graph);

            node.target = target.Clone() as QuestTarget;

            return node;
        }
    }

    [System.Serializable]
    public class QuestTarget : ICloneable
    {
        [SerializeField, JsonRequired, SerializeReference]
        private Rect rect;
        [SerializeField, JsonRequired, SerializeReference]
        private List<LBSTag> tags = new List<LBSTag>();

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