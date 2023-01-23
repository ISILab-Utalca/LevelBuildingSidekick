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


    public void RefreshView(ref LBSLayer layer, string modeName)
    {
        var _layer = layer;
        var template = templates.Find(t => t.layer.ID.Equals(_layer.ID));
        var mode = template.modes.Find(m => m.name.Equals(modeName));

        view.ClearView();
        mode.Drawer.Draw(ref _layer, view);
    }
}
