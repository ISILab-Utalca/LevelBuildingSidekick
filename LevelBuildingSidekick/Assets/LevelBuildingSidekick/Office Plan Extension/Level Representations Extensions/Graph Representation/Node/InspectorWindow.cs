using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LevelBuildingSidekick;

namespace LevelBuildingSidekick
{
    public class InspectorWindow : EditorWindow
    {
        public Controller controller { get; set; }
        private void OnEnable()
        {
        }

        private void OnInspectorUpdate()
        {
            //Repaint();
        }

        private void OnGUI()
        {
            if (controller.Data != null)
            {
                controller.View.DrawEditor();
            }
            else
            {
            }
        }

        private void OnDisable()
        {
            //base.SaveChanges();
            //DestroyImmediate(this);
        }
    }
}

