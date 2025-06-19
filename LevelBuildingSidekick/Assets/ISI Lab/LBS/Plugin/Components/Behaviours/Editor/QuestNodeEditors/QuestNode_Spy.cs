using System.ComponentModel.Composition.Hosting;
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
            _pickerBundle.SetTarget(NodeData.bundleToSpy.layerID, NodeData.bundleToSpy.guid, NodeData.bundleToSpy.position);

            _pickerBundle.OnClicked = () =>
            {
                var pickerManipulator = AssignPickerData();
                pickerManipulator.OnBundlePicked = (layer, positions, pickedGuid, position) =>
                {
                    NodeData.bundleToSpy = new BundleGraph(
                        layer,
                        positions,
                        pickedGuid,
                        position);
       
                    if(layer!=null) _pickerBundle.SetTarget(layer.ID, pickedGuid, position);
                };
            };

            _requiredSpyTime.value = NodeData.spyTime;
            _requiredSpyTime.RegisterValueChangedCallback(evt => NodeData.spyTime = evt.newValue);

            _resetOnExit.value = NodeData.resetTimeOnExit;
            _resetOnExit.RegisterValueChangedCallback(evt => NodeData.resetTimeOnExit = evt.newValue);
        }
    }
}
