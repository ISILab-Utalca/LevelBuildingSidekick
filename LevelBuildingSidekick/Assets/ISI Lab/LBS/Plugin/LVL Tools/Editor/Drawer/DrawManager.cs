using ISILab.LBS.Modules;
using ISILab.LBS.VisualElements.Editor;
using LBS.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ISILab.LBS
{
    public class DrawManager
    {
        private MainView view;
        private List<LayerTemplate> templates;

        private LBSLevelData level;

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

        public void AddContainer(LBSLayer layer)
        {
            view.AddContainer(layer);
        }

        public void RemoveContainer(LBSLayer layer)
        {
            view.RemoveContainer(layer);
        }

        public static void ReDraw()
        {
            instance.RedrawLevel(instance.level, instance.view);
        }

        public void RefreshView(LBSLayer layer, List<LBSLayer> allLayers, string modeName)
        {
            if (layer == null)
                return;

            var template = templates.Find(t => t.layer.ID.Equals((string)layer.ID));

            view.ClearLevelView();

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

        public void DrawLayer(LBSLayer layer, MainView mainView)
        {
            var l = layer;
            if (l == null)
                return;

            if (!l.IsVisible)
                return;

            var behaviours = l.Behaviours;
            foreach (var b in behaviours)
            {
                if (b == null)
                    continue;

                if (!b.visible)
                    continue;

                var drawerT = LBS_Editor.GetDrawer(b.GetType());

                if (drawerT == null)
                    continue;

                var drawer = Activator.CreateInstance(drawerT) as Drawer;

                drawer.Draw(b, view, l.TileSize);
            }

            var assistants = l.Assitants;
            foreach (var a in assistants)
            {
                if (a == null)
                    continue;

                if (!a.visible)
                    continue;

                var drawerT = LBS_Editor.GetDrawer(a.GetType());

                if (drawerT == null)
                    continue;

                var drawer = Activator.CreateInstance(drawerT) as Drawer;

                drawer.Draw(a, view, l.TileSize);
            }
        }

        private List<Type> GetDrawers(LBSLayer layer)
        {
            var toR = new List<Type>();
            foreach (var b in layer.Behaviours)
            {
                if (b == null)
                    continue;

                if (!b.visible)
                    continue;

                var drawerT = LBS_Editor.GetDrawer(b.GetType());

                if (drawerT == null)
                    continue;

                toR.Add(drawerT);
            }

            foreach (var a in layer.Assitants)
            {
                if (a == null)
                    continue;

                if (!a.visible)
                    continue;

                var drawerT = LBS_Editor.GetDrawer(a.GetType());

                if (drawerT == null)
                    continue;

                toR.Add(drawerT);
            }

            return toR;
        }

        public void RedrawLayer(LBSLayer layer, MainView view)
        {
            view.ClearLayerView(layer);
            DrawLayer(layer, view);
        }

        public void RedrawLevel(LBSLevelData level, MainView view)
        {
            view.ClearLevelView();
            DrawLevel(level, view);
        }

        private void DrawLevel(LBSLevelData level, MainView MainView)
        {
            this.level = level;
            this.view = MainView;

            var layers = level.Layers;
            var quests = level.Quests;
            DrawLayers(layers, MainView);
            DrawLayers(quests, MainView);
        }

        private void DrawLayers(List<LBSLayer> layers, MainView mainView)
        {
            for (int i = layers.Count - 1; i >= 0; i--)
            {
                DrawLayer(layers[i], mainView);
            }
        }

        public void RedrawElement(LBSLayer layer, LBSModule module, object[] olds, object[] news)
        {
            var container = view.GetLayerContainer(layer);

            // get drawers of layer
            var drawersT = GetDrawers(layer);
            foreach (var drawerT in drawersT)
            {
                var drawer = Activator.CreateInstance(drawerT) as Drawer;
                drawer.ReDraw(layer, olds, news, view, layer.TileSize);
            }
        }
    }
}
