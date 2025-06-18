using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    public class QuestNodeGive : NodeEditor<DataGive>
    {
        private readonly PickerBundle _pickerBundleGiveTarget;
        private readonly PickerBundle _pickerBundleGiveReceiver;

        public QuestNodeGive()
        {
            Clear();
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestNode_Give");
            visualTree.CloneTree(this);

            _pickerBundleGiveTarget = this.Q<PickerBundle>("GiveTarget");
            _pickerBundleGiveTarget.SetInfo(
                "Object to give",
                "The bundle type the player must give at the location.",
                false);

            _pickerBundleGiveReceiver = this.Q<PickerBundle>("GiveReceiver");
            _pickerBundleGiveReceiver.SetInfo(
                "Target receiver",
                "The object in the graph that will receive the object.",
                true);
        }

        protected override void OnDataAssigned()
        {
            SetupUI();
        }

        private void SetupUI()
        {
            // Give Picker
            _pickerBundleGiveTarget.ClearPicker();
            _pickerBundleGiveTarget.OnClicked += () =>
            {
                AssignPickerData().OnBundlePicked = (layer, pickedGuid, position) =>
                {
                    NodeData.bundleGive.guid = pickedGuid;
                    _pickerBundleGiveTarget.SetTarget(layer, pickedGuid, position);
                };
            };
            _pickerBundleGiveTarget.SetTarget(null, NodeData.bundleGive.guid);

            // Receiver Picker
            _pickerBundleGiveReceiver.ClearPicker();
            _pickerBundleGiveReceiver.OnClicked += () =>
            {
                AssignPickerData().OnBundlePicked = (layer, pickedGuid, position) =>
                {
                    NodeData.bundleGiveTo.layer = layer;
                    NodeData.bundleGiveTo.guid = pickedGuid;
                    NodeData.bundleGiveTo.position = position;
                    _pickerBundleGiveReceiver.SetTarget(layer, pickedGuid, position);
                };
            };
            _pickerBundleGiveReceiver.SetTarget(NodeData.bundleGiveTo.layer, NodeData.bundleGiveTo.guid, NodeData.bundleGiveTo.position);
        }
    }
}
