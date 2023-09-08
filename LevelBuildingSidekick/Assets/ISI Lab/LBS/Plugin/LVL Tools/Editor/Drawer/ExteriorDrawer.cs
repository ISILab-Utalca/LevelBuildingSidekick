using LBS.Behaviours;
using LBS.Components;
using LBS.Components.TileMap;
using LBS.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

[Drawer(typeof(ExteriorBehaviour))]
public class ExteriorDrawer : Drawer
{
    public ExteriorDrawer() : base() { }

    public override void Draw(object target, MainView view, Vector2 teselationSize)
    {
        // Get behaviours
        var exterior = target as ExteriorBehaviour;

        // Get modules
        var tileMod = exterior.Owner.GetModule<TileMapModule>();
        var connectMod = exterior.Owner.GetModule<ConnectedTileMapModule>();

        foreach (var tile in exterior.Tiles)
        {
            var connections = connectMod.GetConnections(tile);

            var tView = new ExteriorTileView(connections);
            tView.SetBackgroundColor(Color.gray);
            tView.SetConnections(connections.ToArray());


            var size = DefalutSize * teselationSize;
            tView.SetPosition(new Rect(tile.Position * size, size));
            view.AddElement(tView);
        }
    }
}
