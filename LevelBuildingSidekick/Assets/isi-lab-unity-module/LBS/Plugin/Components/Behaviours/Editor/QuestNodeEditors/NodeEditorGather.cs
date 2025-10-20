using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using ISILab.LBS.CustomComponents;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    public class NodeEditorGather : NodeEditor<DataGather>
    {
        private readonly PickerBundle _pickerBundle;
        private readonly LBSCustomUnsignedIntegerField _gatherAmount;

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

            _gatherAmount = this.Q<LBSCustomUnsignedIntegerField>("GatherAmount");
            
            _gatherAmount.RegisterValueChangedCallback(evt =>
            {
                _gatherAmount.SetValueWithoutNotify((uint)evt.newValue);
                if (NodeData != null)
                {
                    NodeData.gatherAmount = (int)evt.newValue;
                }
            });
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
            _gatherAmount.SetValueWithoutNotify((uint)NodeData.gatherAmount);
        }
    }
}
