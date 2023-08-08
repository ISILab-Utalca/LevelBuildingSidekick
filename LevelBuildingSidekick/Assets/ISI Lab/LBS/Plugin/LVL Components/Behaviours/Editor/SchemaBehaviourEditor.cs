using LBS.Components.Specifics;
using LBS.Components.TileMap;
using LBS.VisualElements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;

public interface IToolProvider
{
    public void SetTools(ToolKit group);
}

[LBSCustomEditor("SchemaBehaviour", typeof(SchemaBehaviour))]
public class SchemaBehaviourEditor : LBSCustomEditor, IToolProvider
{
    private SchemaBehaviour schema;

    private List<LBSTool> tools = new List<LBSTool>();

    // Manipulators
    private CreateNewRoomNode createNewRoomNode;
    private AddTileToTiledArea<TiledArea, ConnectedTile> addTileToTiledArea;

    // View
    private SimplePallete areaPallete;

    /*
    public SchemaBehaviourEditor()
    {
        //manipulators.Add(new AddConnection());
        //manipulators.Add(new RemoveTile());
    }
    */

    public SchemaBehaviourEditor(object target) : base(target)
    {
        this.schema = target as SchemaBehaviour;

        /*
        this.addSchemaTile = new AddSchemaTile();
        manipulators.Add(addSchemaTile);
        manipulators.Add(new RemoveSchemaTile());
        manipulators.Add(new AddAreaConnection());
        manipulators.Add(new RemoveAreaConnection());
        manipulators.Add(new SetTileConnection());
        */

        Init();

        CreateVisualElement();
    }

    public void SetTools(ToolKit toolKit)
    {
        Texture2D icon;
        //  var tool1 = new LBSTool(icon, "Select", typeof(Select), null, true);

        icon = Resources.Load<Texture2D>("Icons/Addnode");
        this.createNewRoomNode = new CreateNewRoomNode();
        var t1 = new LBSTool(icon, "Add Node", createNewRoomNode);
        toolKit.AddTool(t1);

        icon = Resources.Load<Texture2D>("Icons/AddConnection");
        var t2 = new LBSTool(icon, "Add connection", new CreateNewConnection<RoomNode>());
        toolKit.AddTool(t2);

        icon = Resources.Load<Texture2D>("Icons/Trash");
        var t3 = new LBSTool(icon, "Remove Node", new RemoveGraphNode<RoomNode>());
        toolKit.AddTool(t3);

        icon = Resources.Load<Texture2D>("Icons/Trash");
        var t4 = new LBSTool(icon, "Remove Connection", new RemoveGraphConnection());
        toolKit.AddTool(t4);

        toolKit.AddSeparator(10);

        icon = Resources.Load<Texture2D>("Icons/paintbrush");
        this.addTileToTiledArea = new AddTileToTiledArea<TiledArea, ConnectedTile>();
        var tool6 = new LBSTool(icon, "Paint tile", addTileToTiledArea);
        toolKit.AddTool(tool6);

        /*
        icon = Resources.Load<Texture2D>("Icons/erased");
        var tool7 = new LBSTool(icon,"Erase",
            new RemoveTile<TiledArea, ConnectedTile>(), // Removed<TiledArea<LBSTile>, LBSTile>,
            null
        );

        icon = Resources.Load<Texture2D>("Icons/open-exit-door");
        var tool8 = new LBSTool(icon, "Add door", new AddDoor<TiledArea, ConnectedTile>());

        icon = Resources.Load<Texture2D>("Icons/erased");
        var tool9 = new LBSTool(
            icon,
            "Remove door",
            typeof(RemoveDoor<TiledArea, ConnectedTile>), //typeof(RemoveDoor<TiledArea,ConnectedTile>),
            null,
            true);
        */
    }

    public override void ContextMenu(ContextualMenuPopulateEvent evt)
    {
        evt.menu.AppendAction("Opción 1", action => Debug.Log("Seleccionaste la opción 1"));
        evt.menu.AppendAction("Opción 2", action => Debug.Log("Seleccionaste la opción 2"));
        evt.menu.AppendSeparator();
        evt.menu.AppendAction("Opción 3", action => Debug.Log("Seleccionaste la opción 3"));
    }

    public void Init()
    {
        foreach(var m in tools)
        {
            //m.Init( ,schema.Owner, schema);
        }
    }

    public override void SetInfo(object target)
    {

    }

    protected override VisualElement CreateVisualElement()
    {
        var visualTree = DirectoryTools.SearchAssetByName<VisualTreeAsset>("SchemaBehaviourEditor");
        visualTree.CloneTree(this);

        // Area Pallete
        this.areaPallete = this.Q<SimplePallete>();
        areaPallete.ShowGroups = false;
        areaPallete.SetName("Rooms");
        var icon = Resources.Load<Texture2D>("Icons/BrushIcon");
        areaPallete.SetIcon(icon, new Color(135f / 255f, 215f / 255f, 246f / 255f));
        SetAreaPallete();

        return this;
    }

    private void SetAreaPallete()
    {
        var areas = schema.Areas;
        var options = new object[areas.Count];

        for (int i = 0; i < areas.Count; i++)
        {
            options[i] = areas;
        }

        areaPallete.SetOptions(options, (optionView, option) => 
        {
            var area = (Zone)option;
            optionView.Label = area.ID; // ID or name (??)
            optionView.Color = area.Color;
        });

        areaPallete.OnSelectOption += (selected) => {
            var tk = ToolKit.Instance;

        };

        areaPallete.OnAddOption += () =>
        {
            schema.AddZone(new Vector2Int(0, 0));
            areaPallete.Repaint();
        };

        areaPallete.Repaint();
    }

}