using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LevelBuildingSidekick;

namespace LevelBuildingSidekick.OfficePlan
{
    public class OfficePlanView : LevelRepresentationView
    {
        Vector2 scrollPos;
        public OfficePlanView(Controller controller) : base(controller)
        {
        }

        public override void Draw2D()
        {
            var controller = Controller as OfficePlanController;

            EditorGUILayout.BeginHorizontal();
            controller.Graph.View.Draw2D();
            controller.Blueprint.View.Draw2D();
            controller.toolkit.View.Draw2D();
            EditorGUILayout.EndHorizontal();

        }

        public override void DrawEditor()
        {

            var controller = Controller as OfficePlanController;

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            controller.Graph.Size = EditorGUILayout.Vector2IntField("Level Representation Size", controller.Graph.Size);


            controller.Teseleation = (TeselationType)EditorGUILayout.EnumPopup("Teselation Type", controller.Teseleation);

            if (controller.Teseleation == TeselationType.DOWNSCALE)
            {
                controller.Blueprint.Step = EditorGUILayout.IntField("Tile Size", controller.Blueprint.Step);

                controller.Blueprint.Size = (controller.Graph.Size / controller.Blueprint.Step);
            }
            else
            {
                Vector2Int v = controller.Blueprint.Size;
                controller.Blueprint.Size = EditorGUILayout.Vector2IntField("Tilemap Size", controller.Blueprint.Size);
                if (controller.Blueprint.Size != v)
                {
                    controller.Blueprint.Step = controller.Graph.Size.x / v.x;
                }
            }

            controller.Blueprint.Stride = EditorGUILayout.IntField("Stride", controller.Blueprint.Stride);

            EditorGUILayout.Space();

            controller.Floor = EditorGUILayout.ObjectField("Floor Tile", controller.Floor, typeof(GameObject), true) as GameObject;
            controller.Wall = EditorGUILayout.ObjectField("Wall Tile", controller.Wall, typeof(GameObject), true) as GameObject;
            controller.Door = EditorGUILayout.ObjectField("Door Tile", controller.Door, typeof(GameObject), true) as GameObject;

            if (GUILayout.Button("Regenerate Blueprint"))
            {
                controller.SimpleGraphToBlueprint();
            }

            if (GUILayout.Button("Generate 3D Map"))
            {
                controller.Generate3D();
            }

            EditorGUILayout.EndScrollView();
        }

    }
}

