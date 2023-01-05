using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(MapWFC))]
public class MapWFC_Editor : Editor
{
    public Texture locked, unlocked,free;
    public Vector2 iconSize = new Vector2(32,32);

    /// <summary>
    /// 
    /// </summary>
    private void OnSceneGUI()
    {
        var select = (MapWFC)target;

        var eBorder = select.m_extraBorder;
        var min = select.GetMin() - new Vector2Int(eBorder, eBorder);
        var max = select.GetMax() + new Vector2Int(eBorder + 1, eBorder + 1);

        for (int i = min.x; i < max.x; i++)
        {
            for (int j = min.y; j < max.y; j++)
            {
                var center = new Vector3(i, 0, j) * select.TileSize + select.transform.position;
                var size = new Vector3(select.TileSize, 0, select.TileSize);
                Handles.DrawWireCube(center,size);

                var sCenter = HandleUtility.WorldToGUIPoint(center);
                var rect = new Rect(sCenter - (iconSize / 2f), iconSize);

                var tile = select.Tiles.Find(t => t.position.x == i && t.position.y == j);

                Handles.BeginGUI();
                if(tile.data != null)
                {
                    if (tile.locked ? GUI.Button(rect, locked) : GUI.Button(rect, unlocked))
                    {
                        //SceneView.AddOverlayToActiveView<>();
                        //handleExample.shieldArea = 5;
                    }
                }
                else
                {
                    if (GUI.Button(rect, free)) 
                    {
                        var sel = TileEditWindow.selected;
                        select.Add(new TileWFC_struct(new TileConnectWFC(sel, 0), new Vector2Int(i, j), true)); 
                        //handleExample.shieldArea = 5;
                    }
                }
                Handles.EndGUI();
            }
        }

        foreach (var tile in select.Tiles)
        {
            var center = new Vector3(tile.position.x, 0, tile.position.y) * select.TileSize + select.transform.position;
            
        }
    }
}