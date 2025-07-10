using System;
using System.Collections.Generic;
using ISILab.LBS.Drawers;
using ISILab.LBS.VisualElements.Editor;
using LBS.Components;

namespace ISILab.LBS
{
    public class DrawManager
    {
        private readonly MainView _view = MainView.Instance;
        private LBSLevelData _level;

        public static DrawManager Instance { get; private set; }

        private readonly Dictionary<(Type, LBSLayer), Drawer> _drawerCache = new();
        private readonly Dictionary<LBSLayer, bool> _preVisibility = new();

        public DrawManager()
        {
            Instance = this;
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
            Instance.RedrawLevel(Instance._level);
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
                var drawer = GetOrCreateDrawer(component.GetType(), layer);
                drawer?.Draw(component, MainView.Instance,layer.TileSize);
                //Debug.Log("drawing call");
            }
        }

        private void HideVisuals<T>(List<T> components, LBSLayer layer)
        {
            foreach (var component in components)
            {
                if (component == null)continue;
                var drawer = GetOrCreateDrawer(component.GetType(), layer);
                drawer?.HideVisuals(component, _view);
            }
        }
        private void ShowVisuals<T>(List<T> components, LBSLayer layer)
        {
            foreach (var component in components)
            {
                if (component == null)continue;
                var drawer = GetOrCreateDrawer(component.GetType(), layer);
                drawer?.ShowVisuals(component, _view);
            }
        }

        private Drawer GetOrCreateDrawer(Type type, LBSLayer layer)
        {
            var pairKey = (type, layer);
            if (_drawerCache.TryGetValue(pairKey, out var drawer)) return drawer;
            
            var drawerType = LBS_Editor.GetDrawer(type);
            if (drawerType == null)
                return null;

            drawer = Activator.CreateInstance(drawerType) as Drawer;
            if (drawer != null)
                _drawerCache[pairKey] = drawer;
            return drawer;
        }

        public void RedrawLayer(LBSLayer layer)
        {
            _view.ClearLayerContainer(layer);
            DrawLayer(layer);
        }

        public void RedrawLevel(LBSLevelData level)
        {
            foreach (var layer in level.Layers)
            {
                _view.ClearLayerContainer(layer);
            }
            DrawLevel(level);
        }

        private void DrawLevel(LBSLevelData level)
        {
            foreach (var layer in level.Layers)
            {
                DrawLayer(layer);
            }
        }
    }
}
