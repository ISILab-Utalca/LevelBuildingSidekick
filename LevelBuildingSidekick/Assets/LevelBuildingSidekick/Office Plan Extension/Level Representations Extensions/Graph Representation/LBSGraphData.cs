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
    public class LBSGraphData : LevelRepresentationData
    {
        [SerializeField]
        public List<LBSNodeData> nodes = new List<LBSNodeData>();
        [SerializeField]
        public List<EdgeData> edges = new List<EdgeData>();
        public int cellSize = 32; //Add to View parameters (??)


        [JsonIgnore]
        public override Type ControllerType => typeof(LBSGraphController);
    }
}

