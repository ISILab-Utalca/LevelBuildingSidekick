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
        public System.Action init;
        public System.Action close;
        public System.Action onFocus;

        private void Awake()
        {
            init?.Invoke();
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
            onFocus?.Invoke();
            Repaint();
        }

        private void OnLostFocus()
        {
            base.SaveChanges();
        }

        private void OnDestroy()
        {
            close?.Invoke();
        }
    }
}

