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

    public void RefreshView(ref LBSLayer layer,List<LBSLayer> allLayers, string modeName)
    {
        var _layer = layer;
        var template = templates.Find(t => t.layer.ID.Equals(_layer.ID));
        var mode = template.modes.Find(m => m.name.Equals(modeName));

        // clear
        view.ClearView();

        var _allLayers = new List<LBSLayer>(allLayers);
        foreach (var otherLayer in _allLayers)
        {
            if (!otherLayer.IsVisible)
                continue;

            if (otherLayer == _layer)
            {
                mode.Drawer.Draw(ref _layer, view);
            }
            else
            {
                var oTemplate = templates.Find(t => t.layer.ID.Equals(otherLayer.ID));
                var oMode = oTemplate.modes[oTemplate.modes.Count - 1];
                var _other = otherLayer;
                oMode.Drawer.Draw(ref _other, view);
            }
        }

    }
}
