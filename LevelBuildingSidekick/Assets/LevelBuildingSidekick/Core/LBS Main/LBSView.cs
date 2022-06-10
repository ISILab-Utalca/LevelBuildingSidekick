using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LevelBuildingSidekick
{
    public class LBSView : View
    {
        public LBSView(Controller controller):base(controller)
        {
            //Window = EditorWindow.GetWindow<LBSWindow>();
        }
        public override void Draw2D()
        {
        }
    }
}

