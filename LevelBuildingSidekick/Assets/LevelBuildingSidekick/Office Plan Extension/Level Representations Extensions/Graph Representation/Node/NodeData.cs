using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditorInternal;
using LevelBuildingSidekick;
using System;

namespace LevelBuildingSidekick.Graph
{
    [System.Serializable]
    [CreateAssetMenu(menuName = ("LevelBuildingSidekick/Level Represetations/Graph Representation/Node"))]
    public class NodeData : Data
    {
        public string label;
        public Vector2Int width;
        public Vector2Int height;
        public Vector2Int aspectRatio;

        //Should be in children class
        [NonReorderable]
        public List<GameObject> floorTiles;
        //[SerializeField]
        [NonReorderable]
        public List<GameObject> wallTiles;
        //[SerializeField]
        [NonReorderable]
        public List<GameObject> doorTiles;

        public List<string> tags;

        public Vector2Int Position { get; set; }
        public float Radius { get => 64; } // -> static?
        public Texture2D Sprite { get => Resources.Load("Textures/Circle") as Texture2D; } // -> static?


        public override Type ControllerType => typeof(NodeController);

    }
}

