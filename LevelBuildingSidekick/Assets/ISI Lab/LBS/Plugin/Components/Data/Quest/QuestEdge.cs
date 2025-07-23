using System;
using ISILab.LBS.Components;
using Newtonsoft.Json;
using UnityEngine;

namespace ISILab.LBS.Modules
{
    [Serializable]
    public class QuestEdge : ICloneable
    {
        #region FIELDS
        [SerializeField, SerializeReference, JsonRequired]
        private QuestNode from;

        [SerializeField, SerializeReference, JsonRequired]
        private QuestNode to;
        #endregion
        #region PROPERTIES
        [JsonIgnore]
        public QuestNode From
        {
            get => from;
            set => from = value;
        }

        [JsonIgnore]
        public QuestNode To
        {
            get => to;
            set => to = value;
        }
        #endregion

        #region CONSTRUCTORS
        public QuestEdge()
        {
        }

        public QuestEdge(QuestNode from, QuestNode to)
        {
            this.from = from;
            this.to = to;
        }
        #endregion

        #region METHODS
        public object Clone()
        {
            return new QuestEdge(
                CloneRefs.Get(from) as QuestNode,
                CloneRefs.Get(to) as QuestNode
            );
        }
        #endregion
    }
}
