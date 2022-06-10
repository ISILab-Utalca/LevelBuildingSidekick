using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LevelBuildingSidekick
{
    public abstract class LevelRepresentationView : View
    {
        protected LevelRepresentationView(Controller controller) : base(controller)
        {
        }

        public override void Draw2D()
        {
            var controller = Controller as LevelRepresentationController;
            controller.Toolkit.View.Draw2D();
        }
    }
}

