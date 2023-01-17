using LBS.Components;
using LBS.Components.Graph;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LayerTemplate))]
public class LayerTemplateEditor : Editor
{
    void OnEnable()
    {
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var template = (LayerTemplate)target;

        if(GUILayout.Button("Set as interior"))
        {
            InteriorConstruct(template);
        }

        if (GUILayout.Button("Set as exterior"))
        {
            ExteriorConstruct(template);
        }

        if (GUILayout.Button("Set as population"))
        {
            PopulationConstruct(template);
        }
    }

    private void InteriorConstruct(LayerTemplate template)
    {
        var layer = new LBSLayer();

        // Modules
        layer.AddModule(new LBSBaseGraph());

        // Transformers
        //layer.AddTransformer();

        layer.ID = "Interior";
        layer.Name = "Interior layer";
        layer.iconPath = "Icons/interior-design";

        template.layer = layer;
        AssetDatabase.SaveAssets();
    }

    private void ExteriorConstruct(LayerTemplate template)
    {
        var layer = new LBSLayer();

        // Modules
        layer.AddModule(new LBSBaseGraph());

        // Transformers
        //layer.AddTransformer();

        layer.ID = "Exterior";
        layer.Name = "Exterior layer";
        layer.iconPath = "Icons/pine-tree";

        template.layer = layer;
        AssetDatabase.SaveAssets();
    }

    private void PopulationConstruct(LayerTemplate template)
    {
        var layer = new LBSLayer();

        // Modules
        layer.AddModule(new LBSBaseGraph());

        // Transformers
        //layer.AddTransformer();

        layer.ID = "Population";
        layer.Name = "Population layer";
        layer.iconPath = "Icons/ghost";

        template.layer = layer;
        AssetDatabase.SaveAssets();
    }
}
