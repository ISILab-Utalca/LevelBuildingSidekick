using ISILab.AI.Optimization.Populations;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Modules;
using ISILab.LBS.VisualElements;
using ISILab.LBS.VisualElements.Editor;
//using LBS.Settings;
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
        // GABO TODO: TERMINAR
        public override void Draw(object target, MainView view, Vector2 teselationSize)
        {
            PathOSBehaviour behaviour = target as PathOSBehaviour;

            if (behaviour == null) { return; }

            foreach (var tile in behaviour.Tiles)
            {
                var v = new PathOSTileView(tile);
                var size = behaviour.OwnerLayer.TileSize;// * LBSSettings.Instance.general.TileSize;
                var p = new Vector2(tile.X, -tile.Y);
                v.SetPosition(new Rect(p * size, size));
                view.AddElement(v);
            }
        }
    }
}
