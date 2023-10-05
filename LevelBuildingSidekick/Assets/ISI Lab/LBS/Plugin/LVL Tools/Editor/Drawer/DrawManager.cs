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

    private LBSLevelData level;
    private MainView mainView;

    private static DrawManager instance;
    public static DrawManager Instance
    {
        get { return instance; }
    }

    public DrawManager(ref MainView view, ref List<LayerTemplate> templates)
    {
        this.view = view;
        this.templates = templates;

        DrawManager.instance = this;
    }

    public static void ReDraw()
    {
        instance.Redraw(instance.level, instance.mainView);
    }

    public void RefreshView(LBSLayer layer,List<LBSLayer> allLayers, string modeName)
    {
        if (layer == null)
            return;

        var template = templates.Find(t => t.layer.ID.Equals((string)layer.ID));

        view.ClearView();

        var _allLayers = new List<LBSLayer>(allLayers);
        for (int i = _allLayers.Count - 1; i >= 0; i--)
        {
            var otherLayer = _allLayers[i];

            if (!otherLayer.IsVisible)
                continue;

            if (otherLayer != layer)
            {
                var oTemplate = templates.Find(t => t.layer.ID.Equals(otherLayer.ID));
                var _other = otherLayer;
            }
        }
    }

    public void Draw(LBSLevelData level, MainView MainView)
    {
        this.level = level;
        this.mainView = MainView;

        var layers = level.Layers;

        for(int i = layers.Count - 1; i >= 0; i--)
        {
            var l = layers[i];

            if (l == null)
                continue;

            if (!l.IsVisible)
                continue;

            var behaviours = l.Behaviours;
            foreach(var b in behaviours)
            {
                if (b == null)
                    continue;

                if (!b.visible)
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

                if (!a.visible)
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
