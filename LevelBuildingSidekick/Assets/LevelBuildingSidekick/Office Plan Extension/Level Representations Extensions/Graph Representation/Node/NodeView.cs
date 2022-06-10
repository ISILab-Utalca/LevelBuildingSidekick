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

        public override void Draw2D()
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
            GUILayout.Label("AAAAAAAAAAAAAAAAAa");
            //scrollPos = GUILayout.BeginScrollView(scrollPos);
            //GUILayout.Button(data.Sprite);
            //Rect rt = GUILayoutUtility.GetAspectRect(1);
            //rt.position = Vector2.zero;
            //rt.size = Vector2.one * 2 * data.Radius;
            //GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        public override void DrawEditor()
        {
             NodeController nodeController = Controller as NodeController;
            //Espacio para proximo control
            EditorGUILayout.Space();
            
            nodeController.Label = EditorGUILayout.TextField("Label ", nodeController.Label);
           
            //Espacio para proximo control
            EditorGUILayout.Space();
            nodeController.ProportionType = (ProportionType)EditorGUILayout.EnumPopup("Proportion type", nodeController.ProportionType);

            switch (nodeController.ProportionType)
            {
                case ProportionType.RATIO:
                    nodeController.Ratio = EditorGUILayout.Vector2IntField("Aspect Radio ", nodeController.Ratio);
                    break;
                case ProportionType.SIZE:
                    nodeController.Width = EditorGUILayout.Vector2IntField("Width ", nodeController.Width);
                    nodeController.Height = EditorGUILayout.Vector2IntField("Height", nodeController.Height);
                    break;
            }

        }
    }
}


