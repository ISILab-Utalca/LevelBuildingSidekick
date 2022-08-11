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
    public class LBSNodeData : Data
    {
        public RoomCharacteristics room;
        public int x;
        public int y;
        public int radius;
        public string label = "";

        [JsonIgnore] 
        public Texture2D sprite { get => Resources.Load("Textures/Circle") as Texture2D; } // -> static?

        [JsonIgnore]
        public override Type ControllerType => typeof(LBSNodeController);

        public LBSNodeData()
        {

        }

        public LBSNodeData(string label, Vector2 position, int radius)
        {
            this.label = label;
            x = (int)position.x;
            y = (int)position.y;
            this.radius = radius;
        }

    }
}

public enum ProportionType
{
    RATIO,
    SIZE
}

