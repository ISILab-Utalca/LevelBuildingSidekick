using LBS;
using LBS.Components;
using LBS.Settings;
using LBS.VisualElements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor;
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
    private Toggle toggle;

    private Button recalculate;

    private LBSLayer tempLayer;

    // Manipulators
    private SetZoneConnection setZoneConnection;
    private RemoveAreaConnection removeAreaConnection;

    public HillClimbingAssistantEditor(object target) : base(target)
    {
        this.hillClimbing = target as HillClimbingAssistant;

        CreateVisualElement();

        var wnd = LBSMainWindow.GetWindow<LBSMainWindow>();

        hillClimbing.OnTermination += wnd.Repaint;
    }

    public override void Repaint()
    {
        var moduleConstr = hillClimbing.Owner.GetModule<ConstrainsZonesModule>();
        foldout.Clear();
        foreach (var constraint in moduleConstr.Constraints)
        {
            var view = new ConstraintView();
            view.SetData(constraint);
            foldout.Add(view);
        }
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
        icon = Resources.Load<Texture2D>("Icons/Tools/Node_connection");
        this.setZoneConnection = new SetZoneConnection();
        var t1 = new LBSTool(icon, "Add zone connection", setZoneConnection);
        t1.OnSelect += () => LBSInspectorPanel.ShowInspector("Local", "Assistants");
        t1.Init(hillClimbing.Owner, hillClimbing);
        toolKit.AddTool(t1);

        // Remove zone connections
        icon = Resources.Load<Texture2D>("Icons/Tools/Delete_node_connection");
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
        this.revert.clicked += Revert;

        // Execute
        this.execute = this.Q<Button>("Execute");
        this.execute.clicked += Execute;

        // Show Constraint
        this.toggle = this.Q<Toggle>("ShowConstraintToggle");
        this.toggle.value = hillClimbing.visibleConstraints;
        toggle.RegisterCallback<ChangeEvent<bool>>( x =>
        {
            hillClimbing.visibleConstraints = x.newValue;
            DrawManager.ReDraw();
        });

        this.recalculate = new Button();
        this.recalculate.text = "Recalculate Constraints";
        this.recalculate.clicked += () =>
        {
            hillClimbing.RecalculateConstraint();
            DrawManager.ReDraw();
            Paint();
        };

        this.Add(recalculate);

        return this;
    }

    private void Paint()
    {
        this.Clear();
        CreateVisualElement();
    }


    private void Execute()
    {
        //tempLayer = hillClimbing.Owner.Clone() as LBSLayer; // (!!)
        hillClimbing.Execute();
    }

    private void Revert()
    {
        if (tempLayer == null)
        {
            Debug.Log("No existe version para revertir.");
            return;
        }

        var lvl = hillClimbing.Owner.Parent;
        lvl.ReplaceLayer(hillClimbing.Owner, tempLayer);
    }
}


