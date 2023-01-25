using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InspectorManager
{
    private VisualElement content;
    private List<LBSInspector> inspectors = new List<LBSInspector>();

    public InspectorManager(ref MainView mainView, VisualElement content)
    {
        this.content = content;
    }

    public void AddInspector(LBSInspector inspector)
    {
        inspectors.Add(inspector);
        content.Add(inspector);
    }

    public void RemoveInspector(LBSInspector inspector)
    {
        inspectors.Remove(inspector);
        content.Remove(inspector);
    }
}
