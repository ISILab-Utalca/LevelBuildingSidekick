using ISILab.AI.Optimization.Populations;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Modules;
using ISILab.LBS.VisualElements;
using ISILab.LBS.VisualElements.Editor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            UpdateLoadedTiles(behaviour, teselationSize, view);

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
                tView.style.display = (DisplayStyle)(behaviour.OwnerLayer.IsVisible ? 0 : 1);
                //view.AddElement(tView);
                view.AddElementToLayerContainer(behaviour.OwnerLayer, tile, tView);
                behaviour.Keys.Add(tile);
            }
        }

        public override void HideVisuals(object target, MainView view)
        {
            if (target is not PathOSBehaviour behaviour) return;
            foreach(PathOSTile tile in behaviour.Keys)
            {
                if (tile == null) continue;

                List<GraphElement> elements = view.GetElementsFromLayerContainer(behaviour.OwnerLayer, tile).Where(graphElement => graphElement != null).ToList();
                foreach(GraphElement element in elements)
                {
                    element.style.display = DisplayStyle.None; 
                }
            }
        }

        public override void ShowVisuals(object target, MainView view)
        {
            if (target is not PathOSBehaviour behaviour) return;
            foreach(PathOSTile tile in behaviour.Keys)
            {
                if (tile == null) continue;

                List<GraphElement> elements = view.GetElementsFromLayerContainer(behaviour.OwnerLayer, tile);
                foreach (GraphElement element in elements)
                {
                    element.style.display = DisplayStyle.Flex;
                }
            }
        }

        public override Texture2D GetTexture(object target, Rect sourceRect, Vector2Int teselationSize)
        {
            //return new Texture2D((int)(sourceRect.width * teselationSize.x), (int)(sourceRect.height * teselationSize.y));
            return null;
        }
    }
}
