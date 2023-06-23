using LBS.Components;
using LBS.Tools.Transformer;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class LayerExtensions
{
    public static Dictionary<string, object> GetToolkit(this LBSLayer layer, List<LayerTemplate> layerTemplates)
    {
        Dictionary<string, object> choices = new Dictionary<string, object>();

        var template = layerTemplates.Where(t => t.layer.ID.Equals(layer.ID)).ToList()[0];
        template.modes.ForEach(m => choices.Add(m.name, m.toolkit));
        return choices;
    }

    public static List<Transformer> GetTrasformers(this LBSLayer layer, List<LayerTemplate> layerTemplates)
    {
        var template = layerTemplates.Where(t => t.layer.ID.Equals(layer.ID)).ToList()[0];
        return template.transformers;
    }

    public static LBSMode GetMode(this LBSLayer layer, List<LayerTemplate> layerTemplates, string mode)
    {
        var template = layerTemplates.Find(t => t.layer.ID.Equals(layer.ID));
        var toR = template.modes.Find(m => m.name.Equals(mode));
        return toR;
    }
}
