using LBS.Windows;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;

[Overlay(typeof(LBSPopulationWindow), ID, "Population brush", "-", defaultDisplay = true)]
public class BrushOverlay : Overlay
{
    private const string ID = "PopulationBrushOverlay";

    private BrushView preview;
    private VisualElement brushesPanel;

    public override VisualElement CreatePanelContent()
    {
        var root = new VisualElement();
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("BrushWindowUXML");
        visualTree.CloneTree(root);

        preview = root.Q<BrushView>("Preview");
        brushesPanel = root.Q<VisualElement>("BrushPanel");


        ActualizeBrushPanel(null);
        return root;
    }

    private void ActualizeBrushPanel(List<string> tags)
    {
        var stamps = DirectoryTools.GetScriptables<StampPresset>().ToList();
        brushesPanel.Clear();

        foreach (var stamp in stamps)
        {
            AddBrush(stamp);
        }
    }

    private void AddBrush(StampPresset presset)
    {
        var brush = new BrushView(presset);
        brush.style.height = brush.style.width = 50;
        brush.clicked += () => {
            preview.SetValue(presset);
            BrushWindow.SelectedStamp = presset;
        };
        brushesPanel.Add(brush);
    }
}
