using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using System.Linq;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    public class QuestNodeKill : NodeEditor<DataKill>
    {
        private readonly ListView _killList;

        public QuestNodeKill()
        {
            Clear();
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestNode_Kill");
            visualTree.CloneTree(this);

            _killList = this.Q<ListView>("KillList");
            if (_killList == null) return;

            _killList.makeItem = () =>
            {
                var tilePicker = new PickerBundle();
                tilePicker.SetInfo("Kill target", "Targets that the player must kill to complete this action node.", true);
                return tilePicker;
            };

            _killList.bindItem = (element, i) =>
            {
                if (element is not PickerBundle tilePicker || NodeData == null) return;
                if (i < 0 || i >= NodeData.bundlesToKill.Count) return;

                var bundleGraph = NodeData.bundlesToKill[i];
                tilePicker.ClearPicker();
                tilePicker.SetTarget(bundleGraph.layer, bundleGraph.guid, bundleGraph.position);

                tilePicker.OnClicked = () =>
                {
                    var pickerManipulator = AssignPickerData();
                    pickerManipulator.OnBundlePicked = (layer, pickedGuid, pos) =>
                    {
                        bundleGraph.layer = layer;
                        bundleGraph.guid = pickedGuid;
                        bundleGraph.position = pos;
                        tilePicker.SetTarget(layer, pickedGuid, pos);
                        NodeData.bundlesToKill[i] = bundleGraph;
                    };
                };
            };

            _killList.itemsRemoved += (removedIndices) =>
            {
                foreach (int index in removedIndices.OrderByDescending(x => x))
                {
                    if (index >= 0 && index < NodeData.bundlesToKill.Count)
                        NodeData.bundlesToKill.RemoveAt(index);
                }
                _killList.Rebuild();
            };
        }

        protected override void OnDataAssigned()
        {
            _killList.itemsSource = NodeData.bundlesToKill;
            _killList.Rebuild();
        }
    }
}
