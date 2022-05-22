using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Overlays;

namespace LevelBuildingSidekick
{
    public class StepWindow : EditorWindow, ISupportsOverlays
    {
        public System.Action draw;
        private void OnEnable()
        {
            minSize = new Vector2(100,100);
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }

        private void OnGUI()
        {
            draw?.Invoke();
        }

        private void OnDisable()
        {
            base.SaveChanges();
            //DestroyImmediate(this);
        }

        
    }
}

