using Commons.Optimization.Evaluator;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;
using UnityEditor.UIElements;
using LBS.Bundles;

[CustomVisualElement(typeof(MaximizeDistance))]
public class MaximizeDistanceVE : EvaluatorVE
{
    DropdownField dropdown;
    ListView listView;

    public MaximizeDistanceVE(IEvaluator evaluator) : base(evaluator)
    {

        var visualTree = DirectoryTools.SearchAssetByName<VisualTreeAsset>("DistanceVE");
        visualTree.CloneTree(this);

        dropdown = this.Q<DropdownField>(name: "Dropdown");
        dropdown.choices = Enum.GetNames(typeof(DistanceType)).ToList();
        dropdown.RegisterValueChangedCallback(UpdateValue);

        listView = this.Q<ListView>(name: "ListView");

        listView.fixedItemHeight = 20;
        listView.itemsSource = (evaluator as MaximizeDistance).whiteList;
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
                    (evaluator as MaximizeDistance).whiteList[index] = e.newValue;
                });
            var data = (evaluator as MaximizeDistance).whiteList;
            view.value = data[index];
        };
    }

    public void UpdateValue(ChangeEvent<string> e)
    {
        var val = e.newValue;

        if(Enum.TryParse(typeof(DistanceType), val, out object result))
        {
            (evaluator as MaximizeDistance).distType = (DistanceType)result;
        }
    }

    public VisualElement MakeItem()
    {
        var obj = new ObjectField("Element " + (evaluator as MaximizeDistance).whiteList.Count);
        obj.objectType = typeof(Bundle);
        return obj;
    }

    public void OnItemChosen(IEnumerable<object> objs)
    {
        Debug.Log("OIC");
    }

    public void OnSelectionChange(IEnumerable<object> objs)
    {
        var selected = objs.ToList()[0] as UnityEngine.Object;
        //OnSelectLayer?.Invoke(selected);
    }

    public override void Init()
    {
    }
}
