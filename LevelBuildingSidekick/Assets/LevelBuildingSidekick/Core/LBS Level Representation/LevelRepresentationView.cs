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

        public override void Display()
        {
            Draw();
        }

        public override void Draw()
        {
            var controller = Controller as LevelRepresentationController;
            controller.Toolkit.View.Display();
        }
    }
}

