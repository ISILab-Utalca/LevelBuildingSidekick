using LBS.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DrawManager
{
    private MainView view;
    private List<LayerTemplate> templates;

    public DrawManager(ref MainView view, ref List<LayerTemplate> templates)
    {
        this.view = view;
        this.templates = templates;
    }

    public void RefreshView(ref LBSLayer layer,List<LBSLayer> allLayers, string modeName)
    {
        var _layer = layer;
        if (_layer == null)
            return;


        var template = templates.Find(t => t.layer.ID.Equals(_layer.ID));
        //var mode = template.modes.Find(m => m.name.Equals(modeName));

        // clear
        view.ClearView();

        var _allLayers = new List<LBSLayer>(allLayers);
        for (int i = _allLayers.Count - 1; i >= 0; i--)
        {
            var otherLayer = _allLayers[i];

            if (!otherLayer.IsVisible)
                continue;

            if (otherLayer == _layer)
            {
                //mode.Drawer.Draw(ref _layer, view);
            }
            else
            {
                var oTemplate = templates.Find(t => t.layer.ID.Equals(otherLayer.ID));
                //var oMode = oTemplate.modes[^1];
                var _other = otherLayer;
                //oMode.Drawer.Draw(ref _other, view);
            }
        }
    }

    public void Draw(LBSLevelData level, MainView view)
    {
        var layers = level.Layers;

        foreach(var l in layers)
        {
            if (l == null)
                continue;

            if (!l.IsVisible)
                continue;

            var behaviours = l.Behaviours;
            foreach(var b in behaviours)
            {
                if (b == null)
                    continue;

                var classes = Utility.Reflection.GetClassesWith<DrawerAttribute>();
                if (classes.Count == 0)
                    continue;

                var drawers = classes.Where(t => t.Item2.Any(v => v.type == b.GetType()));

                if (drawers.Count() == 0)
                    continue;

                var drawer = Activator.CreateInstance(drawers.First().Item1) as Drawer; // shold be registering it instead of instantiation each time it will paint
                drawer.Draw(b, view, l.TileSize);
            }

            var assistants = l.Assitants;
            foreach (var a in assistants)
            {
                if (a == null)
                    continue;

                var classes = Utility.Reflection.GetClassesWith<DrawerAttribute>();
                if (classes.Count == 0)
                    continue;

                var drawers = classes.Where(t => t.Item2.Any(v => v.type == a.GetType()));

                if (drawers.Count() <= 0)
                    continue;

                var drawer = Activator.CreateInstance(drawers.First().Item1) as Drawer; // shold be registering it instead of instantiation each time it will paint
                drawer.Draw(a, view, l.TileSize);
            }
        }
    }

    public void Redraw(LBSLevelData level, MainView view)
    {
        view.ClearView();
        Draw(level, view);
    }
}
