using LevelBuildingSidekick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BlueprintView : LevelRepresentationView
{
    Vector2 scrollPos;
    public BlueprintView(Controller controller) : base(controller)
    {
    }

    public override void Draw2D()
    {
        BlueprintData data = (Controller as BlueprintController).Data as BlueprintData;
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        EditorGUILayout.BeginVertical();
        for(int i = 0; i < data.tilemap.GetLength(0); i++)
        {
            EditorGUILayout.BeginHorizontal();
            for(int j = 0; j < data.tilemap.GetLength(1); j++)
            {
                if (data.tilemap[i,j] == 0)
                {
                    Handles.DrawSolidRectangleWithOutline(new Rect(0, 0, data.tileSize, data.tileSize), Color.gray, Color.gray);
                }
                else
                {
                    Handles.DrawSolidRectangleWithOutline(new Rect(0, 0, data.tileSize, data.tileSize), Color.white, Color.white);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
            
    }

}
