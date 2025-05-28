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
            var lbsLayer = layer;
            if (lbsLayer == null)
                return;
            
            if (!lbsLayer.IsVisible)
                return;
            
            var behaviours = lbsLayer.Behaviours;
            foreach (var behaviour in behaviours)
            {
                if (behaviour == null)
                    continue;

                if (!behaviour.visible)
                    continue;

                var drawerT = LBS_Editor.GetDrawer(behaviour.GetType());

                if (drawerT == null)
                    continue;

                var drawer = Activator.CreateInstance(drawerT) as Drawer;

                drawer?.Draw(behaviour, view, lbsLayer.TileSize);
            }

            var assistants = lbsLayer.Assistants;
            foreach (var assistant in assistants)
            {
                if (assistant == null)
                    continue;

                if (!assistant.visible)
                    continue;

                var drawerT = LBS_Editor.GetDrawer(assistant.GetType());

                if (drawerT == null)
                    continue;

                var drawer = Activator.CreateInstance(drawerT) as Drawer;

                drawer?.Draw(assistant, view, lbsLayer.TileSize);
            }
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

        private void DrawLevel(LBSLevelData level, MainView MainView)
        {
            this.level = level;
            view = MainView;

            var layers = level.Layers;
            foreach (var layer in layers)
            {
                DrawLayer(layer);
            }
        }
    }
}
