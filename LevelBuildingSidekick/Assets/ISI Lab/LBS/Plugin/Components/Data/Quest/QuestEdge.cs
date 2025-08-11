using System;
using System.Collections.Generic;
using ISILab.LBS.Components;
using Newtonsoft.Json;
using UnityEngine;

namespace ISILab.LBS.Modules
{
    public enum ConnectionType
    {
        Single,  // default
        Or,      // multiple possible incoming edges
        And      // multiple required incoming edges
    }

    [Serializable]
    public class QuestEdge : ICloneable
    {
        #region FIELDS
        
        [SerializeField, JsonRequired]
        private ConnectionType connectionType = ConnectionType.Single;
        
        [SerializeField, SerializeReference, JsonRequired]
        private List<QuestNode> from = new();

        [SerializeField, SerializeReference, JsonRequired]
        private QuestNode to;
        #endregion
        
        #region PROPERTIES
        
        [JsonIgnore]
        public ConnectionType EdgeType => connectionType;

        [JsonIgnore]
        public List<QuestNode> From
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

        public QuestEdge(QuestNode from, QuestNode to, ConnectionType connectionType = ConnectionType.Single)
        {
            this.from.Add(from);
            this.to = to;
            this.connectionType = connectionType;
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


        public void AddFrom(QuestNode node)
        {
            if(connectionType == ConnectionType.Single) return;
            from.Add(node);
        }


        public void RemoveFrom(QuestNode node)
        {
            if(from.Contains(node)) from.Remove(node);
        }

        public void SetConnectionType(ConnectionType newConnectionType)
        {
            connectionType = newConnectionType;
        }
        #endregion
    }
}
