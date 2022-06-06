using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LevelBuildingSidekick.Graph
{
    public class NodeView : View
    {
        public Texture2D circle;
        Vector2 scrollPos;

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

            var pos = data.position;
            var size = 2 * data.Radius * Vector2.one;

            Rect rect = new Rect(pos, size);
            Rect innerRect = new Rect(pos + (size * 0.2f), size * 0.6f); //0.7 == sqrt(2)/2, side of square inside circle inside square.
                                                                         //should be 0,15 but image has blank space

            GUI.DrawTexture(rect, data.Sprite, ScaleMode.StretchToFill);

            GUILayout.BeginArea(innerRect);
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            //GUILayout.Button(data.Sprite);
            //Rect rt = GUILayoutUtility.GetAspectRect(1);
            //rt.position = Vector2.zero;
            //rt.size = Vector2.one * 2 * data.Radius;
            GUI.contentColor = Color.black;
            GUILayout.Label(data.label);
            GUILayout.Label("Width: " + data.width.x + " - " + data.width.y);
            GUILayout.Label("Width: " + data.height.x + " - " + data.height.y);
            GUILayout.Label("Aspect Ratio: " + data.aspectRatio.x + " : " + data.aspectRatio.y);
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }
    }
}


