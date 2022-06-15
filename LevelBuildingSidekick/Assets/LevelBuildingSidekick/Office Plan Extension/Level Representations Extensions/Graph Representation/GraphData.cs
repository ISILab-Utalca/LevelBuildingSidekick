using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick;
using System;

namespace LevelBuildingSidekick.Graph
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "LevelBuildingSidekick/Level Represetation/Graph Representation/Level")]
    public class GraphData : LevelRepresentationData
    {
        [SerializeField]
        public List<NodeData> nodes = new List<NodeData>();
        [SerializeField]
        public List<EdgeData> edges = new List<EdgeData>();
        public Vector2Int size;

        public override Type ControllerType => typeof(GraphController);
    }
}

