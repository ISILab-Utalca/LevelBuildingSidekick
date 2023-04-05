using LBS.Components;
using LBS.Components.Graph;
using LBS.Components.Specifics;
using LBS.Components.TileMap;
using LBS.Generator;
using LBS.Tools.Transformer;
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

    /// <summary>
    /// 
    /// This function adjust the icons, layout and labels of the system for Contructión in interior
    /// also call the manipulators to make functional buttons in the layout
    /// </summary>
    /// <param name="template"></param>
    private void InteriorConstruct(LayerTemplate template)
    {
        // Basic data layer
        var layer = new LBSLayer();
        var assist = Utility.DirectoryTools.GetScriptable<LBSLayerAssistant>("SchemaAssitant");
        if(assist == null)
        {
            assist = ScriptableObject.CreateInstance<LBSLayerAssistant>();
            assist.name = "SchemaAssitant";
            assist.AddAgent(new SchemaHCAgent(layer, "SchemaHillClimbing"));
            assist.Generator = new SchemaGenerator();

            AssetDatabase.AddObjectToAsset(assist, template);
            AssetDatabase.SaveAssets();
        }

        layer.Assitant = assist;
        layer.ID = "Interior";
        layer.Name = "Layer Interior";
        layer.iconPath = "Icons/interior-design";
        template.layer = layer;

        // Modules
        layer.AddModule(new LBSRoomGraph());    // GraphModule<RoomNode>
        layer.AddModule(new LBSSchema());       // AreaTileMap<TiledArea<ConnectedTile>, ConnectedTile>
        //layer.AddModule(new GraphModule<RoomNode>());
        //layer.AddModule(new LBSSchema());

        // Transformers
        template.transformers.Add(
            new GraphToArea(
                typeof(GraphModule<RoomNode>),
                typeof(AreaTileMap<TiledArea>)
                )
            );

        template.transformers.Add(
            new AreaToGraph(
                typeof(AreaTileMap<TiledArea>),
                typeof(GraphModule<RoomNode>)
                )
            );

        // Mode 1
        Texture2D icon = Resources.Load<Texture2D>("Icons/Select");
        var tool1 = new LBSTool(icon, "Select", typeof(Select), null, true);
        icon = Resources.Load<Texture2D>("Icons/Addnode");
        var tool2 = new LBSTool(icon, "Add node", typeof(CreateNewRoomNode), null, false);
        icon = Resources.Load<Texture2D>("Icons/AddConnection");
        var tool3 = new LBSTool(icon, "Add conection", typeof(CreateNewConnection<RoomNode>), null, false);
        icon = Resources.Load<Texture2D>("Icons/Trash");
        var tool4 = new LBSTool(icon, "Remove", typeof(RemoveGraphNode<RoomNode>), null, false);

        var mode1 = new LBSMode(
            "Graph",
            typeof(GraphModule<RoomNode>)
            , new DrawSimpleGraph(),
            new List<LBSTool>() { tool1, tool2, tool3, tool4 }
            );
        template.modes.Add(mode1);

        // Mode 2
        icon = Resources.Load<Texture2D>("Icons/Select");
        var tool5 = new LBSTool(icon, "Select", typeof(Select), null, true);

        icon = Resources.Load<Texture2D>("Icons/paintbrush");
        var tool6 = new LBSTool(
            icon,
            "Paint tile",
            typeof(AddTileToTiledArea<TiledArea, ConnectedTile>),
            typeof(RoomsPalleteInspector<TiledArea, ConnectedTile>),
            true);

        /*
        icon = Resources.Load<Texture2D>("Icons/paintbrush"); 
        var tool6 = new LBSMultiTool(
            icon,
            "Paint tile",
            new List<string>() { "point", "Line", "Grid","Free" },
            new List<System.Type>() { 
                typeof(AddTileToTiledAreaAtPoint<TiledArea,ConnectedTile>), // point // (!!) implementar
                typeof(AddTileToTiledAreaAtLine<TiledArea,ConnectedTile>), // line // (!!) implementar
                typeof(AddTileToTiledAreaAtGrid<TiledArea,ConnectedTile>), // grid // (!!) implementar
                typeof(AddTileToTiledAreaAtFree<TiledArea,ConnectedTile>)  // free // (!!) implementar
            },
            typeof(RoomsPalleteInspector<TiledArea, ConnectedTile>)
        );
        */

        icon = Resources.Load<Texture2D>("Icons/erased");
        var tool7 = new LBSTool(
            icon,
            "Erase",
            typeof(DeleteTile<TiledArea, ConnectedTile>), // Removed<TiledArea<LBSTile>, LBSTile>,
            null
        );

        icon = Resources.Load<Texture2D>("Icons/open-exit-door");
        var tool8 = new LBSTool(icon, "Add door", typeof(AddDoor<TiledArea,ConnectedTile>), null, true);

        icon = Resources.Load<Texture2D>("Icons/erased");
        var tool9 = new LBSTool(
            icon, 
            "Remove door",
            typeof(RemoveDoor<TiledArea,ConnectedTile>), //typeof(RemoveDoor<TiledArea,ConnectedTile>),
            null, 
            true);

        var mode2 = new LBSMode(
            "Schema",
            typeof(AreaTileMap<TiledArea>),
            new DrawConnectedTilemap(),
            new List<LBSTool>() { tool5, tool6, tool7, tool8, tool9 }
            );
        template.modes.Add(mode2);

        AssetDatabase.SaveAssets();
    }

    /// <summary>
    /// This function adjust the icons, layout and labels of the system for Contructión in exterior
    /// also call the manipulators to make functional buttons in the layout
    /// </summary>
    /// <param name="template"></param>
    private void ExteriorConstruct(LayerTemplate template)
    {
        // Basic data layer
        var layer = new LBSLayer();
        var assist = Utility.DirectoryTools.GetScriptable<LBSLayerAssistant>("ExteriorAsstant");
        if (assist == null)
        {
            assist = ScriptableObject.CreateInstance<LBSLayerAssistant>();
            assist.name = "ExteriorAsstant";
            assist.Generator = new ExteriorGenerator();

            AssetDatabase.AddObjectToAsset(assist, template);
            AssetDatabase.SaveAssets();
        }

        layer.Assitant = assist;
        layer.ID = "Exterior";
        layer.Name = "Layer Exterior";
        layer.iconPath = "Icons/pine-tree";
        template.layer = layer;

        // Modules
        var x = new Exterior();
        layer.AddModule(x);

        // Transformers
        //
        //

        // Mode 1
        Texture2D icon = Resources.Load<Texture2D>("Icons/Select");
        var tool1 = new LBSTool(icon, "Select", typeof(Select), null, true);


        icon = Resources.Load<Texture2D>("Icons/RoomSelection");
        var tool2 = new LBSTool(
            icon,
            "Add empty tile",
            typeof(AddEmptyTile<ConnectedTile>),
            null,
            false);

        icon = Resources.Load<Texture2D>("Icons/RoomSelection");
        var tool3 = new LBSTool(
            icon,
            "Remove tile",
            typeof(RemoveTileExterior<ConnectedTile>),
            null,
            false);

        icon = Resources.Load<Texture2D>("Icons/AddConnection");
        var tool4 = new LBSTool(
            icon,
            "Set connection",
            typeof(AddConnection<ConnectedTile>),
            typeof(TagsPalleteInspector), //typeof(RoomsPalleteInspector<TiledArea, ConnectedTile>),
            false);

        icon = Resources.Load<Texture2D>("Icons/erased");
        var tool5 = new LBSTool(
            icon,
            "Remove connection",
            typeof(RemoveConnection<ConnectedTile>), 
            null, 
            false);

        icon = Resources.Load<Texture2D>("Icons/Collapse_Icon");
        var tool6 = new LBSTool(
            icon, 
            "Collapse connection area", 
            typeof(WaveFunctionCollapseManipulator<ConnectedTile>), 
            null, 
            false);

        var mode1 = new LBSMode(
            "Exterior",
            typeof(TiledArea), // (!!!) implentar la correcta
            new DrawExterior(),
            new List<LBSTool>() { tool1, tool2, tool3, tool4, tool5, tool6 }
            );


        template.modes.Add(mode1);

        AssetDatabase.SaveAssets();
    }

    /// <summary>
    /// 
    /// This function adjust the icons, layout and labels of the Population system
    /// also call the manipulators to make functional buttons in the layout
    /// </summary>
    /// <param name="template"></param>
    private void PopulationConstruct(LayerTemplate template)
    {
        // Basic data layer
        var layer = new LBSLayer();

        var assist = Utility.DirectoryTools.GetScriptable<LBSLayerAssistant>("PopulationAssitant");
        if (assist == null)
        {
            assist = ScriptableObject.CreateInstance<LBSLayerAssistant>();
            assist.name = "PopulationAssitant";
            assist.Generator = new PopulationGenerator();
            assist.AddAgent(new PopulationMapEliteAgent(layer, "Population Map Elite"));

            AssetDatabase.AddObjectToAsset(assist, template);
            AssetDatabase.SaveAssets();
        }

        layer.Assitant = assist;
        layer.ID = "Population";
        layer.Name = "Layer Population";
        layer.iconPath = "Icons/ghost";
        template.layer = layer;

        // Modules
        layer.AddModule(new LBSTileMap());
        layer.AddModule(new TaggedTileMap());
        // Transformers
        //
        //

        // Select
        Texture2D icon = Resources.Load<Texture2D>("Icons/Select");
        var tool1 = new LBSTool(icon, "Select", typeof(Select), null, true);

        //Add
        icon = Resources.Load<Texture2D>("Icons/paintbrush");
        var tool2 = new LBSTool(icon, "Add Tile", typeof(AddTaggedTile), typeof(BundlePalleteInspector), false);

        //Remove
        icon = Resources.Load<Texture2D>("Icons/Trash");
        var tool3 = new LBSTool(icon, "Remove", typeof(RemoveTile), null, false);
        

        var mode1 = new LBSMode(
            "Population",
            //Change to pop
            //Check if 'PopulationTileMap<TiledArea> works
            typeof(TaggedTileMap), 
            new DrawTaggedTileMap(),
            new List<LBSTool>() { tool1, tool2, tool3}
            );
        template.modes.Add(mode1);

        AssetDatabase.SaveAssets();
    }
}
