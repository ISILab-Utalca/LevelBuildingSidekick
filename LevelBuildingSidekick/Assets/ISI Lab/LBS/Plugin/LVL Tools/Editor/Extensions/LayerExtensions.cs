using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class LayerExtensions
{
    public static Dictionary<string, object> GetModes(this LBSLayer layer, List<LayerTemplate> layerTemplates)
    {
        Dictionary<string, object> choices = new Dictionary<string, object>();

        var template = layerTemplates.Where(t => t.layer.ID.Equals(layer.ID)).ToList()[0];
        template.modes.ForEach(m => choices.Add(m.name, m.toolkit));
        return choices;
    }
}
