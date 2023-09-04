using LBS.Bundles;
using LBS.Components.TileMap;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.UI.InputField;

[LBSCustomEditor("Zone", typeof(Zone))]
public class ZoneEditor : LBSCustomEditor
{
    private Zone target;

    private TextField nameField;
    private ColorField colorField;

    private ListView bundleList;
    private List<string> bundlesRef; // ?

    public ZoneEditor() 
    {
        CreateVisualElement();
    }

    public override void SetInfo(object target)
    {
        this.target = target as Zone;

        nameField.value = this.target.ID;
        colorField.value = this.target.Color;
        bundlesRef = this.target.TagsBundles;
    }

    protected override VisualElement CreateVisualElement()
    {
        // NameField
        this.nameField = new TextField("Name");
        nameField.RegisterCallback<ChangeEvent<string>>(evt =>
        {
            target.ID = evt.newValue;
        });
        this.Add(nameField);

        // ColorField
        this.colorField = new ColorField("Color");
        colorField.RegisterCallback<ChangeEvent<Color>>(evt => 
        {
            target.Color = evt.newValue;
        });
        this.Add(colorField);

        //BundleList
        this.bundleList = new ListView();
        bundleList.itemsSource = bundlesRef;
        bundleList.makeItem = MakeItem;
        bundleList.bindItem = BindItem;
        this.Add(bundleList);

        return this;

    }

    private VisualElement MakeItem()
    {
        var field = new ObjectField();
        field.objectType = typeof(Bundle);
        return field;
    }

    private void BindItem(VisualElement ve, int index)
    {

    }

}
