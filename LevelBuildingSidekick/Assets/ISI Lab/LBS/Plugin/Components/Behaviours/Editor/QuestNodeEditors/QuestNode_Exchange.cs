using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using ISILab.LBS.Manipulators;
using LBS.VisualElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    
    public class QuestNode_Exchange : NodeEditor
    {
        private PickerBundle _pickerBundleGive;
        private IntegerField giveAmount;
        private PickerBundle _pickerBundleReceive;
        private IntegerField receiveAmount;
        
        public QuestNode_Exchange()
        {
            Clear();
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestNode_Exchange");
            visualTree.CloneTree(this);
            
            _pickerBundleGive = this.Q<PickerBundle>("ExchangeGiveTarget");
            _pickerBundleGive.SetInfo(
                "Object to give", 
                "The bundle type the player must give at the location.", 
                false); 
            giveAmount = this.Q<IntegerField>("ExchangeGiveAmount");
            
            _pickerBundleReceive = this.Q<PickerBundle>("ExchangeReceiveTarget");
            _pickerBundleReceive.SetInfo(
                "Object to receive",
                "The bundle type the player will receive.",
                false); 
            receiveAmount = this.Q<IntegerField>("ExchangeReceiveAmount");
            
 
        }

        public override void SetNodeData(BaseQuestNodeData data)
        {

            if(data is not DataExchange currentData) return;
            
            #region Give
            _pickerBundleGive.ClearPicker();
            
            _pickerBundleGive._onClicked += () =>
            {
                if (ToolKit.Instance.GetActiveManipulatorInstance() is not QuestPicker pickerManipulator) return;
                pickerManipulator.activeData = currentData;
                pickerManipulator.OnBundlePicked = (layer, pickedGuid, position) =>
                {
                    currentData.bundleGiveType.guid = pickedGuid;
                    _pickerBundleGive.SetTarget(layer,pickedGuid);
                };
            };
            
            _pickerBundleGive.SetTarget(null,currentData.bundleGiveType.guid);
            
            #endregion
            
            
            #region Receive
            
            _pickerBundleReceive.ClearPicker();
            
            _pickerBundleReceive._onClicked += () =>
            {
                if (ToolKit.Instance.GetActiveManipulatorInstance() is not QuestPicker pickerManipulator) return;
                pickerManipulator.activeData = currentData;
                pickerManipulator.OnBundlePicked = (layer, pickedGuid, position) =>
                {
                    currentData.bundleReceiveType.guid = pickedGuid;
                    _pickerBundleReceive.SetTarget(null,currentData.bundleReceiveType.guid);
                };
            };
            
            _pickerBundleReceive.SetTarget(null, currentData.bundleReceiveType.guid);

            #endregion
        }
    }

}