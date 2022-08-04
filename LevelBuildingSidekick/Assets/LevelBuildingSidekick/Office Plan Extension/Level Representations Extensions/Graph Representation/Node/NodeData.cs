using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditorInternal;
using LevelBuildingSidekick;
using System;
using LevelBuildingSidekick.Schema;
using Newtonsoft.Json;

namespace LevelBuildingSidekick.Graph
{
    [System.Serializable]
    //[CreateAssetMenu(menuName = ("LevelBuildingSidekick/Level Represetation/Graph Representation/Node"))]
    public class NodeData : Data
    {
        public RoomCharacteristics room;
        public int x;
        public int y;
        public int radius;

        [JsonIgnore] 
        public Texture2D sprite { get => Resources.Load("Textures/Circle") as Texture2D; } // -> static?

        [JsonIgnore]
        public override Type ControllerType => typeof(NodeController);

    }
}

public enum ProportionType
{
    RATIO,
    SIZE
}

