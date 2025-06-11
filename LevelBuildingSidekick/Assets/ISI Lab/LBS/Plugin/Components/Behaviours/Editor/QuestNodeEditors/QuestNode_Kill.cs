using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using UnityEngine.UIElements;
using System.Linq;
using ISILab.LBS.Manipulators;
using LBS.VisualElements;

namespace ISILab.LBS.VisualElements
{
    public class QuestNode_Kill : NodeEditor
    {
        private ListView killList;
        private DataKill currentData;

        public QuestNode_Kill()
        {
            Clear();
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestNode_Kill");
            visualTree.CloneTree(this);
            
            killList = this.Q<ListView>("KillList");

            if (killList == null) return;

            // Create UI element for each item
            killList.makeItem = () =>
            {
                killList.itemsSource ??= currentData.bundlesToKill;
                
                var tilePicker = new PickerBundle();
                tilePicker.SetInfo("Kill target", "Targets that the player must kill to complete this action node.", true);
                return tilePicker;
            };

            // Bind each list item to bundleGraph
            killList.bindItem = (element, i) =>
            {
                if (element is not PickerBundle tilePicker || currentData == null) return;
                if (i < 0 || i >= currentData.bundlesToKill.Count) return;

                var bundleRef = currentData.bundlesToKill[i];

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
                            // Update the bundle data
                            bundleRef.guid = pickedGuid;
                            bundleRef.position = pos;

                            // Refresh UI
                            tilePicker.SetTarget(pickedGuid, pos);

                            // Force re-assign to update the object inside the list if needed
                            currentData.bundlesToKill[i] = bundleRef;
                        };
                    }
                };
            };

            // Handle item removal
            killList.itemsRemoved += (removedIndices) =>
            {
                foreach (int index in removedIndices.OrderByDescending(x => x))
                {
                    if (index >= 0 && index < currentData.bundlesToKill.Count)
                        currentData.bundlesToKill.RemoveAt(index);
                }
                killList.Rebuild();
            };
        }

        public override void SetNodeData(BaseQuestNodeData data)
        {
            currentData = data as DataKill;
            if (currentData == null) return;

            // Sync list
            killList.itemsSource = currentData.bundlesToKill;
            killList.Rebuild();
        }
    }
}
