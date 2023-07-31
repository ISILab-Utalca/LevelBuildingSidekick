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

    public ToolkitManager(ref ButtonGroup toolPanel, ref MainView view, ref LBSInspectorPanel inspectorManager , ref List<LayerTemplate> templates)
    {
        this.toolPanel = toolPanel;
        this.view = view;
        this.InspectorManager = inspectorManager;
    }

    public void SetTools(object tools, ref LBSLayer layer, ref LBSBehaviour behaviour)
    {
        ClearTools();

        currentTools = tools as List<LBSTool>;
        foreach (var tool in currentTools)
        {
            tool.OnEndAction += OnEndSomeAction;

            var btn = tool.InitButton(view, layer, behaviour);
            btn.style.flexGrow = 1;
            toolPanel.Add(btn);

            if (!string.IsNullOrEmpty(tool.inspector))
            {
                var insp = tool.InitInspector(view, layer, behaviour);
                insp.style.flexGrow = 1;
                btn.OnFocusEvent += () => {
                    InspectorManager.AddInspector(insp,0); 
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

    internal void OnSelectedLayerChange(LBSLayer layer)
    {
        ClearTools();
        //var manipulators = layer.Behaviours.SelectMany(b => b.);
       
    }
}
