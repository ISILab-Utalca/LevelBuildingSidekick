using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawManager
{
    private MainView view;
    private List<LayerTemplate> templates;

    public DrawManager(ref MainView view, ref List<LayerTemplate> templates)
    {
        this.view = view;
        this.templates = templates;
    }

    public void RefreshView(ref LBSLayer layers)
    {
        //layers.ID
    }
}
