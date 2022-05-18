using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

namespace LevelBuildingSidekick
{
    [Overlay(typeof(StepWindow), "CustomToolkit", "Toolkit", true)]
    public class ToolkitOverlay : IMGUIOverlay
    {
        public static System.Action draw;

        public override void OnGUI()
        {
            draw?.Invoke();
        }
    }
}


