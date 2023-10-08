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
            var p = new Vector2(t.Tile.Position.x, -t.Tile.Position.y);
            v.SetPosition(new Rect(p * size, size));
            view.AddElement(v);
        }
    }
}