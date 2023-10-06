using LBS;
using LBS.Behaviours;
using LBS.Components;
using LBS.Settings;
using LBS.VisualElements;
using NUnit.Framework.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class LBSLocalCurrent : LBSInspector, IToolProvider
{
    #region FACTORY
    public new class UxmlFactory : UxmlFactory<LBSLocalCurrent, VisualElement.UxmlTraits> { }
    #endregion

    private LBSLayer layer;
    private UnityEngine.Color colorCurrent => LBSSettings.Instance.view.behavioursColor;

    private VisualElement layerContent;
    private VisualElement selectedContent;
    private ModulesPanel modulesPanel;
    private LayerInfoView layerInfoView;


    public LBSLocalCurrent()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("LBSLocalCurrent");
        visualTree.CloneTree(this);

        //LBSEvents.OnSelectElementInWorkSpace += SetCurrentInfo;

        layerContent = this.Q<VisualElement>("LayerContent");

        selectedContent = this.Q<VisualElement>("SelectedContent");

        modulesPanel = this.Q<ModulesPanel>();
        layerInfoView = this.Q<LayerInfoView>();
    }

    public void SetInfo(LBSLayer target)
    {
        // SetLayer reference
        this.layer = target;

        // Set basic Tool
        SetTools(ToolKit.Instance);

        // Set simple module info
        modulesPanel.SetInfo(target.Modules);

        // Set basic layer info
        layerInfoView.SetInfo(target);
    }

    public override void Init( MainView view, LBSLayer layer, LBSBehaviour behaviour)
    {
        throw new NotImplementedException();
    }

    public override void OnLayerChange(LBSLayer layer)
    {
        SetInfo(layer);
    }

    public void SetTools(ToolKit toolkit)
    {
        var icon = Resources.Load<Texture2D>("Icons/Select");
        var selectTool = new Select();
        var t1 = new LBSTool(icon, "Select", selectTool);
        t1.Init(layer, this);
        t1.OnSelect += () => LBSInspectorPanel.ShowInspector("Local", "Current data");
        ToolKit.Instance.AddTool(t1);

        toolkit.AddSeparator();
    }

    public void SetSelectedVE(List<object> objs)
    {
        // Clear previous view
        selectedContent.Clear();

        foreach (var obj in objs)
        {
            // Check if obj is valid
            if(obj == null)
            {
                selectedContent.Add(new Label("[NULL]"));
                continue;
            }

            // Get type of element
            var type = obj.GetType();

            // Get the editors of the selectable elements
            var ves = Utility.Reflection.GetClassesWith<LBSCustomEditorAttribute>()
                    .Where(t => t.Item2.Any(v => v.type == type)).ToList();

            if (ves.Count <= 0)
            {
                // Add basic label if no have specific editor
                selectedContent.Add(new Label("'" + type + "' does not contain a visualization."));
                continue;
            }

            // Get editor class
            var edtr = ves.First().Item1;

            // Instantiate editor class
            var ve = Activator.CreateInstance(edtr) as LBSCustomEditor;

            // set target info on visual element
            ve.SetInfo(obj);

            // create content container
            var container = new DataContent(ve,ves.First().Item2.First().name);

            // Add custom editor
            selectedContent.Add(container);
        }
    }
}
