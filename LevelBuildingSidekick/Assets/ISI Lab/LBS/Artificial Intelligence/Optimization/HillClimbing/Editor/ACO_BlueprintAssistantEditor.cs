using ISILab.LBS.Editor;
using ISILab.LBS.Manipulators;
using LBS;
using LBS.VisualElements;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS;
using ISILab.LBS.Assistants;
using ISILab.LBS.Modules;
using LBS.Components;
using LBS.Settings;
using System;
using System.Drawing;
using ISILab.LBS.VisualElements;
using System.CodeDom.Compiler;

[LBSCustomEditor("AcoAssistant", typeof(ACO_BlueprintAssistant))]
public class ACO_BlueprintAssistantEditor : LBSCustomEditor, IToolProvider
{
    public ACO_BlueprintAssistant ACO_Assitant;

    // Manipulators
    private SetZoneConnection setZoneConnection;
    private RemoveAreaConnection removeAreaConnection;

    private Foldout foldout;
    private Button revert;
    private Button execute;
    private Toggle toggle;
    private Toggle toggleTimer;
    private Button recalculate;

    private LBSLayer tempLayer;

    public ACO_BlueprintAssistantEditor(object target) : base(target)
    {
        ACO_Assitant = target as ACO_BlueprintAssistant;

        CreateVisualElement();

        var wnd = EditorWindow.GetWindow<LBSMainWindow>();

        ACO_Assitant.OnTermination += wnd.Repaint;
    }

    public override void SetInfo(object target)
    {
        ACO_Assitant = target as ACO_BlueprintAssistant;
    }

    public void SetTools(ToolKit toolkit)
    {
        Texture2D icon;

        toolkit.AddSeparator();

        // Add Zone connection
        icon = Resources.Load<Texture2D>("Icons/Tools/Node_connection");
        setZoneConnection = new SetZoneConnection();
        var t1 = new LBSTool(icon, "Add zone connection", setZoneConnection);
        t1.OnSelect += () => LBSInspectorPanel.ShowInspector("Assistants");
        t1.Init(ACO_Assitant.Owner, ACO_Assitant);
        toolkit.AddTool(t1);

        // Remove zone connections
        icon = Resources.Load<Texture2D>("Icons/Tools/Delete_node_connection");
        removeAreaConnection = new RemoveAreaConnection();
        var t2 = new LBSTool(icon, "Remove zone connection", removeAreaConnection);
        t2.Init(ACO_Assitant.Owner, ACO_Assitant);
        toolkit.AddTool(t2);
    }

    protected override VisualElement CreateVisualElement()
    {
        var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("HillClimbingEditor_2");
        visualTree.CloneTree(this);

        var moduleConstr = ACO_Assitant.Owner.GetModule<ConstrainsZonesModule>();

        // iteration slider
        var it_slider = this.Q<Slider>("iteration_slider");
        it_slider.value = ACO_Assitant.iterations;
        it_slider.RegisterValueChangedCallback(x =>
        {
            ACO_Assitant.iterations = (int)x.newValue;
        });

        // ants amount slider
        var ants_slider = this.Q<Slider>("ant_amount_slider");
        ants_slider.value = ACO_Assitant.antsPerIteration;
        ants_slider.RegisterValueChangedCallback(x =>
        {
            ACO_Assitant.antsPerIteration = (int)x.newValue;
        });

        // pheromone intensity slider
        var pheromone_slider = this.Q<Slider>("pheromone_slider");
        pheromone_slider.value = ACO_Assitant.pheromoneIntensity;
        pheromone_slider.RegisterValueChangedCallback(x =>
        {
            ACO_Assitant.pheromoneIntensity = x.newValue;
        });

        // evaporation slider
        var evaporation_slider = this.Q<Slider>("evaporation_slider");
        evaporation_slider.value = ACO_Assitant.evaporationRate;
        evaporation_slider.RegisterValueChangedCallback(x =>
        {
            ACO_Assitant.evaporationRate = x.newValue;
        });

        // seed field
        var seed_field = this.Q<TextField>("seed_field");
        seed_field.value = ACO_Assitant.seed;
        seed_field.RegisterValueChangedCallback(x =>
        {
            ACO_Assitant.seed = x.newValue;
        });

        // weigth list
        var weigth_list = this.Q<ListView>("weigth_list");
        weigth_list.itemsSource = ACO_Assitant.evaluatorWeight;
        weigth_list.makeItem = () => new FloatField();
        weigth_list.bindItem = (e, i) =>
        {
            (e as FloatField).value = ACO_Assitant.evaluatorWeight[i];
            (e as FloatField).RegisterValueChangedCallback(x =>
            {
                ACO_Assitant.evaluatorWeight[i] = x.newValue;
            });
        };


        // Foldout
        foldout = this.Q<Foldout>();
        foreach (var constraint in moduleConstr.Constraints)
        {
            var view = new ConstraintView();
            view.SetData(constraint);
            foldout.Add(view);
        }

        // Print Timers
        toggleTimer = this.Q<Toggle>("ShowTimerToggle");
        toggleTimer.RegisterCallback<ChangeEvent<bool>>(x =>
        {
            ACO_Assitant.printClocks = x.newValue;
        });

        // Execute
        execute = this.Q<Button>("Execute");
        execute.clicked += Execute;

        // Show Constraint
        toggle = this.Q<Toggle>("ShowConstraintToggle");
        toggle.value = ACO_Assitant.visibleConstraints;
        toggle.RegisterCallback<ChangeEvent<bool>>(x =>
        {
            ACO_Assitant.visibleConstraints = x.newValue;
            DrawManager.ReDraw();
            Debug.Log("Show contrains: "+ x.newValue);
        });

        recalculate = new Button();
        recalculate.text = "Recalculate Constraints";
        recalculate.clicked += () =>
        {
            // Save history version to revert if necessary
            var x = LBSController.CurrentLevel;
            Undo.RegisterCompleteObjectUndo(x, "Recalculate Constraints");
            EditorGUI.BeginChangeCheck();

            // Recalculate constraints
            ACO_Assitant.RecalculateConstraint(); // TODO: esto tiene que ser general

            // Mark as dirty
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(x);
            }

            DrawManager.ReDraw();
            Paint();
        };

        Add(recalculate);

        return this;
    }

    private void Execute()
    {
        // Save history version to revert if necessary
        var x = LBSController.CurrentLevel;
        Undo.RegisterCompleteObjectUndo(x, "Execute ACO");
        EditorGUI.BeginChangeCheck();

        // Execute hill climbing
        ACO_Assitant.Execute();

        // Mark as dirty
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(x);
        }
    }

    private void Paint()
    {
        Clear();
        CreateVisualElement();
    }

    public override void Repaint()
    {
        var moduleConstr = ACO_Assitant.Owner.GetModule<ConstrainsZonesModule>();
        foldout.Clear();
        foreach (var constraint in moduleConstr.Constraints)
        {
            var view = new ConstraintView();
            view.SetData(constraint);
            foldout.Add(view);
        }
    }
}
