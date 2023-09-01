using Commons.Optimization.Evaluator;
using LBS.Bundles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;

[CustomVisualElement(typeof(GroupCount))]
public class GroupCountVE : EvaluatorVE
{

    FloatField floatField;
    DropdownField dropdown;
    ListView listView;

    public GroupCountVE(IEvaluator evaluator) : base(evaluator)
    {
        var visualTree = DirectoryTools.SearchAssetByName<VisualTreeAsset>("DistanceVE");
        visualTree.CloneTree(this);

        floatField = new FloatField("In Group Distance");
        floatField.value = (evaluator as GroupCount).DistThreshold;
        floatField.RegisterValueChangedCallback((ev) => { (evaluator as GroupCount).DistThreshold = ev.newValue; });

        this.Insert(0, floatField);

        dropdown = this.Q<DropdownField>(name: "Dropdown");
        dropdown.choices = Enum.GetNames(typeof(DistanceType)).ToList();
        dropdown.RegisterValueChangedCallback(UpdateValue);

        listView = this.Q<ListView>(name: "ListView");

        listView.fixedItemHeight = 20;
        listView.itemsSource = (evaluator as GroupCount).WhiteList;
        listView.makeItem = MakeItem;
        //listView.onItemsChosen += OnItemChosen;
        //listView.onSelectionChange += OnSelectionChange;

        listView.bindItem += (item, index) =>
        {
            var view = (item as ObjectField);
            view.label = "Element " + index;
            view.RegisterValueChangedCallback(
                (e) =>
                {
                    (evaluator as GroupCount).WhiteList[index] = e.newValue;
                });
            var data = (evaluator as GroupCount).WhiteList;
            view.value = data[index];
        };
    }

    public void UpdateValue(ChangeEvent<string> e)
    {
        var val = e.newValue;

        if (Enum.TryParse(typeof(DistanceType), val, out object result))
        {
            (evaluator as GroupCount).DistType = (DistanceType)result;
        }
    }

    public VisualElement MakeItem()
    {
        var obj = new ObjectField("Element " + (evaluator as GroupCount).WhiteList.Count);
        obj.objectType = typeof(Bundle);
        return obj;
    }

    public override void Init()
    {
    }
}
