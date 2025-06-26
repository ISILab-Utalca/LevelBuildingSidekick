using System;
using System.Collections.Generic;
using ISILab.LBS.Drawers;
using ISILab.LBS.Template;
using ISILab.LBS.VisualElements.Editor;
using LBS.Components;

namespace ISILab.LBS
{
    public class DrawManager
    {
        private MainView view;
        private LBSLevelData level;
        private static DrawManager instance;

        public static DrawManager Instance => instance;

        private readonly Dictionary<Type, Drawer> drawerCache = new();

        public DrawManager(ref MainView view)
        {
            this.view = view;
            instance = this;
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

        private void DrawLayer(LBSLayer layer)
        {
            if (layer == null || !layer.IsVisible)
                return;

            // Draw behaviours and assistants (if both share same drawer system)
            DrawVisibleComponents(layer.Behaviours, layer);
            DrawVisibleComponents(layer.Assistants, layer);
        }

        private void DrawVisibleComponents<T>(List<T> components, LBSLayer layer)
        {
            foreach (var component in components)
            {
                if (component == null)continue;
                var drawer = GetOrCreateDrawer(component.GetType());
                drawer?.Draw(component, view, layer.TileSize);
            }
        }

        private Drawer GetOrCreateDrawer(Type type)
        {
            if (!drawerCache.TryGetValue(type, out var drawer))
            {
                var drawerType = LBS_Editor.GetDrawer(type);
                if (drawerType == null)
                    return null;

                drawer = Activator.CreateInstance(drawerType) as Drawer;
                if (drawer != null)
                    drawerCache[type] = drawer;
            }
            return drawer;
        }

        public void RedrawLayer(LBSLayer layer, MainView view)
        {
            view.ClearLayerView(layer);
            DrawLayer(layer);
        }

        public void RedrawLevel(LBSLevelData level, MainView view)
        {
            foreach (var layer in level.Layers)
            {
                view.ClearLayerView(layer);
            }
            DrawLevel(level, view);
        }

        private void DrawLevel(LBSLevelData level, MainView view)
        {
            this.level = level;
            this.view = view;

            foreach (var layer in level.Layers)
            {
                DrawLayer(layer);
            }
        }
    }
}
