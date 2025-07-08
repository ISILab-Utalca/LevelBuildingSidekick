using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    public class NodeEditorGather : NodeEditor<DataGather>
    {
        private readonly PickerBundle _pickerBundle;
        private readonly IntegerField _gatherAmount;

        public NodeEditorGather()
        {
            Clear();
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("NodeEditorGather");
            visualTree.CloneTree(this);

            _pickerBundle = this.Q<PickerBundle>("GatherTarget");
            _pickerBundle.SetInfo(
                "Object to gather",
                "The bundle type the player must gather/collect within the trigger area."
                );

            _gatherAmount = this.Q<IntegerField>("GatherAmount");
            _gatherAmount.RegisterValueChangedCallback(evt => NodeData.gatherAmount = evt.newValue);
        }

        protected override void OnDataAssigned()
        {
            _pickerBundle.ClearPicker();

            _pickerBundle.OnClicked += () =>
            {
                AssignPickerData().OnBundlePicked = (layer, tileBundle) =>
                {
                    NodeData.bundleGatherType = new BundleType(layer, tileBundle);
                    _pickerBundle.SetEditorLayerTarget(NodeData.bundleGatherType);
                };
            };

            _pickerBundle.SetEditorLayerTarget(NodeData.bundleGatherType);
            _gatherAmount.SetValueWithoutNotify(NodeData.gatherAmount);
        }
    }
}
