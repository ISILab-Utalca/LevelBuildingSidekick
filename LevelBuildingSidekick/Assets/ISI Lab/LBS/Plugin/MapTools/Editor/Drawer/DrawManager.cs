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
        private MainView _view;
        private LBSLevelData _level;
        private static DrawManager instance;

        public static DrawManager Instance => instance;

        private readonly Dictionary<Type, Drawer> _drawerCache = new();

        public DrawManager(ref MainView view)
        {
            this._view = view;
            instance = this;
        }

        public void AddContainer(LBSLayer layer)
        {
            _view.AddContainer(layer);
        }

        public void RemoveContainer(LBSLayer layer)
        {
            _view.RemoveContainer(layer);
        }

        public static void ReDraw()
        {
            instance.RedrawLevel(instance._level, instance._view);
        }

        private void DrawLayer(LBSLayer layer)
        {
            if (layer == null || !layer.IsVisible) return;
            
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
                drawer?.Draw(component, _view, layer.TileSize);
            }
        }

        private Drawer GetOrCreateDrawer(Type type)
        {
            if (!_drawerCache.TryGetValue(type, out var drawer))
            {
                var drawerType = LBS_Editor.GetDrawer(type);
                if (drawerType == null)
                    return null;

                drawer = Activator.CreateInstance(drawerType) as Drawer;
                if (drawer != null)
                    _drawerCache[type] = drawer;
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
            view.ClearLevelView();
            DrawLevel(level, view);
        }

        private void DrawLevel(LBSLevelData level, MainView view)
        {
            this._level = level;
            this._view = view;

            foreach (var layer in level.Layers)
            {
                DrawLayer(layer);
            }
        }
    }
}
