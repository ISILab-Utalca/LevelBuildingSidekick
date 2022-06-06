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
        Vector2 scrollPos;
        Vector2 lastElement;
        Vector2 offset;
        public GraphView(Controller controller) : base(controller)
        {
            offset = new Vector2Int(256, 256);
        }

        public override void Draw()
        {
            var controller = Controller as GraphController;

            scrollPos = GUILayout.BeginScrollView(scrollPos);

            lastElement = controller.FartherPosition();
            var size = lastElement + offset;

            var r = GUILayoutUtility.GetRect(size.x, size.y);
            GUI.DrawTexture(r, new Texture2D(1, 1));
            base.Draw();
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
                n.View.Display();
            }
            foreach (EdgeController e in controller.Edges)
            {
                e.View.Display();
            }
            GUILayout.EndScrollView();

        }
    }
}


