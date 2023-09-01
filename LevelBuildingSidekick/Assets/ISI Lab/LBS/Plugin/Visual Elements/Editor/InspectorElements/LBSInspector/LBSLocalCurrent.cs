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

    public VisualElement currentContent;

    public LBSLocalCurrent()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("LBSLocalCurrent");
        visualTree.CloneTree(this);

        //LBSEvents.OnSelectElementInWorkSpace += SetCurrentInfo;

        currentContent = this.Q<VisualElement>("CurrentContent");
    }

    public void SetCurrentInfo(object obj)
    {
        currentContent.Clear();

        var so = (ScriptableObject)obj;

        var editor = Editor.CreateEditor(so);


        var inspector = new IMGUIContainer(() =>
        {
            editor.OnInspectorGUI();
        });

        currentContent.Add(inspector);
    }

    public void SetInfo(LBSLayer target)
    {
        // SetLayer reference
        this.layer = target;

        // Set basic Tool
        SetTools(ToolKit.Instance);

        currentContent.Clear();
        foreach (var module in target.Modules)
        {

        }

        /*
        contentContainer.Clear();
        foreach(var b in target.Behaviours)
        {
            var type = b.GetType();
            var ves = Utility.Reflection.GetClassesWith<LBSCustomEditorAttribute>().Where(t => t.Item2.Any(v => v.type == type)); 
            if (ves.Count() == 0)
            {
                throw new Exception("[ISI Lab] No class marked as LBSCustomEditor found for type: " + type);
            }

            var ve = Activator.CreateInstance(ves.First().Item1, new object[] { b });
            if (!(ve is VisualElement))
            {
                throw new Exception("[ISI Lab] " + ve.GetType() + " is not a VisualElement ");
            }

            contentContainer.Add(ve as VisualElement);
        }*/

        /*
        contentAssist.Clear();

        this.target = target;

        if (target.Assitants.Count <= 0)
        {
            noContentPanel.SetDisplay(true);
        }

        foreach (var assist in target.Assitants)
        {
            var so = Utility.Reflection.MakeGenericScriptable(assist);
            var editor = Editor.CreateEditor(so);

            var cont = new VisualElement();
            cont.style.backgroundColor = new Color(0, 0, 0, 0.1f);
            cont.SetBorder(new Color(0, 0, 0, 0.6f), 1);
            cont.SetBorderRadius(3);
            cont.SetPaddings(4);

            var inspector = new IMGUIContainer(() =>
            {
                editor.OnInspectorGUI();
            });

            cont.Add(inspector);
            contentAssist.Add(cont);
        }
        */
    }

    public override void Init(List<IManipulatorLBS> lBSManipulators, MainView view, LBSLayer layer, LBSBehaviour behaviour)
    {
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
        ToolKit.Instance.AddTool(t1);
    }

    public void SetSelectedVE(List<object> objs)
    {
        // Clear previous view
        currentContent.Clear();

        foreach (var obj in objs)
        {
            // Get type of element
            var type = obj.GetType();

            // Get the editors of the selectable elements
            var ves = Utility.Reflection.GetClassesWith<LBSCustomEditorAttribute>()
                    .Where(t => t.Item2.Any(v => v.type == type)).ToList();

            if (ves.Count <= 0)
            {
                // Add basic label if no have specific editor
                currentContent.Add(new Label("'"+type+ "' does not contain a visualization."));
                continue;
            }

            // Get editor class
            var edtr = ves.First().Item1;

            // Instantiate editor class
            var ve = Activator.CreateInstance(edtr) as LBSCustomEditor;

            // set target info on visual element
            ve.SetInfo(obj);

            // Add custom editor
            currentContent.Add(ve as VisualElement);
        }
    }
}
