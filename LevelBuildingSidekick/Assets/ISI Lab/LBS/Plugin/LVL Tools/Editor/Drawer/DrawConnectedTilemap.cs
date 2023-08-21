using LBS.Behaviours;
using LBS.Components;
using LBS.Components.TileMap;
using LBS.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DrawConnectedTilemap : Drawer // DrawSchema
{
    public override void Draw(ref LBSLayer layer, MainView view)
    {
        var tilemap = layer.GetModule<AreaTileMap<TiledArea>>();

        foreach (var area in tilemap.Areas)
        {
            foreach (var tile in area.Tiles)
            {
                var cTile = tile as ConnectedTile;
                var tView = new SchemaTileView(cTile);
                tView.SetBackgroundColor(area.Color);
                tView.SetConnections(cTile.Connections);
                var size = LBSSettings.Instance.general.TileSize * layer.TileSize;
                tView.SetPosition(new Rect(tile.Position * size, size));
                view.AddElement(tView);
            }
        }
    }

    public override void Draw(object target, MainView view, Vector2 teselationSize)
    {
        throw new System.NotImplementedException();
    }
}
