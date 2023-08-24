using LBS.Behaviours;
using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine.EventSystems;
using LBS.Components.Specifics;

[Drawer(typeof(SchemaBehaviour))]
public class SchemaDrawer : Drawer
{
    private readonly Vector2 nodeSize = new Vector2(100, 100);

    public override void Draw(ref LBSLayer layer, MainView view)
    {
        throw new System.NotImplementedException();
    }

    public override void Draw(object target, MainView view, Vector2 teselationSize)
    {
        var schema = target as SchemaBehaviour;

        foreach (var t in schema.Tiles)
        {
            var tView = new SchemaTileView();
            var size = DefalutSize * teselationSize;
            tView.SetPosition(new Rect(t.Position * size, size));

            var zone = schema.GetZone(t);
            tView.SetBackgroundColor(zone.Color);

            var conections = schema.GetConnections(t);
            tView.SetConnections(conections.ToArray());

            view.AddElement(tView);
        }

        var zones = schema.ZonesWhitTiles;
        foreach (var zone in zones)
        {
            var nView = new LBSNodeView();
            
            var bound = schema.GetTiles(zone).GetBounds();

            nView.SetPosition(new Rect(bound.center * nodeSize - (nodeSize / 2f), nodeSize));
            nView.SetText(zone.ID);
            nView.SetColor(zone.Color);

            view.AddElement(nView);
        }
    }
}
