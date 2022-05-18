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

        }

        private void OnGUI()
        {
            draw?.Invoke();
        }
    }
}

