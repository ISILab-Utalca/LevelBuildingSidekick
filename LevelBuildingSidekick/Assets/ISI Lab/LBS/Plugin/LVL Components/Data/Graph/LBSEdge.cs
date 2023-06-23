using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS;
using System;
using Newtonsoft.Json;

namespace LBS.Components.Graph
{
    [System.Serializable]
    public class LBSEdge : ICloneable
    {
        #region FIELDS

        [SerializeField, JsonRequired, SerializeReference]
        private LBSNode firstNode;

        [SerializeField, JsonRequired, SerializeReference]
        private LBSNode secondNode;

        /// <summary>
        /// The direction of the edge.
        /// </summary>
        [SerializeField, JsonRequired]
        EdgeDirection direction;

        [JsonIgnore]
        public float thikness = 5; // -> static (??)

        #endregion

        #region PROPERTIES

        [HideInInspector, JsonIgnore]
        public EdgeDirection Direction => direction;

        [HideInInspector, JsonIgnore]
        public LBSNode FirstNode => firstNode;

        [HideInInspector, JsonIgnore]
        public LBSNode SecondNode => secondNode;

        #endregion

        #region CONSTRUCTOR

        /// <summary>
        /// Empty constructor, necessary for serialization with json.
        /// </summary>
        public LBSEdge() { }

        /// <summary>
        /// Constructor for the LBSEdgeData class, which creates an edge between two nodes.
        /// </summary>
        /// <param name="n1">First node data object in the edge</param>
        /// <param name="n2">Second node data object in the edge</param>
        public LBSEdge(LBSNode n1, LBSNode n2)
        {
            firstNode = n1;
            secondNode = n2;
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Determines whether this edge contains the given node.
        /// </summary>
        /// <param name="nodeID"> The identifier of the node to check for.</param>
        /// <returns> True if the edge contains the given node, false otherwise.</returns>
        public bool Contains(LBSNode node)
        {
            return firstNode.Equals(node) || secondNode.Equals(node);
        }

        public object Clone()
        {
            return new LBSEdge(FirstNode.Clone() as LBSNode, SecondNode.Clone() as LBSNode);
        }

        #endregion
    }

    /// <summary>
    ///  Enumeration of posibles edges directions.
    /// </summary>
    [System.Serializable]
    public enum EdgeDirection
    {
        BIDIRECTIONAL,
        FORWARD,
        BACKWARDS
    }
}


