using LBS.Components.Specifics;
using LBS.Components.TileMap;
using LBS.Settings;
using LBS.VisualElements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;



[LBSCustomEditor("SchemaBehaviour", typeof(SchemaBehaviour))]
public class SchemaBehaviourEditor : LBSCustomEditor, IToolProvider
{
    private readonly Color BHcolor = LBSSettings.Instance.view.behavioursColor;
    
    private SchemaBehaviour schema;

    // Manipulators
    private AddSchemaTile createNewRoomNode;
    private RemoveSchemaTile removeSchemaTile;

    private SetTileConnection setTileConnection;
    private RemoveTileConnection removeTileConnection;

    // View
    private SimplePallete areaPallete;
    private SimplePallete connectionPallete;

    public SchemaBehaviourEditor(object target) : base(target)
    {
        this.schema = target as SchemaBehaviour;

        CreateVisualElement();
    }

    public void SetTools(ToolKit toolKit)
    {
        Texture2D icon;
        //  var tool1 = new LBSTool(icon, "Select", typeof(Select), null, true);

        // Add Zone Tiles
        icon = Resources.Load<Texture2D>("Icons/Addnode");
        this.createNewRoomNode = new AddSchemaTile();
        var t1 = new LBSTool(icon, "Paint Zone", createNewRoomNode);
        t1.Init(schema.Owner, schema);
        toolKit.AddTool(t1);

        // Remove Tiles
        icon = Resources.Load<Texture2D>("Icons/Trash");
        this.removeSchemaTile = new RemoveSchemaTile();
        var t2 = new LBSTool(icon, "Remove Tile", removeSchemaTile);
        t2.Init(schema.Owner, schema);
        toolKit.AddTool(t2);

        toolKit.AddSeparator(10);

        // Add Tile connection
        icon = Resources.Load<Texture2D>("Icons/open-exit-door");
        this.setTileConnection = new SetTileConnection();
        var t3 = new LBSTool(icon, "Set connection", setTileConnection);
        t3.Init(schema.Owner, schema);
        toolKit.AddTool(t3);

        // Remove Tile connection
        icon = Resources.Load<Texture2D>("Icons/open-exit-door");
        this.removeTileConnection = new RemoveTileConnection();
        var t4 = new LBSTool(icon,"Clean connection", removeTileConnection);
        t4.Init(schema.Owner, schema);
        toolKit.AddTool(t4);
    }

    public override void ContextMenu(ContextualMenuPopulateEvent evt)
    {
        evt.menu.AppendAction("Opción 1", action => Debug.Log("Seleccionaste la opción 1"));
        evt.menu.AppendAction("Opción 2", action => Debug.Log("Seleccionaste la opción 2"));
        evt.menu.AppendSeparator();
        evt.menu.AppendAction("Opción 3", action => Debug.Log("Seleccionaste la opción 3"));
    }


    public override void SetInfo(object target)
    {
        throw new System.NotImplementedException();
    }

    protected override VisualElement CreateVisualElement()
    {
        var visualTree = DirectoryTools.SearchAssetByName<VisualTreeAsset>("SchemaBehaviourEditor");
        visualTree.CloneTree(this);

        // Area Pallete
        this.areaPallete = this.Q<SimplePallete>("ZonePallete");
        SetAreaPallete();

        // Connection Pallete
        this.connectionPallete = this.Q<SimplePallete>("ConnectionPallete");
        SetConnectionPallete();

        return this;
    }

    private void SetAreaPallete()
    {
        areaPallete.ShowGroups = false;
        areaPallete.SetName("Zones");
        var icon = Resources.Load<Texture2D>("Icons/BrushIcon");
        areaPallete.SetIcon(icon, BHcolor);

        var zones = schema.Zones;
        var options = new object[zones.Count];
        for (int i = 0; i < zones.Count; i++)
        {
            options[i] = zones[i];
        }

        // Select option event
        areaPallete.OnSelectOption += (selected) => {
            // var tk = ToolKit.Instance;
            createNewRoomNode.ToSet = selected as Zone;
        };

        // OnAdd option event
        areaPallete.OnAddOption += () =>
        {
            schema.AddZone();
            areaPallete.Options = new object[schema.Zones.Count];
            for (int i = 0; i < zones.Count; i++)
            {
                areaPallete.Options[i] = schema.Zones[i];
            }
            areaPallete.Repaint();
        };

        // Init options
        areaPallete.SetOptions(options, (optionView, option) =>
        {
            var area = (Zone)option;
            optionView.Label = area.ID; // ID or name (??)
            optionView.Color = area.Color;
        });

        areaPallete.Repaint();
    }

    private void SetConnectionPallete()
    {
        connectionPallete.ShowGroups = false;
        connectionPallete.SetName("Connections");
        var icon = Resources.Load<Texture2D>("Icons/BrushIcon");
        connectionPallete.SetIcon(icon, BHcolor);

        var connections = schema.Connections;
        var options = new object[connections.Count];
        for (int i = 0; i < connections.Count; i++)
        {
            options[i] = connections[i];
        }

        // Select option event
        connectionPallete.OnSelectOption += (selected) => {
            // var tk = ToolKit.Instance;
            setTileConnection.ToSet = selected as string;
        };

        // OnAdd option event
        connectionPallete.OnAddOption += () =>
        {
            Debug.LogWarning("Por ahora esta herramienta no permite agregar nuevos tipos de conexiones");
        };

        // Init options
        connectionPallete.SetOptions(options, (optionView, option) =>
        {
            var conencts = (string)option;
            optionView.Label = conencts; 
            optionView.Color = Color.black;
        });

        connectionPallete.Repaint();

    }
}