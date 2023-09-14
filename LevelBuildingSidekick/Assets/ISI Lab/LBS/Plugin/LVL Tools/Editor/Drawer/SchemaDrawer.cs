using LBS.Behaviours;
using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine.EventSystems;
using LBS.Components.Specifics;
using LBS.Components.TileMap;

[Drawer(typeof(SchemaBehaviour))]
public class SchemaDrawer : Drawer
{
    public override void Draw(object target, MainView view, Vector2 teselationSize)
    {
        var schema = target as SchemaBehaviour;
        var layer = schema.Owner;

        var tilesMod = layer.GetModule<TileMapModule>();
        var zonesMod = layer.GetModule<SectorizedTileMapModule>();
        var connectionsMod = layer.GetModule<ConnectedTileMapModule>();

        foreach (var t in tilesMod.Tiles)
        {
            var pos = new Vector2(t.Position.x, -t.Position.y);

            var tView = new SchemaTileView();
            var size = DefalutSize * teselationSize;
            tView.SetPosition(new Rect(pos * size, size));

            var zone = zonesMod.GetZone(t);
            tView.SetBackgroundColor(zone.Color);

            var conections = connectionsMod.GetConnections(t);
            tView.SetConnections(conections.ToArray());

            view.AddElement(tView);
        }
    }
}
