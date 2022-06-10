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
        NodeWindow nodeInspector;
        //Vector2 scrollPos;
        Vector2 lastElement;
        Vector2 offset;
        public GraphView(Controller controller) : base(controller)
        {
            offset = new Vector2Int(256, 256);
        }

        public override void Draw2D()
        {
            var controller = Controller as GraphController;

            controller.scrollPosition = GUILayout.BeginScrollView(controller.scrollPosition);

            lastElement = controller.FartherPosition();
            var size = lastElement + offset;

            var r = GUILayoutUtility.GetRect(size.x, size.y);
            GUI.DrawTexture(r, new Texture2D(1, 1));
            base.Draw2D();
            //Debug.Log("Graph View");

            if (controller.SelectedNode != null)
            {
                if (nodeInspector == null)
                {
                    nodeInspector = EditorWindow.GetWindow<NodeWindow>(new Type[] { typeof(StepWindow) });
                }
                nodeInspector.controller = controller.SelectedNode;
                /*if(nodeInspector.Data == null)// || !nodeInspector.Data.Equals(controller.SelectedNode.Data))
                {
                    nodeInspector.Data = controller.SelectedNode.Data as NodeData;
                }*/
            }


            foreach (NodeController n in controller.Nodes)
            {
                n.View.Draw2D();
            }
            foreach (EdgeController e in controller.Edges)
            {
                e.View.Draw2D();
            }
            GUILayout.EndScrollView();

        }
    }
}


