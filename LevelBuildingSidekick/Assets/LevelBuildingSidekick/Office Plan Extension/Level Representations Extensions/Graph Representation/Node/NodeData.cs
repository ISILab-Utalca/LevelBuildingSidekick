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
    [CreateAssetMenu(menuName = ("LevelBuildingSidekick/Level Represetations/Graph Representation/Node"))]
    public class NodeData : Data
    {
        RoomCharacteristics room;
        public Vector2Int position;
        public int Radius { get => 64; } // -> static?
        public Texture2D Sprite { get => Resources.Load("Textures/Circle") as Texture2D; } // -> static?


        public override Type ControllerType => typeof(NodeController);

    }
}

public enum ProportionType
{
    RATIO,
    SIZE
}

