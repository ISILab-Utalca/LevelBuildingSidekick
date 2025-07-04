using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    public class NodeEditorGive : NodeEditor<DataGive>
    {
        private readonly PickerBundle _pickerBundleGiveTarget;
        private readonly PickerBundle _pickerBundleGiveReceiver;

        public NodeEditorGive()
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
                AssignPickerData().OnBundlePicked = (layer,tilebundle) =>
                {
                    NodeData.bundleGive = new BundleType(layer, tilebundle);
                    _pickerBundleGiveTarget.SetEditorLayerTarget(NodeData.bundleGive);
                };
            };
            _pickerBundleGiveTarget.SetEditorLayerTarget(NodeData.bundleGive);

            // Receiver Picker
            _pickerBundleGiveReceiver.ClearPicker();
            _pickerBundleGiveReceiver.OnClicked += () =>
            {
                AssignPickerData().OnBundlePicked = (layer, tilebundle) =>
                {
                    NodeData.bundleGiveTo = new BundleGraph(NodeData, layer, tilebundle);
                    if(layer!=null)  _pickerBundleGiveReceiver.SetEditorLayerTarget(NodeData.bundleGiveTo);
                };
            };
            _pickerBundleGiveReceiver.SetEditorLayerTarget(NodeData.bundleGiveTo);
        }
    }
}
