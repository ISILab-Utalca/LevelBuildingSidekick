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
        assistantOptions = typeof(LBSAssistant).GetDerivedTypes().ToList();
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
            template.layer.AddAssistant(ass as LBSAssistant);
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
        layer.AddBehaviour(bh);

        // Assistants
        var assIcon = Resources.Load<Texture2D>("Icons/Select");
        var ass = new HillClimbingAssistant(assIcon,"HillClimbing");
        layer.AddAssistant(ass);
        
        // Generators
        layer.AddGeneratorRule(new SchemaRuleGenerator());
        layer.AddGeneratorRule(new SchemaRuleGeneratorExteriror());

        // Seting generator
        layer.Settings = new Generator3D.Settings()
        {
            scale = new Vector2Int(2, 2),
            resize = new Vector2(0, 0),
            position = new Vector3(0, 0, 0),
            name = "Interior",
        };

        // Save scriptableObject
        Debug.Log("Set Interior Default");
        EditorUtility.SetDirty(target);
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

        // Modules
        // XXX

        // Behaviours
        var bhIcon = Resources.Load<Texture2D>("Icons/Select");
        var bh = new ExteriorBehaviour(bhIcon, "Exteriror behaviour");
        bh.OnAttachLayer(layer);
        layer.AddBehaviour(bh);

        // Assistant
        var assIcon = Resources.Load<Texture2D>("Icons/Select");
        var ass = new AssistantWFC(assIcon, "Assistant WFC");
        ass.OnAttachLayer(layer);
        layer.AddAssistant(ass);

        // Generators
        layer.AddGeneratorRule(new ExteriorRuleGenerator());

        // Settings generator
        layer.Settings = new Generator3D.Settings()
        {
            scale = new Vector2Int(2, 2),
            resize = new Vector2(0, 0),
            position = new Vector3(0, 0, 0),
            name = "Exteriror",
        };

        // Save scriptableObject
        Debug.Log("Set Exterior Default");
        EditorUtility.SetDirty(target);
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

        // Behaviours
        var Icon = Resources.Load<Texture2D>("Icons/Select");
        layer.AddBehaviour(new PopulationBehaviour(Icon, "Population Behavior"));

        // Assistants
        var assIcon = Resources.Load<Texture2D>("Icons/Select");
        var ass = new AssistantMapElite(assIcon, "A");
        //ass.OnAdd(layer);
        layer.AddAssistant(ass);

        // Rules
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
        EditorUtility.SetDirty(target);
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
        EditorUtility.SetDirty(target);
        AssetDatabase.SaveAssets();
    }
}
