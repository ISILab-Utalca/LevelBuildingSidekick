using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LevelBuildingSidekick
{
    public abstract class LevelRepresentationView : View
    {
        List<string> Windows = new List<string>();
        protected LevelRepresentationView(Controller controller) : base(controller)
        {
        }

        public override void Draw2D()
        {
            var controller = Controller as LevelRepresentationController;
            if(controller.Toolkit != null)
            {
                (controller.Toolkit.View as View).Draw2D();
            }
        }
    }
}

