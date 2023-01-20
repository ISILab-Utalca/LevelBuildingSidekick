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
    private MainView view;

    // Templates
    private List<LayerTemplate> templates;

    // ref data
    private LBSLevelData level;
    private LBSLayer layer;
    private LBSModule module;

    public ToolkitManager(ref ButtonGroup toolPanel, ref ModeSelector modeSelector, ref MainView view , ref List<LayerTemplate> templates)
    {
        this.toolPanel = toolPanel;
        this.modeSelector = modeSelector;
        this.modeSelector.OnSelectionChange += SelectionChange;
        this.view = view;

        this.templates = templates;
    }

    public void SetTool(ref LBSLevelData level ,ref LBSLayer layer, ref LBSModule? module)
    {
        this.level = level;
        this.layer = layer;
        this.module = module;
        var template = templates.Where(t => t.layer.ID.Equals(this.layer.ID)).ToList()[0];

        Dictionary<string, object> choices = new Dictionary<string, object>();
        template.modes.ForEach(m => choices.Add(m.name, m.toolkit));
        modeSelector.SetChoices(choices);
        modeSelector.Index = 0;
    }

    public void SelectionChange(string name)
    {
        toolPanel.Clear();
        var obj = this.modeSelector.GetSelection(name);
        var tools = obj as List<LBSTool>;
        foreach (var tool in tools)
        {
            var btn = tool.GetButton(view);
            tool.InitManipulator(ref level, ref layer, ref module);
            btn.style.flexGrow = 1;
            toolPanel.Add(btn);
        }
        
    }
}
