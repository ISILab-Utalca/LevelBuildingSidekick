using LBS.Assisstants;
using LBS.Behaviours;
using LBS.Components;
using LBS.Components.Graph;
using LBS.Components.Specifics;
using LBS.Components.TileMap;
using LBS.Generator;
using LBS.Tools.Transformer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[LBSCustomEditor("Layer template",typeof(LayerTemplate))]
[CustomEditor(typeof(LayerTemplate))]
public class LayerTemplateEditor : Editor
{
    private int behaviourIndex = 0;
    private List<Type> behaviourOptions;

    private int assitantIndex = 0;
    private List<Type> assistantOptions;

    private int ruleIndex = 0;
    private List<Type> ruleOptions; 

    void OnEnable()
    {
        behaviourOptions = typeof(LBSBehaviour).GetDerivedTypes().ToList();
        assistantOptions = typeof(LBSAssistantAI).GetDerivedTypes().ToList();
        ruleOptions = typeof(LBSGeneratorRule).GetDerivedTypes().ToList();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var template = (LayerTemplate)target;

        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        behaviourIndex = EditorGUILayout.Popup("Type:", behaviourIndex, behaviourOptions.Select(e => e.Name).ToArray());
        var selected = behaviourOptions[behaviourIndex];
        if (GUILayout.Button("Add behaviour"))
        {
            var bh = Activator.CreateInstance(selected, null, "Default Name");
            template.layer.AddBehaviour(bh as LBSBehaviour);
        }
        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal();
        assitantIndex = EditorGUILayout.Popup("Type:", assitantIndex, assistantOptions.Select(e => e.Name).ToArray());
        var selected2 = assistantOptions[assitantIndex];
        if (GUILayout.Button("Add Assistent"))
        {
            var ass = Activator.CreateInstance(selected2);
            template.layer.AddAssistant(ass as LBSAssistantAI);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        ruleIndex = EditorGUILayout.Popup("Type:", ruleIndex, ruleOptions.Select(e => e.Name).ToArray());
        var selected3 = ruleOptions[ruleIndex];
        if (GUILayout.Button("Add Assistent"))
        {
            var rule = Activator.CreateInstance(selected3);
            template.layer.AddGeneratorRule(rule as LBSGeneratorRule);
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(20);

        if (GUILayout.Button("Set as Interior")) 
        {
            InteriorConstruct(template);
        }

        if (GUILayout.Button("Set as Exterior"))
        {
            ExteriorConstruct(template);
        }

        if (GUILayout.Button("Set as Population"))
        {
            PopulationConstruct(template);
        }

        if (GUILayout.Button("Set as Quest"))
        {
            Questconstuct(template);
        }
    }

    /// <summary>
    /// This function adjust the icons, layout and labels of the system for Contructión in interior
    /// also call the manipulators to make functional buttons in the layout
    /// </summary>
    /// <param name="template"></param>
    private void InteriorConstruct(LayerTemplate template)
    {
        template.Clear();

        // Basic data layer
        var layer = template.layer;
        layer.ID = "Interior";
        layer.Name = "Layer Interior";
        layer.iconPath = "Assets/ISI Lab/LBS/Plugin/Assets2D/Resources/Icons/interior-design.png";
        template.layer = layer;

        // Modules
        // layer.AddModule(new LBSRoomGraph());    // GraphModule<RoomNode>
        // layer.AddModule(new LBSSchema());        // AreaTileMap<TiledArea<ConnectedTile>, ConnectedTile>

        // Behaviours
        var bhIcon = Resources.Load<Texture2D>("Icons/Select");
        var bh = new SchemaBehaviour(bhIcon, "Schema behaviour");
        bh.OnAdd(layer);
        layer.AddBehaviour(bh);

        // Assistants
        layer.AddAssistant(new AssistantHillClimbing());
        
        // Generators
        layer.AddGeneratorRule(new SchemaRuleGenerator());
        layer.AddGeneratorRule(new SchemaRuleGeneratorExteriror());

        // Seting Generator
        layer.Settings = new Generator3D.Settings()
        {
            scale = new Vector2Int(2, 2),
            resize = new Vector2(0, 0),
            position = new Vector3(0, 0, 0),
            name = "Interior",
        };

        /*
        // Transformers
        template.transformers.Add(
            new GraphToArea(
                typeof(GraphModule<RoomNode>),
                typeof(AreaTileMap<TiledArea>)
                )
            );

        layer.AddBehaviour(new SchemaBehaviour());

        // Mode 1
        Texture2D icon = Resources.Load<Texture2D>("Icons/Select");
        var tool1 = new LBSTool(icon, "Select", typeof(Select), null, true);
        icon = Resources.Load<Texture2D>("Icons/Addnode");
        var tool2 = new LBSTool(icon, "Add Node", typeof(CreateNewRoomNode), null, false);
        icon = Resources.Load<Texture2D>("Icons/AddConnection");
        var tool3 = new LBSTool(icon, "Add conection", typeof(CreateNewConnection<RoomNode>), null, false);
        icon = Resources.Load<Texture2D>("Icons/Trash");
        var tool4 = new LBSTool(icon, "Remove Node", typeof(RemoveGraphNode<RoomNode>), null, false);
        icon = Resources.Load<Texture2D>("Icons/Trash");
        var tool10 = new LBSTool(icon, "Remove Connection", typeof(RemoveGraphConnection), null, false);

        var mode1 = new LBSMode(
            "Graph",
            typeof(GraphModule<RoomNode>)
            , new DrawSimpleGraph(),
            new List<LBSTool>() { tool1, tool2, tool3, tool4, tool10 }
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
        template.modes.Add(mode2);*/

        Debug.Log("Set Interior Default");
        AssetDatabase.SaveAssets();
    }

    /// <summary>
    /// This function adjust the icons, layout and labels of the system for Contructión in exterior
    /// also call the manipulators to make functional buttons in the layout
    /// </summary>
    /// <param name="template"></param>
    private void ExteriorConstruct(LayerTemplate template)
    {
        template.Clear();

        // Basic data layer
        var layer = template.layer;
        layer.ID = "Exterior";
        layer.Name = "Layer Exterior";
        layer.iconPath = "Assets/ISI Lab/LBS/Plugin/Assets2D/Resources/Icons/pine-tree.png";
        template.layer = layer;

        var bhIcon = Resources.Load<Texture2D>("Icons/Select");
        var bh = new ExteriorBehaviour(bhIcon, "Schema behaviour");
        bh.OnAdd(layer);
        layer.AddBehaviour(bh);

        layer.Settings = new Generator3D.Settings()
        {
            scale = new Vector2Int(10, 10),
            resize = new Vector2(0, 0),
            position = new Vector3(0, 0, 0),
            name = "Exteriror",
        };

        layer.AddAssistant(new AssitantWFC());
        layer.AddGeneratorRule(new ExteriorRuleGenerator());

        // Modules
        //var x = new ExteriorModule();
        //layer.AddModule(x);

        // Transformers
        //
        //

        // Mode 1
        /*
        Texture2D icon = Resources.Load<Texture2D>("Icons/Select");
        var tool1 = new LBSTool(icon, "Select", typeof(Select), null, true);


        icon = Resources.Load<Texture2D>("Icons/RoomSelection");
        var tool2 = new LBSTool(
            icon,
            "Add empty tile",
            typeof(AddSchemaTile),
            null,
            false);

        icon = Resources.Load<Texture2D>("Icons/RoomSelection");
        var tool3 = new LBSTool(
            icon,
            "Remove tile",
            typeof(RemoveTileExterior),
            null,
            false);

        icon = Resources.Load<Texture2D>("Icons/AddConnection");
        var tool4 = new LBSTool(
            icon,
            "Set connection",
            typeof(AddConnection),
            typeof(TagsPalleteInspector), //typeof(RoomsPalleteInspector<TiledArea, ConnectedTile>),
            false);

        icon = Resources.Load<Texture2D>("Icons/erased");
        var tool5 = new LBSTool(
            icon,
            "Remove connection",
            typeof(RemoveAreaConnection), 
            null, 
            false);

        icon = Resources.Load<Texture2D>("Icons/Collapse_Icon");
        var tool6 = new LBSTool(
            icon, 
            "Collapse connection area", 
            typeof(WaveFunctionCollapseManipulator), 
            null, 
            false);
        */

        Debug.Log("Set Exterior Default");
        AssetDatabase.SaveAssets();
    }

    /// <summary>
    /// This function adjust the icons, layout and labels of the Population system
    /// also call the manipulators to make functional buttons in the layout
    /// </summary>
    /// <param name="template"></param>
    private void PopulationConstruct(LayerTemplate template)
    {
        template.Clear();

        // Basic data layer
        var layer = template.layer;
        layer.ID = "Population";
        layer.Name = "Layer Population";
        layer.iconPath = "Assets/ISI Lab/LBS/Plugin/Assets2D/Resources/Icons/ghost.png";
        template.layer = layer;

        //layer.TileSize = new Vector2Int(2, 2);
        layer.Settings = new Generator3D.Settings()
        {
            scale = new Vector2Int(2, 2),
            resize = new Vector2(0, 0),
            position = new Vector3(0, 0, 0),
            name = "Population",
        };

        layer.ID = "Population";
        layer.Name = "Layer Population";
        layer.iconPath = "Assets/ISI Lab/LBS/Plugin/Assets2D/Resources/Icons/ghost.png";
        template.layer = layer;

        var Icon = Resources.Load<Texture2D>("Icons/Select");
        layer.AddBehaviour(new PopulationBehaviour(Icon, "Population Behavior"));

        layer.AddAssistant(new AssistantMapElite());
        layer.AddGeneratorRule(new PopulationRuleGenerator());

        /*
        // Select
        Texture2D icon = Resources.Load<Texture2D>("Icons/Select");
        var tool1 = new LBSTool(icon, "Select", typeof(Select), null, true);

        //Add
        icon = Resources.Load<Texture2D>("Icons/paintbrush");
        var tool2 = new LBSTool(icon, "Add Tile", typeof(AddTaggedTile), typeof(BundlePalleteInspector), false);

        //Remove
        icon = Resources.Load<Texture2D>("Icons/Trash");
        var tool3 = new LBSTool(icon, "Remove", typeof(RemoveTile), null, false);
        */
        //template.modes.Add(mode1);

        Debug.Log("Set Population Default");
        AssetDatabase.SaveAssets();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="template"></param>
    private void Questconstuct(LayerTemplate template)
    {
        template.Clear();

        // Basic data layer
        var layer = template.layer;
        layer.ID = "Quest";
        layer.Name = "Layer Quest";
        layer.iconPath = "Assets/ISI Lab/LBS/Plugin/Assets2D/Resources/Icons/Quest.png";
        template.layer = layer;

        layer.Settings = new Generator3D.Settings()
        {
            scale = new Vector2Int(2, 2),
            resize = new Vector2(0, 0),
            position = new Vector3(0, 0, 0),
            name = "Quest",
        };

        var bhIcon = Resources.Load<Texture2D>("Icons/Select");
        var bh = new QuestBehaviour(bhIcon, "Quest behaviour");
        bh.OnAdd(layer);
        layer.AddBehaviour(bh);

        //layer.AddAssistant(new AssistentGrammar());

        // Modules
        //layer.AddModule(new LBSGraph());
        //layer.AddModule(new LBSGrammarGraph()); // (!)

        // Mode 1
        /*
        Texture2D icon = Resources.Load<Texture2D>("Icons/Select");
        var tool1 = new LBSTool(icon, "Select", typeof(Select), null, true);
        icon = Resources.Load<Texture2D>("Icons/Addnode");
        var tool2 = new LBSTool(icon, "Add node", typeof(CreateNewGrammarNode), typeof(GrammarPallete), false); // (!)
        icon = Resources.Load<Texture2D>("Icons/AddConnection");
        var tool3 = new LBSTool(icon, "Add conection", typeof(CreateNewConnection<LBSNode>), null, false); // (!)
        icon = Resources.Load<Texture2D>("Icons/Trash");
        var tool4 = new LBSTool(icon, "Remove", typeof(RemoveGraphNode<LBSNode>), null, false); // (!)
        */
        //template.modes.Add(mode1);

        Debug.Log("Set Quest Default");
        AssetDatabase.SaveAssets();
    }
}
