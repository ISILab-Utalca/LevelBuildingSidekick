using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LevelBuildingSidekick;
using System;

namespace LevelBuildingSidekick.Graph
{
    public class GraphView : LevelRepresentationView
    {
        //Vector2 scrollPos;
        Vector2 scrollPosition;
        public GraphView(Controller controller) : base(controller)
        {
        }

        public override void Draw2D()
        {
            var controller = Controller as GraphController;

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            //lastElement = controller.FartherPosition();
            //var size = lastElement + offset;

            var r = GUILayoutUtility.GetRect(controller.Size.x, controller.Size.y);
            GUI.DrawTexture(r, new Texture2D(1, 1));
            //Debug.Log("Graph View");

            foreach (NodeController n in controller.Nodes)
            {
                n.View.Draw2D();
            }
            foreach (EdgeController e in controller.Edges)
            {
                e.View.Draw2D();
            }
            base.Draw2D();
            GUILayout.EndScrollView();

        }

        public override void DrawEditor()
        {
            var controller = Controller as GraphController;

            controller.Size = EditorGUILayout.Vector2IntField("Size", controller.Size);
        }
    }
}


