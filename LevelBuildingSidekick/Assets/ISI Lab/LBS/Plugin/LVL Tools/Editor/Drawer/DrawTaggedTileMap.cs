using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawTaggedTileMap : Drawer
{

    public override void Draw(ref LBSLayer layer, MainView view)
    {
        var tilemap = layer.GetModule<TaggedTileMap>();

        foreach (var k in tilemap.tiles.Keys)
        {
            var bundle = tilemap.tiles[k];
            var tView = new TileView(k);
            tView.style.backgroundImage = Utility.DirectoryTools.GetScriptable<LBSIdentifier>(bundle.BundleTag).Icon;
            var size = view.TileSize;
            tView.SetPosition(new Rect(k.Position * size, size));
            view.AddElement(tView);
        }
    }
}
