using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.VisualElements;
using System.Linq;
using System;

public class ToolkitManager
{
    // VisualElement references
    private ButtonGroup toolPanel;
    private MainView view;

    // ref data
    private LBSLevelData level;
    private LBSLayer layer;
    private LBSModule module;

    // event
    public event Action OnEndSomeAction;

    public ToolkitManager(ref ButtonGroup toolPanel,ref ModeSelector modeSelector, ref MainView view , ref List<LayerTemplate> templates)
    {
        this.toolPanel = toolPanel;
        this.view = view;
    }

    public void SetTools(object tools, ref LBSLevelData level ,ref LBSLayer layer, ref LBSModule module)
    {
        this.level = level;
        this.layer = layer;
        this.module = module;

        toolPanel.Clear();
        var _tools = tools as List<LBSTool>;
        foreach (var tool in _tools)
        {
            var btn = tool.GetButton(view);
            tool.InitManipulator(ref view,ref level, ref layer, ref module);
            btn.style.flexGrow = 1;
            toolPanel.Add(btn);
            tool.OnEndAction += OnEndSomeAction;
        }
    }

}
