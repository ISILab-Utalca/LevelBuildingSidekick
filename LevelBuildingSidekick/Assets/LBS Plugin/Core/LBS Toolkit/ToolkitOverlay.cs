using LevelBuildingSidekick.Graph;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

namespace LevelBuildingSidekick
{
    public abstract class ToolkitOverlay : IMGUIOverlay
    {
    }


    [Overlay(typeof(LBSGraphWindow), "GraphToolkit", "Graph Toolkit", true)]
    public class GraphToolkitOverlay : ToolkitOverlay 
    {
        public static ToolkitView toolkit;

        public override void OnGUI()
        {
            if(toolkit != null)
            {
                toolkit.DrawEditor();
            }
        }
    }
}


