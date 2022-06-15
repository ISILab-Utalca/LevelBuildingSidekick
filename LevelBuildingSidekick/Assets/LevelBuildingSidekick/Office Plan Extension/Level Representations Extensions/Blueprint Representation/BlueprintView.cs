using LevelBuildingSidekick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LevelBuildingSidekick.Blueprint
{
    public class BlueprintView : LevelRepresentationView
    {
        Vector2 scrollPos;
        public BlueprintView(Controller controller) : base(controller)
        {
        }

        public override void Draw2D()
        {
            var controller = (Controller as BlueprintController);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            var r = GUILayoutUtility.GetRect((controller.Size.x * controller.Step) + ((controller.Size.x + 1) * controller.Stride),
                                            (controller.Size.y * controller.Step) + ((controller.Size.y + 1) * controller.Stride));
            //GUILayout.BeginArea(r);

            //Texture2D texture = new Texture2D(controller.Size.x, controller.Size.y);
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, Color.black);

            for (int i = 0; i < controller.TileMap.GetLength(0); i++)
            {
                for (int j = 0; j < controller.TileMap.GetLength(1); j++)
                {
                    if (controller.TileMap[i, j] == 0)
                    {
                        //texture.SetPixel(i, j, Color.black);
                        /*Handles.DrawSolidRectangleWithOutline(new Rect((controller.Step*i) + (controller.Stride*(i+1)),
                            (controller.Step * j) + (controller.Stride * (j + 1)), 
                            controller.Step, controller.Step), 
                            Color.black, Color.black);*/
                    }
                    else
                    {
                        //texture.SetPixel(i, j, Color.white);
                        Handles.DrawSolidRectangleWithOutline(new Rect((controller.Step * i) + (controller.Stride * (i + 1)),
                            (controller.Step * j) + (controller.Stride * (j + 1)),
                            controller.Step, controller.Step),
                            Color.white, Color.white);
                    }
                }
            }
            texture.Apply();
            //base.Draw2D();
            GUI.DrawTexture(r, texture);
            //GUILayout.EndArea();
            EditorGUILayout.EndScrollView();

        }

        public override void DrawEditor()
        {
            var controller = Controller as BlueprintController;
            controller.Size = EditorGUILayout.Vector2IntField("Size", controller.Size);
            controller.Step = EditorGUILayout.IntField("Size", controller.Step);
            controller.Stride = EditorGUILayout.IntField("Size", controller.Stride);
        }

    }
}

