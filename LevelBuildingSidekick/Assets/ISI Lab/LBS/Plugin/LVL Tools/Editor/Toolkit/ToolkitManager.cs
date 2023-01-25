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
    private InspectorManager InspectorManager;

    // ref data
    private LBSLevelData level;
    private LBSLayer layer;
    private LBSModule module;

    // event
    public event Action OnEndSomeAction;

    public ToolkitManager(ref ButtonGroup toolPanel,ref ModeSelector modeSelector, ref MainView view, ref InspectorManager inspectorManager , ref List<LayerTemplate> templates)
    {
        this.toolPanel = toolPanel;
        this.view = view;
        this.InspectorManager = inspectorManager;
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
            var btn = tool.InitButton(view, ref level, ref layer, ref module);
            btn.style.flexGrow = 1;
            toolPanel.Add(btn);
            tool.OnEndAction += OnEndSomeAction;

            if (!string.IsNullOrEmpty(tool.inspector))
            {
                Debug.Log("CCC");
                var insp = tool.InitInspector(view, ref level, ref layer, ref module);
                btn.OnFocusEvent += () => { InspectorManager.AddInspector(insp); Debug.Log("BBB"); };
                btn.OnBlurEvent += () => { InspectorManager.RemoveInspector(insp); };
            }
        }
    }

}
