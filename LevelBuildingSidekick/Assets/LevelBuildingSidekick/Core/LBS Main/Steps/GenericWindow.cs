using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Overlays;

namespace LevelBuildingSidekick
{
    public class GenericWindow : EditorWindow, ISupportsOverlays
    {
        public System.Action draw;
        private void OnEnable()
        {
        }

        private void OnInspectorUpdate()
        {
        }

        private void OnGUI()
        {
            draw?.Invoke();
            Repaint();
        }

        private void OnDisable()
        {
            base.SaveChanges();
            //DestroyImmediate(this);
        }

        private void OnFocus()
        {
            Repaint();
        }

        private void OnLostFocus()
        {
            
        }
    }
}

