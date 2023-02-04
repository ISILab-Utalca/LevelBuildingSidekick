using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.VisualElements;
using System.Linq;
using System;

public class ToolkitManager
{
    private List<LBSTool> currentTools = new List<LBSTool>();

    // VisualElement references
    private ButtonGroup toolPanel;
    private MainView view;
    private LBSInspectorPanel InspectorManager;

    // event
    public event Action OnEndSomeAction;

    public ToolkitManager(ref ButtonGroup toolPanel,ref ModeSelector modeSelector, ref MainView view, ref LBSInspectorPanel inspectorManager , ref List<LayerTemplate> templates)
    {
        this.toolPanel = toolPanel;
        this.view = view;
        this.InspectorManager = inspectorManager;
    }

    public void SetTools(object tools, ref LBSLevelData level ,ref LBSLayer layer, ref LBSModule module)
    {
        ClearTools();

        currentTools = tools as List<LBSTool>;
        foreach (var tool in currentTools)
        {
            tool.OnEndAction += OnEndSomeAction;

            var btn = tool.InitButton(view, ref level, ref layer, ref module);
            btn.style.flexGrow = 1;
            toolPanel.Add(btn);

            if (!string.IsNullOrEmpty(tool.inspector))
            {
                var insp = tool.InitInspector(view, ref level, ref layer, ref module);
                insp.style.flexGrow = 1;
                btn.OnFocusEvent += () => {
                    InspectorManager.AddInspector(insp); 
                };
                btn.OnBlurEvent += () => {
                    InspectorManager.RemoveInspector(insp);
                };
            }
        }
    }

    public void ClearTools()
    {
        foreach (var tool in currentTools)
        {
            tool.OnEndAction -= OnEndSomeAction;
        }

        toolPanel.Clear();
    }

}
