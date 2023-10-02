using LBS.Bundles;
using LBS.Components.TileMap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.UI.InputField;

[LBSCustomEditor("Zone", typeof(Zone))]
public class ZoneEditor : LBSCustomEditor
{
    private Zone _target;

    private TextField nameField;
    private ColorField colorField;
    private ObjectField objectField;

    private ListView bundleList;
    private List<string> bundlesRef; // ?

    public ZoneEditor() 
    {
        CreateVisualElement();
    }

    public override void SetInfo(object target)
    {
        // Set referenced Zone
        this._target = target as Zone;

        // Get bundles
        var bundles = LBSAssetsStorage.Instance.Get<Bundle>();

        // Set basic values
        nameField.value = this._target.ID;
        colorField.value = this._target.Color;

        // PARCHE (!)
        if (this._target.TagsBundles.Count > 0)
        {
            foreach (var bundle in bundles)
            {
                if (bundle.name == this._target.TagsBundles[0])
                {
                    objectField.value = bundle;
                    break;
                }
            } // objectField.value = bundles.First(b => b.ID.Label == this.target.TagsBundles[0]);
        }

        bundlesRef = this._target.TagsBundles;
    }

    protected override VisualElement CreateVisualElement()
    {
        // NameField
        this.nameField = new TextField("Name");
        nameField.RegisterCallback<ChangeEvent<string>>(evt =>
        {
            _target.ID = evt.newValue;
        });
        this.Add(nameField);

        // ColorField
        this.colorField = new ColorField("Color");
        colorField.RegisterCallback<ChangeEvent<Color>>(evt => 
        {
            _target.Color = evt.newValue;
        });
        this.Add(colorField);

        // ObjectField (Bundle)
        this.objectField = new ObjectField("Bundle");
        this.objectField.objectType = typeof(Bundle);
        objectField.RegisterValueChangedCallback(v => // objectField.RegisterCallback<ChangeEvent<ScriptableObject>>(evt => 
        {
            var bundle = v.newValue as Bundle;
            _target.TagsBundles = new List<string>() { bundle.name };
        });
        this.Add(objectField);

        // BundleList
        /*
        this.bundleList = new ListView();
        bundleList.itemsSource = bundlesRef;
        bundleList.makeItem = MakeItem;
        bundleList.bindItem = BindItem;
        this.Add(bundleList);
        */

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
        var field = (ve as ObjectField);

        var bundles = LBSAssetsStorage.Instance.Get<Bundle>();
        bundles = bundles.Where(b=> b.Name == bundlesRef[index]).ToList();

        field.value = bundles[index];
    }

}
