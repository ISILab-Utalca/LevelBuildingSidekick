using System.ComponentModel.Composition.Hosting;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    public class NodeEditorSpy : NodeEditor<DataSpy>
    {
        private readonly PickerBundle _pickerBundle;
        private readonly FloatField _requiredSpyTime;
        private readonly Toggle _resetOnExit;

        public NodeEditorSpy()
        {
            Clear();
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("NodeEditorSpy");
            visualTree.CloneTree(this);

            _pickerBundle = this.Q<PickerBundle>("SpyTarget");
            _pickerBundle.SetInfo(
                "Spy target",
                "The target in the graph that the player must spy.",
                true);

            _requiredSpyTime = this.Q<FloatField>("SpyTime");
            _resetOnExit = this.Q<Toggle>("SpyResetOnExit");
        }

        protected override void OnDataAssigned()
        {
            _pickerBundle.ClearPicker();
            _pickerBundle.SetEditorLayerTarget(NodeData.bundleToSpy);

            _pickerBundle.OnClicked = () =>
            {
                var pickerManipulator = AssignPickerData();
                pickerManipulator.OnBundlePicked = (layer, tileBundle) =>
                {
                    NodeData.bundleToSpy = new BundleGraph(NodeData, layer, tileBundle);
                    _pickerBundle.SetEditorLayerTarget(NodeData.bundleToSpy);
                };
            };

            _requiredSpyTime.value = NodeData.spyTime;
            _requiredSpyTime.RegisterValueChangedCallback(evt => NodeData.spyTime = evt.newValue);

            _resetOnExit.value = NodeData.resetTimeOnExit;
            _resetOnExit.RegisterValueChangedCallback(evt => NodeData.resetTimeOnExit = evt.newValue);
        }
    }
}
