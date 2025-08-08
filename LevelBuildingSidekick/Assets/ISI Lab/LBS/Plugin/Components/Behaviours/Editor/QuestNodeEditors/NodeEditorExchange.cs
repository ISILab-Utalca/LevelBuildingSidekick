using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using ISILab.LBS.CustomComponents;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    public class NodeEditorExchange : NodeEditor<DataExchange>
    {
        private readonly PickerBundle _pickerBundleGive;
        private readonly PickerBundle _pickerBundleReceive;

        private readonly LBSCustomUnsignedIntegerField _requiredAmount;
        private readonly LBSCustomUnsignedIntegerField _receiveAmount;

        public NodeEditorExchange()
        {
            Clear();
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("NodeEditorExchange");
            visualTree.CloneTree(this);

            _pickerBundleGive = this.Q<PickerBundle>("ExchangeGiveTarget");
            _pickerBundleGive.SetInfo("Object to give", "The bundle type the player must give at the location.");
            
            _pickerBundleReceive = this.Q<PickerBundle>("ExchangeReceiveTarget");
            _pickerBundleReceive.SetInfo("Object to receive", "The bundle type the player will receive.");

            _requiredAmount = this.Q<LBSCustomUnsignedIntegerField>("ExchangeGiveAmount");
            _requiredAmount.RegisterValueChangedCallback(evt =>
            {
                _requiredAmount.SetValueWithoutNotify(evt.newValue);
                if(NodeData is not null) NodeData.requiredAmount = (int)evt.newValue;
            });
            
            _receiveAmount =  this.Q<LBSCustomUnsignedIntegerField>("ExchangeReceiveAmount");
            _receiveAmount.RegisterValueChangedCallback(evt =>
                {
                    _receiveAmount.SetValueWithoutNotify(evt.newValue);
                    if(NodeData is not null) NodeData.receiveAmount = (int)evt.newValue;
                });
        }

        protected override void OnDataAssigned()
        {
            SetupUI();
        }

        private void SetupUI()
        {
            // Setup "Give" picker
            _pickerBundleGive.ClearPicker();
            _pickerBundleGive.OnClicked += () =>
            {
                AssignPickerData().OnBundlePicked = (layer,tileBundle) =>
                {
                    NodeData.bundleGiveType = new BundleType(layer, tileBundle);
                    _pickerBundleGive.SetEditorLayerTarget(NodeData.bundleGiveType);
                };
            };
            _pickerBundleGive.SetEditorLayerTarget(NodeData.bundleGiveType);

            // Setup "Receive" picker
            _pickerBundleReceive.ClearPicker();
            _pickerBundleReceive.OnClicked += () =>
            {
                AssignPickerData().OnBundlePicked = (layer,tileBundle) =>
                {
                    NodeData.bundleReceiveType = new BundleType(layer, tileBundle);
                    _pickerBundleReceive.SetEditorLayerTarget(NodeData.bundleReceiveType);
                };
            };
            _pickerBundleReceive.SetEditorLayerTarget(NodeData.bundleReceiveType);
            
            _requiredAmount.SetValueWithoutNotify((uint)NodeData.requiredAmount);
            _receiveAmount.SetValueWithoutNotify((uint)NodeData.receiveAmount);
        }
    }
}
