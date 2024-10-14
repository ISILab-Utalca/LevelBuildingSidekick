using ISILab.LBS.Behaviours;
using ISILab.LBS.VisualElements.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace ISILab.LBS.Drawers
{
    [Drawer(typeof(PathOSBehaviour))]
    public class PathOSDrawer : Drawer
    {
        // GABO TODO: TERMINAR
        public override void Draw(object target, MainView view, Vector2 teselationSize)
        {
            PathOSBehaviour behaviour = target as PathOSBehaviour;

            if (behaviour != null) { return; }

            //foreach ()
            //{

            //}

            throw new System.NotImplementedException();
        }
    }
}
