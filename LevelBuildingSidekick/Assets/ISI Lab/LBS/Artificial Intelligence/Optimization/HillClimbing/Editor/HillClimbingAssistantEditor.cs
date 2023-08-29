using LBS.Components;
using LBS.Settings;
using LBS.VisualElements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;

[LBSCustomEditor("HillClimbingAssistant", typeof(HillClimbingAssistant))]
public class HillClimbingAssistantEditor : LBSCustomEditor, IToolProvider
{
    private readonly UnityEngine.Color AssColor = LBSSettings.Instance.view.assitantsColor;

    private HillClimbingAssistant hillClimbing;

    private Foldout foldout;
    private Button revert;
    private Button execute;

    private LBSLayer tempLayer;

    // Manipulators
    private SetZoneConnection setZoneConnection;
    private RemoveAreaConnection removeAreaConnection;

    public HillClimbingAssistantEditor(object target) : base(target)
    {
        this.hillClimbing = target as HillClimbingAssistant;

        CreateVisualElement();
    }

    public override void SetInfo(object target)
    {
        throw new System.NotImplementedException();
    }

    public void SetTools(ToolKit toolKit)
    {
        Texture2D icon;

        toolKit.AddSeparator();

        // Add Zone connection
        icon = Resources.Load<Texture2D>("Icons/Addnode");
        this.setZoneConnection = new SetZoneConnection();
        var t1 = new LBSTool(icon, "Add zone connection", setZoneConnection);
        t1.Init(hillClimbing.Owner, hillClimbing);
        toolKit.AddTool(t1);

        // Remove zone connections
        icon = Resources.Load<Texture2D>("Icons/Addnode");
        this.removeAreaConnection = new RemoveAreaConnection();
        var t2 = new LBSTool(icon,"Remove zone connection", removeAreaConnection);
        t2.Init(hillClimbing.Owner,hillClimbing);
        toolKit.AddTool(t2);
    }

    protected override VisualElement CreateVisualElement()
    {
        var visualTree = DirectoryTools.SearchAssetByName<VisualTreeAsset>("HillClimbingEditor");
        visualTree.CloneTree(this);

        var moduleConstr = hillClimbing.Owner.GetModule<ConstrainsZonesModule>();

        // Foldout
        this.foldout = this.Q<Foldout>();
        foreach (var constraint in moduleConstr.Constraints)
        {
            var view = new ConstraintView();
            view.SetData(constraint);
            foldout.Add(view);
        }

        // Revert
        this.revert = this.Q<Button>("Revert");
        this.revert.clicked += OnRevert;

        // Execute
        this.execute = this.Q<Button>("Execute");
        this.execute.clicked += OnExecute;


        return this;
    }

    private void OnExecute()
    {
        tempLayer = hillClimbing.Owner.Clone() as LBSLayer;
        hillClimbing.Execute();
    }

    private void OnRevert()
    {
        if (tempLayer == null)
        {
            Debug.Log("|no existe version para revertir.");
            return;
        }

        var lvl = hillClimbing.Owner.Parent;
        lvl.ReplaceLayer(hillClimbing.Owner, tempLayer);
    }
}


