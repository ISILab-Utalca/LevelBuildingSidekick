using LevelBuildingSidekick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LevelBuildingSidekick.Blueprint
{
    public class SchemaView : LevelRepresentationView
    {
        Vector2 scrollPos;
        public SchemaView(Controller controller) : base(controller)
        {
        }

        public override void Draw2D()
        {
            //Color c = Color.black;
            var controller = (Controller as SchemaController);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            Vector2Int size = new Vector2Int(controller.TileMap.GetLength(0), controller.TileMap.GetLength(1));

            var r = GUILayoutUtility.GetRect((size.x * controller.TileSize), 
                (size.y * controller.TileSize), 
                (size.x * controller.TileSize),
                (size.y * controller.TileSize));
            r.width = (size.x * controller.TileSize);
            r.height = (size.y * controller.TileSize);
            //Debug.Log(r.width + " - " + r.height);
            //GUILayout.BeginArea(r);

            //Texture2D texture = new Texture2D(controller.Size.x, controller.Size.y);
            Texture2D texture = new Texture2D(1,1);
            texture.SetPixel(0, 0, Color.black);
            texture.Apply();
            GUI.DrawTexture(r, texture);

            for (int i = 0; i < controller.TileMap.GetLength(0); i++)
            {
                for (int j = 0; j < controller.TileMap.GetLength(1); j++)
                {
                    if (controller.TileMap[i, j] == 0)
                    {
                        //texture.SetPixel(i, j, Color.black);
                        /*Handles.DrawSolidRectangleWithOutline(new Rect((controller.Step*i),
                            (controller.Step * j), 
                            controller.Step, controller.Step), 
                            Color.black, Color.black);*/
                    }
                    else
                    {
                            //texture.SetPixel(i, j, Color.white);
                        Texture2D t = new Texture2D(1, 1);
                        t.SetPixel(0, 0, Color.white);
                        t.Apply();
                       var rect = new Rect((controller.TileSize * i),
                            (controller.TileSize * j),
                            controller.TileSize, controller.TileSize);
                        GUI.DrawTexture(rect, t);
                        /*Handles.DrawSolidRectangleWithOutline(new Rect((controller.Step * i),
                            (controller.Step * j),
                            controller.Step, controller.Step),
                            Color.white, Color.white);*/
                    }
                }
            }
            //base.Draw2D();
            //GUILayout.EndArea();
            EditorGUILayout.EndScrollView();

        }

        public override void DrawEditor()
        {
            var controller = Controller as SchemaController;
            //controller.Size = EditorGUILayout.Vector2IntField("Size", controller.Size);
            controller.TileSize = EditorGUILayout.IntField("Size", controller.TileSize);
        }

    }
}

