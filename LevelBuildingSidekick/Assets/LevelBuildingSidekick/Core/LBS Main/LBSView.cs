using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LevelBuildingSidekick
{
    public class LBSView : View
    {
        public LBSWindow Window { get; set; }

        public LBSView(Controller controller):base(controller)
        {
            Window = new LBSWindow();
        }
        public override void Draw()
        {
        }

        public override void Display()
        {
            Window.Show();
        }
    }

    public class LBSWindow : EditorWindow
    {
        public void OnEnable()
        {
            
        }
        private void OnGUI()
        {
            
        }
    }
}

