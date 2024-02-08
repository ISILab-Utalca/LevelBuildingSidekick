using ISILab.Commons.Utility.Editor;
using ISILab.LBS.VisualElements;
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

[LBSCustomEditor("Schema Behaviour", typeof(SchemaBehaviour))]
public class SchemaBehaviourEditor : LBSCustomEditor, IToolProvider
{

    #region FIELDS
    private SchemaBehaviour _target;

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
        this._target = target as SchemaBehaviour;

        CreateVisualElement();
    }

    public void SetTools(ToolKit toolKit)
    {
        Texture2D icon;

        // Add Zone Tiles
        icon = Resources.Load<Texture2D>("Icons/Tools/Brush_interior_tile");
        this.createNewRoomNode = new AddSchemaTile();
        var t1 = new LBSTool(icon, "Paint Zone", createNewRoomNode);
        t1.OnSelect += () => LBSInspectorPanel.ShowInspector("Behaviours");
        t1.OnEnd += (l)=> areaPallete.Repaint();
        t1.Init(_target.Owner, _target);
        toolKit.AddTool(t1);

        // Remove Tiles
        icon = Resources.Load<Texture2D>("Icons/Tools/Delete_interior_tile");
        this.removeSchemaTile = new RemoveSchemaTile();
        var t2 = new LBSTool(icon, "Remove Tile", removeSchemaTile);
        t2.Init(_target.Owner, _target);
        toolKit.AddTool(t2);

        toolKit.AddSeparator(10);

        // Add Tile connection
        icon = Resources.Load<Texture2D>("Icons/Tools/Set_Connection");
        this.setTileConnection = new SetSchemaTileConnection();
        var t3 = new LBSTool(icon, "Set connection", setTileConnection);
        t3.OnSelect += () => LBSInspectorPanel.ShowInspector("Behaviours");
        t3.Init(_target.Owner, _target);
        toolKit.AddTool(t3);

        // Remove Tile connection
        icon = Resources.Load<Texture2D>("Icons/Tools/Delete_Set_Connection");
        this.removeTileConnection = new RemoveTileConnection();
        var t4 = new LBSTool(icon,"Clean connection", removeTileConnection);
        t4.Init(_target.Owner, _target);
        toolKit.AddTool(t4);
    }

    public override void ContextMenu(ContextualMenuPopulateEvent evt)
    {

    }

    public override void SetInfo(object target)
    {
        this._target = target as SchemaBehaviour;
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
        insideField.value = _target.PressetInsideStyle;
        insideField.RegisterValueChangedCallback(evt =>
        {
            _target.PressetInsideStyle = evt.newValue as Bundle;
        });

        // Outside Field
        var outsideField = this.Q<ObjectField>("OutsideField");
        outsideField.value = _target.PressetOutsideStyle;
        outsideField.RegisterValueChangedCallback(evt =>
        {
            _target.PressetOutsideStyle = evt.newValue as Bundle;
        });

        return this;
    }

    private void SetAreaPallete()
    {
        areaPallete.ShowGroups = false;
        areaPallete.SetName("Zones");
        var icon = Resources.Load<Texture2D>("Icons/BrushIcon");
        areaPallete.SetIcon(icon, BHcolor);

        var zones = _target.Zones;
        var options = new object[zones.Count];
        for (int i = 0; i < zones.Count; i++)
        {
            options[i] = zones[i];
        }

        // Select option event
        areaPallete.OnSelectOption += (selected) => {
            _target.roomToSet = selected as Zone;
            ToolKit.Instance.SetActive("Paint Zone");
        };

        // OnAdd option event
        areaPallete.OnAddOption += () =>
        {
            var newZone = _target.AddZone();
            newZone.InsideStyles = new List<string>() { _target.PressetInsideStyle.Name };
            newZone.OutsideStyles = new List<string>() { _target.PressetOutsideStyle.Name };
            areaPallete.Options = new object[_target.Zones.Count];
            for (int i = 0; i < zones.Count; i++)
            {
                areaPallete.Options[i] = _target.Zones[i];
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
            var zones = _target.Zones;
            areaPallete.Options = new object[zones.Count];
            for (int i = 0; i < zones.Count; i++)
            {
                areaPallete.Options[i] = _target.Zones[i];
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

            _target.RemoveZone(option as Zone);

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

        var connections = _target.Connections;
        var options = new object[connections.Count];
        for (int i = 0; i < connections.Count; i++)
        {
            options[i] = connections[i];
        }

        // Select option event
        connectionPallete.OnSelectOption += (selected) => {
            // var tk = ToolKit.Instance;
            _target.conectionToSet = selected as string;
            //setTileConnection.ToSet = selected as string;
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