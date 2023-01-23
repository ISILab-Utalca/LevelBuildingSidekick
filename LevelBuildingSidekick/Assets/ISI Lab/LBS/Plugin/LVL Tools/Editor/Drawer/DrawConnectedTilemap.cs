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
        var tilemap = layer.GetModule<TileMapModule<ConnectedTile>>();

        foreach (var tile in tilemap.Tiles)
        {
            //var tile = new LBSConectedTileView(,);
        }
    }
}
