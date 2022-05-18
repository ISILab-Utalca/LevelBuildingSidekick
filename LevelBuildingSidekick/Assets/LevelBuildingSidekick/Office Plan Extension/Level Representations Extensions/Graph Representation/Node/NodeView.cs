using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick;
using UnityEditor;

public class NodeView: View
{
    public Texture2D circle;

    public NodeView(Controller controller) : base(controller)
    {
    }

    public override void Display()
    {
        Draw();
    }

    public override void Draw()
    {

        //Debug.Log("Node View");
        var data = Controller.Data as NodeData;

        var pos = data.Position;
        var size = 2 * data.Radius * Vector2.one;

        Rect rect = new Rect(pos, size);

        Texture2D texture = new Texture2D(1, 1);

        GUILayout.BeginArea(rect);
        GUI.DrawTexture(new Rect(Vector2.zero, Vector2.one*2*data.Radius), data.Sprite, ScaleMode.StretchToFill);
        GUI.contentColor = Color.black;
        GUI.Label(new Rect(Vector2.one*data.Radius/2, Vector2.one * data.Radius),data.label);
        GUILayout.EndArea();
    }
}
