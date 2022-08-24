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

public class TileGridView : GraphView
{
    public new class UxmlFactory : UxmlFactory<TileGridView, GraphView.UxmlTraits> { }

    public TileMapController controller;

    public Vector2 tileSize = new Vector2(100,100);

    public TileGridView()
    {
        Insert(0,new GridBackground());

        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
         
        var styleSheet = DirectoryTools.SearchAssetByName<StyleSheet>("TileMapEditor");
        styleSheets.Add(styleSheet);

    }

    internal void SetView(TileMapController controller)
    {
        this.controller = controller;
        DeleteElements(graphElements);

        var data = controller.Data as LBSTileMapData;
        var mtx = data.GetMatrix();
        for (int i = 0; i < mtx.GetLength(0); i++)
        {
            for (int j = 0; j < mtx.GetLength(1); j++)
            {
                var roomId = mtx[i, j];
                if(data.GetRoomByID(roomId) != null)
                {
                    CreateTileView(new Vector2Int(i,j), tileSize);
                }
            }
        }
    }

    void CreateTileView(Vector2Int tilePos,Vector2 size)
    {
        var tile = new Tile();
        tile.SetPosition(new Rect(tilePos * size, size));
        tile.SetSize((int)size.x, (int)size.y);
        AddElement(tile);
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        //base.BuildContextualMenu(evt);
        var pos = (evt.localMousePosition - new Vector2(viewTransform.position.x, viewTransform.position.y)) / scale;
        Vector2Int tilePos = new Vector2Int((int)pos.x / (int)tileSize.x, (int)pos.y / (int)tileSize.y);

        
        evt.menu.AppendAction("Add Tile", (a) => 
        {
            var data = controller.Data as LBSTileMapData;
            data.AddTiles(new List<Vector2Int>() { tilePos }, "test");
            CreateTileView(tilePos, tileSize);
            Debug.Log("Ge:"+graphElements.ToList().Count);
        });
        
    }
}
