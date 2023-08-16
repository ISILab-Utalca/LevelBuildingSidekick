using LBS.Behaviours;
using LBS.Components;
using LBS.Components.TileMap;
using LBS.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DrawExterior : Drawer
{
    public DrawExterior() : base() { }

    public override void Draw(ref LBSLayer layer, MainView view)
    {
        var tilemap = layer.GetModule<TileMapModule>();

        foreach (var tile in tilemap.Tiles)
        {
            var cTile = tile as ConnectedTile;
            var tView = new ExteriorTileView(cTile);
            tView.SetBackgroundColor(Color.gray);
            tView.SetConnections(cTile.Connections);
            var size = LBSSettings.Instance.general.TileSize * layer.TileSize;
            tView.SetPosition(new Rect(tile.Position * size, size));
            view.AddElement(tView);
        }
    }

    public override void Draw(LBSBehaviour behaviour, MainView view)
    {
        throw new System.NotImplementedException();
    }
}
