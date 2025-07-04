using System.ComponentModel.Composition.Hosting;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    public class NodeEditorTake : NodeEditor<DataTake>
    {
        private readonly PickerBundle _pickerBundle;

        public NodeEditorTake()
        {
            Clear();
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestNode_Take");
            visualTree.CloneTree(this);

            _pickerBundle = this.Q<PickerBundle>("TakeTarget");
            _pickerBundle.SetInfo(
                "Take target",
                "The target in the graph that the player must take.",
                true);
        }

        protected override void OnDataAssigned()
        {
            _pickerBundle.ClearPicker();
            _pickerBundle.SetEditorLayerTarget(NodeData.bundleToTake);

            _pickerBundle.OnClicked = () =>
            {
                var pickerManipulator = AssignPickerData();
                pickerManipulator.OnBundlePicked = (layer, tileBundle) =>
                {
                    NodeData.bundleToTake = new BundleGraph(NodeData, layer, tileBundle);

                    if (layer != null)
                        _pickerBundle.SetEditorLayerTarget(NodeData.bundleToTake);
                };
            };
        }
    }
}
