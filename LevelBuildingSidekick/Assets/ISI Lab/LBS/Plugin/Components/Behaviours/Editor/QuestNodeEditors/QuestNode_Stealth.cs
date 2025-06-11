using System;
using System.Collections.Generic;
using System.Linq;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using ISILab.LBS.Manipulators;
using LBS.VisualElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    public class QuestNode_Stealth : NodeEditor
    {
        private ListView observerList;
        private PickerVector2Int requiredPosition;
        private DataStealth currentData;

        public QuestNode_Stealth()
        {
            Clear();
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestNode_Stealth");
            visualTree.CloneTree(this);
            
            requiredPosition = this.Q<PickerVector2Int>("RequiredPosition");
            requiredPosition.SetInfo(
                "Stealth Required Position", 
                "The position within the graph, that the player must reach to complete the action node.");
            requiredPosition._onClicked = () =>
            {  
                var pickerManipulator = ToolKit.Instance.GetActiveManipulatorInstance() as QuestPicker;
                if (pickerManipulator == null) return;
                
                pickerManipulator.activeData = currentData;
                pickerManipulator.OnBundlePicked = (layer, pickedGuid, pos) =>
                {
                    // Update the bundle data
                    currentData.objective = pos;

                    // Refresh UI
                    requiredPosition.SetTarget(pos);
                };

            };
            
            
            
            observerList = this.Q<ListView>("ObserverList");

            if (observerList == null) return;

            // Create UI element for each item
            observerList.makeItem = () =>
            {
                observerList.itemsSource ??= currentData.bundlesObservers;
                
                var tilePicker = new PickerBundle();
                tilePicker.SetInfo("Observer target", "Objects that can detect the player.", true);
                return tilePicker;
            };

            // Bind each list item to bundleGraph
            observerList.bindItem = (element, i) =>
            {
                if (element is not PickerBundle tilePicker || currentData == null) return;
                if (i < 0 || i >= currentData.bundlesObservers.Count) return;

                var bundleGraph = currentData.bundlesObservers[i];

                tilePicker.ClearPicker();
                tilePicker.SetTarget(bundleGraph.layer, bundleGraph.guid, bundleGraph.position);

                tilePicker._onClicked = () =>
                {
                    var pickerManipulator = ToolKit.Instance.GetActiveManipulatorInstance() as QuestPicker;
                    if (pickerManipulator != null)
                    {
                        pickerManipulator.activeData = currentData;
                        pickerManipulator.OnBundlePicked = (layer, pickedGuid, pos) =>
                        {
                            // Update the bundle data
                            bundleGraph.layer = layer;    
                            bundleGraph.guid = pickedGuid;
                            bundleGraph.position = pos;

                            // Refresh UI
                            tilePicker.SetTarget(layer, pickedGuid, pos);

                            // Force re-assign to update the object inside the list if needed
                            currentData.bundlesObservers[i] = bundleGraph;
                        };
                    }
                };
            };

            // Handle item removal
            observerList.itemsRemoved += (removedIndices) =>
            {
                foreach (int index in removedIndices.OrderByDescending(x => x))
                {
                    if (index >= 0 && index < currentData.bundlesObservers.Count)
                        currentData.bundlesObservers.RemoveAt(index);
                }
                observerList.Rebuild();
            };
        }

        public override void SetNodeData(BaseQuestNodeData data)
        {
            currentData = data as DataStealth;
            if (currentData == null) return;

            // Sync list
            observerList.itemsSource = currentData.bundlesObservers;
            observerList.Rebuild();

            requiredPosition.SetTarget(currentData.objective);
        }
    }
}
