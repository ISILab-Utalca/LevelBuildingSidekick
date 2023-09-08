using LBS.Behaviours;
using LBS.Components;
using LBS.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[Drawer(typeof(PopulationBehaviour))]
public class PopulationDrawer : Drawer
{
<<<<<<< Updated upstream
    [Obsolete]
    public override void Draw(ref LBSLayer layer, MainView view)
    {
        throw new System.NotImplementedException();
    }
=======
>>>>>>> Stashed changes

    public override void Draw(object target, MainView view, Vector2 teselationSize)
    {
        var population = target as PopulationBehaviour;

        if (population == null)
        {
            return;
        }

        foreach (var t in population.Tilemap)
        {
            var v = new PopulationTileView(t);
            var size = population.Owner.TileSize * LBSSettings.Instance.general.TileSize;
            v.SetPosition(new Rect(t.Tile.Position * size, size));
            view.AddElement(v);

        }
    }
}
