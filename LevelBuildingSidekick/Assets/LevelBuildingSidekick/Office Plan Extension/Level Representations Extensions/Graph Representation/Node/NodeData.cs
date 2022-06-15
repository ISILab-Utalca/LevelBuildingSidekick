using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditorInternal;
using LevelBuildingSidekick;
using System;
using LevelBuildingSidekick.Blueprint;

namespace LevelBuildingSidekick.Graph
{
    [System.Serializable]
    [CreateAssetMenu(menuName = ("LevelBuildingSidekick/Level Represetation/Graph Representation/Node"))]
    public class NodeData : Data
    {
        public RoomCharacteristics room;
        public Vector2Int position;
        public int radius { get => 64; } // -> static?
        public Texture2D sprite { get => Resources.Load("Textures/Circle") as Texture2D; } // -> static?

        public override Type ControllerType => typeof(NodeController);

    }
}

public enum ProportionType
{
    NONE,
    RATIO,
    SIZE
}

