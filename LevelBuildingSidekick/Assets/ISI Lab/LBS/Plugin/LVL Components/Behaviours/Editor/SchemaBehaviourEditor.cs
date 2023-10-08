using LBS;
using LBS.Bundles;
using LBS.Components.Specifics;
using LBS.Components.TileMap;
using LBS.Settings;
using LBS.VisualElements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;

[LBSCustomEditor("Schema Behaviour", typeof(SchemaBehaviour))]
public class SchemaBehaviourEditor : LBSCustomEditor, IToolProvider
{
    #region FIELDS
    private SchemaBehaviour schema;

    private AddSchemaTile createNewRoomNode;
    private RemoveSchemaTile removeSchemaTile;

    private SetSchemaTileConnection setTileConnection;
    private RemoveTileConnection removeTileConnection;
    #endregion

    #region VIEW FIELDS
    private SimplePallete areaPallete;
    private SimplePallete connectionPallete;
    #endregion

    #region PROPERTIES
    private Color BHcolor => LBSSettings.Instance.view.behavioursColor;
    #endregion

    public SchemaBehaviourEditor(object target) : base(target)
    {
        this.schema = target as SchemaBehaviour;

        CreateVisualElement();
    }

    public void SetTools(ToolKit toolKit)
    {
        Texture2D icon;

        // Move Tile
        // [Implementar]

        // Add Zone Tiles
        icon = Resources.Load<Texture2D>("Icons/Tools/Brush_interior_tile");
        this.createNewRoomNode = new AddSchemaTile();
        var t1 = new LBSTool(icon, "Paint Zone", createNewRoomNode);
        t1.OnSelect += () => LBSInspectorPanel.ShowInspector("Local","Behaviours");
        t1.OnEnd += ()=> areaPallete.Repaint();
        t1.Init(schema.Owner, schema);
        toolKit.AddTool(t1);

        // Remove Tiles
        icon = Resources.Load<Texture2D>("Icons/Tools/Delete_interior_tile");
        this.removeSchemaTile = new RemoveSchemaTile();
        var t2 = new LBSTool(icon, "Remove Tile", removeSchemaTile);
        t2.Init(schema.Owner, schema);
        toolKit.AddTool(t2);

        toolKit.AddSeparator(10);

        // Add Tile connection
        icon = Resources.Load<Texture2D>("Icons/Tools/Set_Connection");
        this.setTileConnection = new SetSchemaTileConnection();
        var t3 = new LBSTool(icon, "Set connection", setTileConnection);
        t3.OnSelect += () => LBSInspectorPanel.ShowInspector("Local", "Behaviours");
        t3.Init(schema.Owner, schema);
        toolKit.AddTool(t3);

        // Remove Tile connection
        icon = Resources.Load<Texture2D>("Icons/Tools/Delete_Set_Connection");
        this.removeTileConnection = new RemoveTileConnection();
        var t4 = new LBSTool(icon,"Clean connection", removeTileConnection);
        t4.Init(schema.Owner, schema);
        toolKit.AddTool(t4);
    }

    public override void ContextMenu(ContextualMenuPopulateEvent evt)
    {
        //evt.menu.AppendSeparator();
        //evt.menu.AppendAction("Show view", action => schema.visible = true);
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

        // Inside Field
        var insideField = this.Q<ObjectField>("InsideField");
        insideField.value = schema.PressetInsideStyle;
        insideField.RegisterValueChangedCallback(evt =>
        {
            schema.PressetOutsideStyle = evt.newValue as Bundle;
        });

        // Outside Field
        var outsideField = this.Q<ObjectField>("OutsideField");
        outsideField.value = schema.PressetOutsideStyle;
        outsideField.RegisterValueChangedCallback(evt =>
        {
            schema.PressetOutsideStyle = evt.newValue as Bundle;
        });

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
            createNewRoomNode.ToSet = selected as Zone;
            ToolKit.Instance.SetActive("Paint Zone");
        };

        // OnAdd option event
        areaPallete.OnAddOption += () =>
        {
            var newZone = schema.AddZone();
            newZone.InsideStyles = new List<string>() { schema.PressetInsideStyle.Name };
            newZone.OutsideStyles = new List<string>() { schema.PressetOutsideStyle.Name };
            areaPallete.Options = new object[schema.Zones.Count];
            for (int i = 0; i < zones.Count; i++)
            {
                areaPallete.Options[i] = schema.Zones[i];
            }
            ToolKit.Instance.SetActive("Paint Zone");
            areaPallete.Repaint();
        };

        // Init options
        areaPallete.SetOptions(options, (optionView, option) =>
        {
            var area = (Zone)option;
            optionView.Label = area.ID; // ID or name (??)
            optionView.Color = area.Color;
        });

        areaPallete.OnRepaint += () =>
        {
            var zones = schema.Zones;
            areaPallete.Options = new object[zones.Count];
            for (int i = 0; i < zones.Count; i++)
            {
                areaPallete.Options[i] = schema.Zones[i];
            }
        };

        areaPallete.OnRemoveOption += (option) =>
        {
            if (option == null)
                return;

            var answer = EditorUtility.DisplayDialog("Caution",
                "You are about to delete a zone, which may be related" +
                " to tiles on your map. If you delete the zone," +
                " the corresponding tiles will also be removed." +
                " Are you sure you want to proceed?", "Continue", "Cancel");

            if (!answer)
                return;

            schema.RemoveZone(option as Zone);

            DrawManager.ReDraw();
            areaPallete.Repaint();
        };

        areaPallete.Repaint();
    }

    private void SetConnectionPallete()
    {
        connectionPallete.ShowGroups = false;
        connectionPallete.ShowRemoveButton = false;
        connectionPallete.ShowAddButton = false;

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
            ToolKit.Instance.SetActive("Set connection");
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