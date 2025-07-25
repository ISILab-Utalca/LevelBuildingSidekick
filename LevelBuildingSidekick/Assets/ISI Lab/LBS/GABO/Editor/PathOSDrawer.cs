using ISILab.AI.Optimization.Populations;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Modules;
using ISILab.LBS.VisualElements;
using ISILab.LBS.VisualElements.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Drawers
{
    [Drawer(typeof(PathOSBehaviour))]
    public class PathOSDrawer : Drawer
    {
        public override void Draw(object target, MainView view, Vector2 teselationSize)
        {
            PathOSBehaviour behaviour = target as PathOSBehaviour;

            if (behaviour == null) { return; }

            PaintNewTiles(behaviour, teselationSize, view);
            //UpdateLoadedTiles(behaviour, teselationSize, view);

            if(!Loaded || FullRedrawRequested)
            {
                LoadAllTiles(behaviour, teselationSize, view);
                Loaded = true;
                FullRedrawRequested = false;
            }
        }

        private void PaintNewTiles(PathOSBehaviour behaviour, Vector2 teselationSize, MainView view)
        {
            foreach(PathOSTile tile in behaviour.RetrieveNewTiles())
            {
                var tView = new PathOSTileView(tile);
                Vector2 pos = new Vector2(tile.X, -tile.Y);
                Vector2 size = DefalutSize * teselationSize;
                tView.SetPosition(new Rect(pos * size, size));
                //view.AddElement(tView);
                view.AddElementToLayerContainer(behaviour.OwnerLayer, tile, tView);
            }
        }

        private void UpdateLoadedTiles(PathOSBehaviour behaviour, Vector2 teselationSize, MainView view)
        {
            behaviour.Keys.RemoveWhere(item => item == null);

            foreach(object obj in behaviour.Keys)
            {
                if (obj is not PathOSTile tile) continue;

                List<GraphElement> elements = view.GetElementsFromLayerContainer(behaviour.OwnerLayer, tile);
                if (elements == null) continue;

                foreach(GraphElement element in elements)
                {
                    var tView = (PathOSTileView) element;

                    if(tView == null) continue;
                    if(!tView.visible) continue;

                    Vector2 pos = new Vector2(tile.X, -tile.Y);
                    Vector2 size = DefalutSize * teselationSize;

                    tView.SetPosition(new Rect(pos * size, size));

                    tView.layer = behaviour.OwnerLayer.index;
                }
            }
        }

        private void LoadAllTiles(PathOSBehaviour behaviour, Vector2 teselationSize, MainView view)
        {
            foreach (PathOSTile tile in behaviour.Tiles)
            {
                var tView = new PathOSTileView(tile);
                //var size = behaviour.OwnerLayer.TileSize * LBSSettings.Instance.general.TileSize;
                Vector2 pos = new Vector2(tile.X, -tile.Y);
                Vector2 size = DefalutSize * teselationSize;
                tView.SetPosition(new Rect(pos * size, size));
                //view.AddElement(tView);
                view.AddElementToLayerContainer(behaviour.OwnerLayer, tile, tView);
                behaviour.Keys.Add(tile);
            }
        }

        public override void HideVisuals(object target, MainView view)
        {
            try
            {
                throw new System.NotImplementedException();
            }
            catch (System.NotImplementedException e)
            {
                Debug.LogError(e);
            }
        }

        public override void ShowVisuals(object target, MainView view)
        {
            try
            {
                throw new System.NotImplementedException();
            }
            catch (System.NotImplementedException e)
            {
                Debug.LogError(e);
            }
        }
    }
}
