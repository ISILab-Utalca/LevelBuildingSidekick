using LBS.Components.TileMap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[LBSCustomEditor("BundleData", typeof(BundleData))]
public class BundleDataEditor : LBSCustomEditor
{
    /*public BundleDataEditor(object target) : base(target)
    {
        CreateVisualElement();
        //SetInfo(target);
    }*/

    public override void SetInfo(object target)
    {
        this.target = target;
        CreateVisualElement();
    }

    protected override VisualElement CreateVisualElement()
    {
        var data = target as BundleData;

        this.Add(new Label(data.BundleName));


        this.Add(new Label("Characteristics"));

        foreach(var c in data.Characteristics)
        {
            if (c == null)
            {
                this.Add(new Label("[NULL]"));
                continue;
            }

            // Get type of element
            var type = c.GetType();

            // Get the editors of the selectable elements
            var ves = Utility.Reflection.GetClassesWith<LBSCustomEditorAttribute>()
                    .Where(t => t.Item2.Any(v => v.type == type)).ToList();

            if (ves.Count <= 0)
            {
                // Add basic label if no have specific editor
                this.Add(new Label("'" + type + "' does not contain a visualization."));
                continue;
            }

            // Get editor class
            var edtr = ves.First().Item1;

            // Instantiate editor class
            var ve = Activator.CreateInstance(edtr, new object[] { c }) as LBSCustomEditor;

            this.Add(ve);
        }

        return this;
    }
}
