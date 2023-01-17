using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.VisualElements;
using System.Linq;

public class ToolkitManager
{
    // VisualElement references
    private ButtonGroup toolPanel;
    private ModeSelector modeSelector;

    // Templates
    private List<LayerTemplate> templates;

    //
    private LBSLayer layer;
    private string mode;

    public ToolkitManager(ref ButtonGroup toolPanel, ref ModeSelector modeSelector,ref List<LayerTemplate> templates)
    {
        this.toolPanel = toolPanel;
        this.modeSelector = modeSelector;
        this.templates = templates;
    }

    public void SetTool(ref LBSLayer selected, ref string mode)
    {
        var layer = selected;
        var template = templates.Where(t => t.layer.ID.Equals(layer.ID)).ToList()[0];


        Debug.Log("change layer: " + selected.Name);
    }
}
