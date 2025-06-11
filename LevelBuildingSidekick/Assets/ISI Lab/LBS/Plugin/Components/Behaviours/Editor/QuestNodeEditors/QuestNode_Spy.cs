using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    public class QuestNodeSpy : NodeEditor<DataSpy>
    {
        private readonly PickerBundle _pickerBundle;
        private readonly FloatField _requiredSpyTime;
        private readonly Toggle _resetOnExit;

        public QuestNodeSpy()
        {
            Clear();
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestNode_Spy");
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
            _pickerBundle.SetTarget(NodeData.BundleToSpy.Layer, NodeData.BundleToSpy.Guid, NodeData.BundleToSpy.Position);

            _pickerBundle.OnClicked = () =>
            {
                var pickerManipulator = AssignPickerData();
                pickerManipulator.OnBundlePicked = (layer, pickedGuid, position) =>
                {
                    NodeData.BundleToSpy.Layer = layer;
                    NodeData.BundleToSpy.Guid = pickedGuid;
                    NodeData.BundleToSpy.Position = position;
                    _pickerBundle.SetTarget(layer, pickedGuid, position);
                };
            };

            _requiredSpyTime.value = NodeData.spyTime;
            _requiredSpyTime.RegisterValueChangedCallback(evt => NodeData.spyTime = evt.newValue);

            _resetOnExit.value = NodeData.resetTimeOnExit;
            _resetOnExit.RegisterValueChangedCallback(evt => NodeData.resetTimeOnExit = evt.newValue);
        }
    }
}
