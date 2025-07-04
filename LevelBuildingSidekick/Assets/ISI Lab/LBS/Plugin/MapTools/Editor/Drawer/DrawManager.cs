using System;
using System.Collections.Generic;
using ISILab.LBS.Drawers;
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
        private readonly Dictionary<LBSLayer, bool> _preVisibility = new();

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
            // Validation
            if (layer == null) return;
            if (!_preVisibility.ContainsKey(layer))
            {
                _preVisibility.Add(layer, layer.IsVisible);
            }
            
            // Change visibility of layer
            else
            {
                switch (layer.IsVisible)
                {
                    case true when !_preVisibility[layer]:
                        ShowVisuals(layer.Assistants, layer);
                        ShowVisuals(layer.Behaviours, layer);
                        break;
                    case false when _preVisibility[layer]:
                        HideVisuals(layer.Assistants, layer);
                        HideVisuals(layer.Behaviours, layer);
                        break;
                }
            }
            _preVisibility[layer] = layer.IsVisible;
            
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
                drawer?.Draw(component, MainView.Instance,layer.TileSize);
                //Debug.Log("drawing call");
            }
        }

        private void HideVisuals<T>(List<T> components, LBSLayer layer)
        {
            foreach (var component in components)
            {
                if (component == null)continue;
                var drawer = GetOrCreateDrawer(component.GetType());
                drawer?.HideVisuals(component, _view);
            }
        }
        private void ShowVisuals<T>(List<T> components, LBSLayer layer)
        {
            foreach (var component in components)
            {
                if (component == null)continue;
                var drawer = GetOrCreateDrawer(component.GetType());
                drawer?.ShowVisuals(component, _view);
            }
        }

        private Drawer GetOrCreateDrawer(Type type)
        {
            if (_drawerCache.TryGetValue(type, out var drawer)) return drawer;
            
            var drawerType = LBS_Editor.GetDrawer(type);
            if (drawerType == null)
                return null;

            drawer = Activator.CreateInstance(drawerType) as Drawer;
            if (drawer != null)
                _drawerCache[type] = drawer;
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
            this._level = level;
            this._view = view;

            foreach (var layer in level.Layers)
            {
                DrawLayer(layer);
            }
        }
    }
}
