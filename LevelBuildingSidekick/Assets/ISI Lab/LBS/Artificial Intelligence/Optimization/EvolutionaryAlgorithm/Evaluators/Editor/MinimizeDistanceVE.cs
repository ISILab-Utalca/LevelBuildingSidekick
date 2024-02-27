using Commons.Optimization.Evaluator;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using LBS.Bundles;
using ISILab.Commons.Utility.Editor;
using ISILab.Extensions;
using ISILab.LBS.AI.VisualElements;
using ISILab.LBS.Internal;
using ISILab.AI.Categorization;

namespace ISILab.LBS.VisualElements
{
    [CustomVisualElement(typeof(MinimizeDistance))]
    public class MinimizeDistanceVE : EvaluatorVE
    {
        DropdownField dropdown;
        ListView listView;

        public MinimizeDistanceVE(IEvaluator evaluator) : base(evaluator)
        {

            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("DistanceVE");
            visualTree.CloneTree(this);

            dropdown = this.Q<DropdownField>(name: "Dropdown");
            dropdown.choices = Enum.GetNames(typeof(DistanceType)).ToList();
            dropdown.RegisterValueChangedCallback(UpdateValue);

            listView = this.Q<ListView>(name: "ListView");

            listView.fixedItemHeight = 20;
            listView.itemsSource = (evaluator as MinimizeDistance).whiteList;
            listView.makeItem = MakeItem;

            listView.bindItem += (item, index) =>
            {
                var view = item as ObjectField;
                view.label = "Element " + index;
                view.RegisterValueChangedCallback(
                    (e) =>
                    {
                        (evaluator as MinimizeDistance).whiteList[index] = e.newValue;
                    });
                var data = (evaluator as MinimizeDistance).whiteList;
                view.value = data[index];
            };
        }

        public void UpdateValue(ChangeEvent<string> e)
        {
            var val = e.newValue;

            if (Enum.TryParse(typeof(DistanceType), val, out object result))
            {
                (evaluator as MinimizeDistance).distType = (DistanceType)result;
            }
        }

        public VisualElement MakeItem()
        {
            var obj = new ObjectField("Element " + (evaluator as MinimizeDistance).whiteList.Count);
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
        }

        public override void Init()
        {
        }
    }
}