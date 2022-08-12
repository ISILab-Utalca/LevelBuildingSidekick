using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using System;
using Utility;
using LBS.Representation.TileMap;

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

    void CreateTileView(Vector2Int tile,Vector2 size)
    {
        var t = new TileView(tile,size);
        AddElement(t);
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        //base.BuildContextualMenu(evt);
        var pos = (evt.localMousePosition - new Vector2(viewTransform.position.x, viewTransform.position.y)) / scale;
        Vector2Int tilePos = new Vector2Int((int)pos.x / (int)tileSize.x, (int)pos.y / (int)tileSize.y);

        /*
        evt.menu.AppendAction("Add Tile", (a) => 
        {
            var tile = map.CreateTile(tilePos);
            CreateTileView(tile,tileSize);
            Debug.Log("Ge:"+graphElements.ToList().Count);
        });
        */
    }
}
public class TileView : GraphElement, ICollectibleElement
{
    private Vector2Int tile;

    public TileView(Vector2Int tile,Vector2 size)
    {
        this.tile = tile;
        this.SetPosition(new Rect(tile * size, size));
        var v = new VisualElement();
        v.Add(new Box());
        Add(v);
       

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/TEST/USS/TileUSS.uss");
        styleSheets.Add(styleSheet);
    }

    public void CollectElements(HashSet<GraphElement> collectedElementSet, Func<GraphElement, bool> conditionFunc)
    {
        throw new NotImplementedException();
    }

}
