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
                if (i < 0 || i >= NodeData.BundlesToKill.Count) return;

                var bundleGraph = NodeData.BundlesToKill[i];
                tilePicker.ClearPicker();
                tilePicker.SetTarget(bundleGraph.Layer, bundleGraph.Guid, bundleGraph.Position);

                tilePicker.OnClicked = () =>
                {
                    var pickerManipulator = AssignPickerData();
                    pickerManipulator.OnBundlePicked = (layer, pickedGuid, pos) =>
                    {
                        bundleGraph.Layer = layer;
                        bundleGraph.Guid = pickedGuid;
                        bundleGraph.Position = pos;
                        tilePicker.SetTarget(layer, pickedGuid, pos);
                        NodeData.BundlesToKill[i] = bundleGraph;
                    };
                };
            };

            _killList.itemsRemoved += (removedIndices) =>
            {
                foreach (int index in removedIndices.OrderByDescending(x => x))
                {
                    if (index >= 0 && index < NodeData.BundlesToKill.Count)
                        NodeData.BundlesToKill.RemoveAt(index);
                }
                _killList.Rebuild();
            };
        }

        protected override void OnDataAssigned()
        {
            _killList.itemsSource = NodeData.BundlesToKill;
            _killList.Rebuild();
        }
    }
}
