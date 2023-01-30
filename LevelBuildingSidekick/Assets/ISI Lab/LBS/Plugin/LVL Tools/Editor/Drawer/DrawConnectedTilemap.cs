using LBS.Components;
using LBS.Components.TileMap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DrawConnectedTilemap : Drawer
{
    public override void Draw(ref LBSLayer layer, MainView view)
    {
        var tilemap = layer.GetModule<AreaTileMap<TiledArea<ConnectedTile>, ConnectedTile>>();

        foreach (var area in tilemap.Areas)
        {
            foreach (var tile in area.Tiles)
            {
                var tView = new LBSConectedTileView(tile as ConnectedTile);
                tView.SetBackgroundColor(area.Color);
                var size = view.TileSize;
                tView.SetPosition(new Rect(tile.Position * size, size));
                view.AddElement(tView);
            }
        }
    }
}
