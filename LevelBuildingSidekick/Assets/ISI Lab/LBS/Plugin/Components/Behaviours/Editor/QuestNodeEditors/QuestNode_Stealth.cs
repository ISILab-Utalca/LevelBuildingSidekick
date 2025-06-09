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
        private Vector2Field requiredPosition;
        private DataStealth currentData;

        public QuestNode_Stealth()
        {
            Clear();
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestNode_Stealth");
            visualTree.CloneTree(this);

            requiredPosition = this.Q<Vector2Field>("RequiredPosition");
            observerList = this.Q<ListView>("ObserverList");

            if (observerList == null) return;

            observerList.makeItem = () =>
            {
                var tilePicker = new VeQuestTilePicker();
                tilePicker.SetInfo("Observer target", "Objects that can detect the player.", true);
                return tilePicker;
            };

            observerList.bindItem = (element, i) =>
            {
      
                if (element is not VeQuestTilePicker tilePicker || currentData == null)
                    return;

                // Ensure the list has the correct number of entries
                while (currentData.bundles.Count <= i)
                    currentData.bundles.Add(new bundleGraph(string.Empty, Vector2Int.zero));
                observerList.itemsSource = currentData.bundles; // update source
                
                
                var bundleRef = currentData.bundles[i];

                tilePicker.ClearPicker();
                tilePicker.SetTarget(bundleRef.guid, bundleRef.position);

                tilePicker._onClicked = () =>
                {
                    var pickerManipulator = ToolKit.Instance.GetActiveManipulatorInstance() as QuestPicker;
                    if (pickerManipulator != null)
                    {
                        pickerManipulator.activeData = currentData;
                        pickerManipulator.OnBundlePicked = (pickedGuid, pos) =>
                        {
                            bundleRef.guid = pickedGuid;
                            bundleRef.position = pos;
                            tilePicker.SetTarget(pickedGuid, pos);
                            
                        };
                    }
                };
            };

            observerList.itemsRemoved += (removedIndices) =>
            {
                foreach (int index in removedIndices.OrderByDescending(x => x))
                {
                    if (index >= 0 && index < currentData.bundles.Count)
                        currentData.bundles.RemoveAt(index);
                }
            };

            // Prepare empty list first - on MakeItem assign proper source
            observerList.itemsSource = new List<bundleGraph>();
            observerList.Rebuild();
        }

        public override void SetMyData(BaseQuestNodeData data)
        {
            currentData = data as DataStealth;
            if (currentData == null) return;

            observerList.itemsSource = currentData.bundles;
            observerList.Rebuild();
        }
        
    }
    
}