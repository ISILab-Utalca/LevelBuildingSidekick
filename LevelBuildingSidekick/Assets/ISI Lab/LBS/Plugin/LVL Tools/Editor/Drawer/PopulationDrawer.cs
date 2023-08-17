using LBS.Behaviours;
using LBS.Components;
using LBS.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Drawer(typeof(PopulationBehaviour))]
public class PopulationDrawer : Drawer
{
    public override void Draw(ref LBSLayer layer, MainView view)
    {
        throw new System.NotImplementedException();
    }

    public override void Draw(LBSBehaviour behaviour, MainView view)
    {
        var population = behaviour as PopulationBehaviour;

        if (population == null)
        {
            return;
        }

        foreach(var t in population.Tilemap)
        {
            var v = new PopulationTileView(t);
            v.SetPosition(new Rect(t.Tile.Position, behaviour.Owner.TileSize * LBSSettings.Instance.general.TileSize));
            view.Add(v);
        }

    }
}
