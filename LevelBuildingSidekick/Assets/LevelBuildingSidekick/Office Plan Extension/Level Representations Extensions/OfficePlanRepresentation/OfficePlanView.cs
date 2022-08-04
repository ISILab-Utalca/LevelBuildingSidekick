using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LevelBuildingSidekick;
using System.Threading;

namespace LevelBuildingSidekick.OfficePlan
{
    public class OfficePlanView : LevelRepresentationView
    {
        Vector2 scrollPos;
        Thread thread;
        public OfficePlanView(Controller controller) : base(controller)
        {
        }

        public override void Draw2D()
        {
            var controller = Controller as OfficePlanController;

            EditorGUILayout.BeginHorizontal();
            controller.Graph.View.Draw2D();
            controller.Schema.View.Draw2D();
            controller.Toolkit.View.Draw2D();
            EditorGUILayout.EndHorizontal();

        }

        public override void DrawEditor()
        {
            var controller = Controller as OfficePlanController;

            if (controller.Graph.SelectedNode != null)
            {
                controller.Graph.SelectedNode.View.DrawEditor();
            }
            else if (controller.Graph.SelectedEdge != null)
            {
                controller.Graph.SelectedEdge.View.DrawEditor();
            }
            else
            {
                if (LBSController.Instance.CurrentLevel != null)
                {
                    LBSController.Instance.CurrentLevel.View.DrawEditor();

                    EditorGUILayout.Separator();
                }

                scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

                //controller.Schema.Size = EditorGUILayout.Vector2IntField("Tilemap Size", controller.Schema.Size);

                controller.Schema.TileSize = EditorGUILayout.IntField("Tile Size", controller.Schema.TileSize);

                EditorGUILayout.Space();

                ///controller.Floor = EditorGUILayout.ObjectField("Floor Tile", controller.Floor, typeof(GameObject), true) as GameObject;
                //controller.Wall = EditorGUILayout.ObjectField("Wall Tile", controller.Wall, typeof(GameObject), true) as GameObject;
                //controller.Door = EditorGUILayout.ObjectField("Door Tile", controller.Door, typeof(GameObject), true) as GameObject;

                if (GUILayout.Button("Regenerate Schema"))
                {
                    controller.GraphToSchema();
                    //Debug.Log(controller.Schema.Rooms.Count);
                }

                if (GUILayout.Button("Generate 3D Map"))
                {
                    controller.Generate3D();
                }

                if (GUILayout.Button("Optimize"))
                {
                    if(thread == null || !thread.IsAlive)
                    {
                        thread = new Thread(controller.Schema.Optimize);
                        thread.Start();
                    }
                }
                if(thread != null && thread.IsAlive)
                {
                    EditorGUILayout.Space();
                    GUILayout.Label("Running Thread");
                    EditorGUILayout.Space();
                }

                EditorGUILayout.Space();
                EditorGUILayout.EndScrollView();
            }
        }

        public override void Display2DWindow()
        {
            var controller = Controller as OfficePlanController;

            var graph = LBSController.Instance.RequestWindow("Graph Window");
            graph.draw = () => { controller.Graph.View.Draw2D(); controller.Graph.Update(); };
            var schema = LBSController.Instance.RequestWindow("Schema Window");
            schema.draw = () => { controller.Schema.View.Draw2D(); controller.Schema.Update(); };

            graph.Show();
            schema.Show();
        }

        public override void DisplayInspectorWindow()
        {
            var controller = Controller as OfficePlanController;

            var inspector = LBSController.Instance.RequestWindow("Inspector Window");
            inspector.draw = DrawEditor;

            inspector.Show();
        }
    }
}

