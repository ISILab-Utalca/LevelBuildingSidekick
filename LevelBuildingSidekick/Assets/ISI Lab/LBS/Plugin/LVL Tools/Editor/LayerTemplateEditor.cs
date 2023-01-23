using LBS.Components;
using LBS.Components.Graph;
using LBS.Components.Specifics;
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
        layer.AddModule(new LBSRoomGraph());

        // Transformers
        //layer.AddTransformer();

        layer.ID = "Interior";
        layer.Name = "Interior layer";
        layer.iconPath = "Icons/interior-design";

        template.layer = layer;



        // Mode 1
        Texture2D icon = Resources.Load<Texture2D>("Icons/Select");
        var tool1 = new LBSTool(icon, "Select", typeof(Empty), null, true);
        icon = Resources.Load<Texture2D>("Icons/Addnode");
        var tool2 = new LBSTool(icon, "Add node",typeof(CreateNewRoomNode), null, false);
        icon = Resources.Load<Texture2D>("Icons/AddConnection");
        var tool3 = new LBSTool(icon, "Add conection", typeof(CreateNewRoomNode), null, false);


        var mode1 = new LBSMode("Graph", new DrawSimpleGraph(), new List<LBSTool>() { tool1, tool2, tool3 });
        template.modes.Add(mode1);

        // Mode 2
        var tool4 = new LBSTool(icon, "Select", typeof(Empty), null);

        var mode2 = new LBSMode("Schema", new DrawSimpleGraph(),new List<LBSTool>() { tool4 });
        template.modes.Add(mode2);

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