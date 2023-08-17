using LBS.Behaviours;
using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Drawer(typeof(SchemaBehaviour))]
public class SchemaDrawer : Drawer
{
    public override void Draw(ref LBSLayer layer, MainView view)
    {
        throw new System.NotImplementedException();
    }

    public override void Draw(LBSBehaviour behaviour, MainView view)
    {
        var schema = behaviour as SchemaBehaviour;

        foreach(var t in schema.Tiles)
        {
            var v = new SchemaTileView(schema.GetConnections(t), schema.GetZone(t));
        }
    }
}
