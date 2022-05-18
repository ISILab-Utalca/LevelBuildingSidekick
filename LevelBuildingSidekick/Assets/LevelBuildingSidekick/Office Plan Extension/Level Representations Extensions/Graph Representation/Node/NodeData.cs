using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick;
using System;

[System.Serializable]
[CreateAssetMenu(menuName = ("LevelBuildingSidekick/Level Represetations/Graph Representation/Node"))]
public class NodeData : Data
{
    public string label;

    public Vector2 Position { get; set ; }
    public float Radius { get => 80;} // -> static?
    public Texture2D Sprite { get => Resources.Load("Textures/Circle") as Texture2D;} // -> static?


    public override Type ControllerType => typeof(NodeController);
}
