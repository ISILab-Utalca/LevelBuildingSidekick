using ISILab.Commons.Utility.Editor;
using ISILab.LBS;
using ISILab.LBS.Assistants;
using ISILab.LBS.Editor;
using ISILab.LBS.Manipulators;
using ISILab.LBS.Modules;
using ISILab.LBS.VisualElements;
using LBS;
using LBS.Components;
using LBS.Settings;
using LBS.VisualElements;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;
using Debug = UnityEngine.Debug;
using Toggle = UnityEngine.UIElements.Toggle;

[LBSCustomEditor("BlueprintAssistant", typeof(BlueprintAssistant))]
public class BlueprintAssitantEditor : LBSCustomEditor, IToolProvider
{
    public BlueprintAssistant blueprintAssistant;

    // Basic options
    private Toggle displayTimingsToggle;
    private Toggle showConstraintToggle;
    private Button executeButton;
    private Button recalculateConstraintsButton;

    // Data
    private Foldout constrainsFoldout;

    // Algorithm options
    private DropdownField algorithmDropdown;

    // Manipulators
    private SetZoneConnection setZoneConnection;
    private RemoveAreaConnection removeAreaConnection;

    public BlueprintAssitantEditor(object target) : base(target)
    {
        blueprintAssistant = target as BlueprintAssistant;

        CreateVisualElement();

        var wnd = EditorWindow.GetWindow<LBSMainWindow>();

        blueprintAssistant.OnTermination += wnd.Repaint;
    }

    public override void Repaint()
    {
        var moduleConstr = blueprintAssistant.Owner.GetModule<ConstrainsZonesModule>();
        constrainsFoldout.Clear();
        foreach (var constraint in moduleConstr.Constraints)
        {
            var view = new ConstraintView();
            view.SetData(constraint);
            constrainsFoldout.Add(view);
        }
    }

    public override void SetInfo(object target)
    {
        blueprintAssistant = target as BlueprintAssistant;
    }

    public void SetTools(ToolKit toolKit)
    {
        Texture2D icon;

        toolKit.AddSeparator();

        // Add Zone connection
        icon = Resources.Load<Texture2D>("Icons/Tools/Node_connection");
        setZoneConnection = new SetZoneConnection();
        var t1 = new LBSTool(icon, "Add zone connection", setZoneConnection);
        t1.OnSelect += () => LBSInspectorPanel.ShowInspector("Assistants");
        t1.Init(blueprintAssistant.Owner, blueprintAssistant);
        toolKit.AddTool(t1);

        // Remove zone connections
        icon = Resources.Load<Texture2D>("Icons/Tools/Delete_node_connection");
        removeAreaConnection = new RemoveAreaConnection();
        var t2 = new LBSTool(icon, "Remove zone connection", removeAreaConnection);
        t2.Init(blueprintAssistant.Owner, blueprintAssistant);
        toolKit.AddTool(t2);
    }

    protected override VisualElement CreateVisualElement()
    {
        var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("BlueprintEditor");
        visualTree.CloneTree(this);

        var moduleConstr = blueprintAssistant.Owner.GetModule<ConstrainsZonesModule>();

        // Foldout
        constrainsFoldout = this.Q<Foldout>();
        foreach (var constraint in moduleConstr.Constraints)
        {
            var view = new ConstraintView();
            view.SetData(constraint);
            constrainsFoldout.Add(view);
        }

        // Print Timers
        displayTimingsToggle = this.Q<Toggle>("ShowTimerToggle");
        displayTimingsToggle.RegisterCallback<ChangeEvent<bool>>(x =>
        {
            blueprintAssistant.printClocks = x.newValue;
        });

        // Execute
        executeButton = this.Q<Button>("Execute");
        executeButton.clicked += Execute;

        // Show Constraint
        showConstraintToggle = this.Q<Toggle>("ShowConstraintToggle");
        showConstraintToggle.value = blueprintAssistant.visibleConstraints;
        showConstraintToggle.RegisterCallback<ChangeEvent<bool>>(x =>
        {
            blueprintAssistant.visibleConstraints = x.newValue;
            DrawManager.ReDraw();
            Debug.Log("Show contrains: " + x.newValue);
        });

        // recalculate constraints
        recalculateConstraintsButton = new Button();
        recalculateConstraintsButton.text = "Recalculate Constraints";
        recalculateConstraintsButton.clicked += () =>
        {
            // Save history version to revert if necessary
            var x = LBSController.CurrentLevel;
            Undo.RegisterCompleteObjectUndo(x, "Recalculate Constraints");
            EditorGUI.BeginChangeCheck();

            // Recalculate constraints
            blueprintAssistant.RecalculateConstraint();

            // Mark as dirty
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(x);
            }

            DrawManager.ReDraw();
            Paint();
        };
        Add(recalculateConstraintsButton);

        return this;
    }

    private void Paint()
    {
        Clear();
        CreateVisualElement();
    }

    private void Execute()
    {
        // Save history version to revert if necessary
        var x = LBSController.CurrentLevel;
        Undo.RegisterCompleteObjectUndo(x, "Execute HillClimbing 2");
        EditorGUI.BeginChangeCheck();

        // Execute hill climbing
        blueprintAssistant.Execute();

        // Mark as dirty
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(x);
        }
    }
}
