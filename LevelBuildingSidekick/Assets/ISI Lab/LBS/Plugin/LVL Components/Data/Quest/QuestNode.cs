using ISILab.LBS.Components;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
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

        #region PROPERTIES
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

        public QuestNode(string id, Vector2 position, string action)
        {
            this.id = id;
            x = (int)position.x;
            y = (int)position.y;
            this.questAction = action;
            target = new QuestTarget();
        }
        #endregion

        public object Clone()
        {
            var node = new QuestNode(ID, Position, QuestAction);

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
        private List<LBSIdentifier> tags = new List<LBSIdentifier>();

        [JsonIgnore]
        public Rect Rect
        {
            get => rect;
            set => rect = value;
        }

        [JsonIgnore]
        public List<LBSIdentifier> Tags => tags;

        public QuestTarget() { }

        public object Clone()
        {
            var target = new QuestTarget();
            target.tags = new List<LBSIdentifier>(tags);
            target.rect = rect;
            return target;
        }
    }
}