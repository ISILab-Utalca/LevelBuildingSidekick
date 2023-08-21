using LBS.Behaviours;
using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine.EventSystems;

[Drawer(typeof(SchemaBehaviour))]
public class SchemaDrawer : Drawer
{
    public override void Draw(ref LBSLayer layer, MainView view)
    {
        throw new System.NotImplementedException();
    }

    public override void Draw(object target, MainView view, Vector2 teselationSize)
    {
        var schema = target as SchemaBehaviour;

        foreach (var t in schema.Tiles)
        {
            //var z = schema.GetZone(t);
            //var c = schema.GetConnections(t);
            var tView = new SchemaTileView();
            var size = DefalutSize * teselationSize;
            tView.SetPosition(new Rect(t.Position * size, size));

            var zone = schema.GetZone(t);
            tView.SetBackgroundColor(zone.Color);

            var conections = schema.GetConnections(t);
            tView.SetConnections(conections.ToArray());

            view.AddElement(tView);
        }
    }
}
