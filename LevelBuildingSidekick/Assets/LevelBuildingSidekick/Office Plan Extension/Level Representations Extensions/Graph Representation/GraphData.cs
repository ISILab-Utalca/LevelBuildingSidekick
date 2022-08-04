using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick;
using System;
using Newtonsoft.Json;

namespace LevelBuildingSidekick.Graph
{
    [System.Serializable]
    //[CreateAssetMenu(menuName = "LevelBuildingSidekick/Level Represetation/Graph Representation/Level")]
    public class GraphData : LevelRepresentationData
    {
        [SerializeField]
        public List<NodeData> nodes = new List<NodeData>();
        [SerializeField]
        [JsonIgnore]
        public List<EdgeData> edges = new List<EdgeData>();
        public int cellSize = 64; //Add to View parameters (??)


        [JsonIgnore]
        public override Type ControllerType => typeof(GraphController);
    }
}

