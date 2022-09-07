using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using System;
using Utility;
using LBS.Representation.TileMap;
using LBS.ElementView;
using LevelBuildingSidekick;



public class TileGridView : LBSGraphView
{
    /*
    public new class UxmlFactory : UxmlFactory<TileGridView, GraphView.UxmlTraits> { }

    // public LBSTileGridControler controller
    public LBSTileMapData data;

    public Vector2 tileSize = new Vector2(100,100);

    public TileGridView()
    {
        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
         
        var styleSheet = DirectoryTools.SearchAssetByName<StyleSheet>("TileMapEditor");
        styleSheets.Add(styleSheet);
    }

    public void ShowLabels(bool value)
    {
        foreach (var element in graphElements)
        {
            if(element is TileView)
            {
                var el = element as TileView;
                el.ShowLabel(value);
            }
        }
    }

    public override void Populate<T>(T value)
    {
        var data = value as LBSTileMapData;
        if (data == null)
            Debug.LogWarning("[Error]: The information you are trying to upload cannot be displayed in this view.");

        // Esto demora 1.8 seg en completarse con alrededor de 550 tiles,
        // es necesario mejorar la eficinecia en este paso ya que añade mucha demora.
        // Se sugiere probar con object pool o algo asi. (!!!)
        var mtx = data.GetMatrix();
        for (int i = 0; i < mtx.GetLength(0); i++)
        {
            for (int j = 0; j < mtx.GetLength(1); j++)
            {
                var roomId = mtx[i, j];
                var roomData = data.GetRoom(roomId);
                if (roomData != null)
                {
                    CreateTileView(new Vector2Int(i, j), tileSize, roomData); // esta es la linea en cuestion que lagea (!!!)
                }
            }
        }

    }

    void CreateTileView(Vector2Int tilePos,Vector2 size,RoomData data)
    {
        var tile = new TileView();
        tile.SetPosition(new Rect(tilePos * size, size));
        tile.SetSize((int)size.x, (int)size.y);
        tile.SetColor(data.Color);
        tile.SetLabel(tilePos);
        AddElement(tile);
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        //base.BuildContextualMenu(evt);
        var pos = (evt.localMousePosition - new Vector2(viewTransform.position.x, viewTransform.position.y)) / scale;
        Vector2Int tilePos = new Vector2Int((int)pos.x / (int)tileSize.x, (int)pos.y / (int)tileSize.y);

        evt.menu.AppendAction("Cords/enable", (a) => 
        {
            ShowLabels(true);
        });

        evt.menu.AppendAction("Cords/disable", (a) => 
        {
            ShowLabels(false);
        });
        evt.menu.AppendAction("Add Tile", (a) => 
        {
            data.AddTiles(new List<Vector2Int>() { tilePos }, "test");
            CreateTileView(tilePos, tileSize);
            Debug.Log("Ge:"+graphElements.ToList().Count);
        });
    }
     */
}
