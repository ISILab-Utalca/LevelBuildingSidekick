using System;
using System.Collections.Generic;
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
        private List<GraphNode> from = new();

        [SerializeField, SerializeReference, JsonRequired]
        private GraphNode to;
        #endregion
        
        #region PROPERTIES
        
        [JsonIgnore]
        public List<GraphNode> From
        {
            get => from;
            set => from = value;
        }

        [JsonIgnore]
        public GraphNode To
        {
            get => to;
            set => to = value;
        }
        #endregion

        #region CONSTRUCTORS
        public QuestEdge()
        {
        }

        public QuestEdge(GraphNode from, GraphNode to)
        {
            this.from.Add(from);
            this.to = to;
        }

        #endregion

        #region METHODS
        public object Clone()
        {
            return new QuestEdge(
                CloneRefs.Get(from) as GraphNode,
                CloneRefs.Get(to) as GraphNode
            );
        }


        public void AddFrom(GraphNode node)
        {
            if(from.Contains(node)) return;
            from.Add(node);
        }


        public void RemoveFrom(GraphNode node)
        {
            if(!from.Contains(node)) return;
            from.Remove(node);
        }
        
        #endregion
    }
}
