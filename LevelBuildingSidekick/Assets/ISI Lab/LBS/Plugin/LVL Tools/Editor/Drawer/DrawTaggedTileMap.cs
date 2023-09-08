using LBS.Behaviours;
using LBS.Components;
using LBS.Settings;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class DrawTaggedTileMap : Drawer
{


    public override void Draw(object target, MainView view, Vector2 teselationSize)
    {
        /*
        var tilemap = layer.GetModule<TaggedTileMap>();

        foreach (var k in tilemap.PairTiles.Select(x => x.Tile))
        {
            var bundle = tilemap.GetBundleData(k);
            var tView = new TileView(k);
            tView.style.backgroundImage = bundle.Identifier.Icon; //Utility.DirectoryTools.GetScriptable<LBSIdentifier>(bundle.BundleTag).Icon;
            var size = LBSSettings.Instance.general.TileSize * layer.TileSize;
            tView.SetPosition(new Rect(k.Position * size, size));
            view.AddElement(tView);
        }
        */
    }
}
