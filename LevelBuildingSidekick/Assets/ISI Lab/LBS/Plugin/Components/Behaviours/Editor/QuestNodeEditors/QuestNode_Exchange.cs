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
        private VeQuestPickerBundle _pickerBundleGive;
        private IntegerField giveAmount;
        private VeQuestPickerBundle _pickerBundleReceive;
        private IntegerField receiveAmount;
        
        public QuestNode_Exchange()
        {
            Clear();
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestNode_Exchange");
            visualTree.CloneTree(this);
            
            _pickerBundleGive = this.Q<VeQuestPickerBundle>("ExchangeGiveTarget");
            _pickerBundleGive.SetInfo(
                "Object to give", 
                "The bundle type the player must give at the location.", 
                false); 
            giveAmount = this.Q<IntegerField>("ExchangeGiveAmount");
            
            _pickerBundleReceive = this.Q<VeQuestPickerBundle>("ExchangeReceiveTarget");
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
                pickerManipulator.OnBundlePicked = (pickedGuid, position) =>
                {
                    currentData.bundleGiveType.guid = pickedGuid;
                    _pickerBundleGive.SetTarget(currentData.bundleGiveType.guid);
                };
            };
            
            _pickerBundleGive.SetTarget(currentData.bundleGiveType.guid);
            
            #endregion
            
            
            #region Receive
            
            _pickerBundleReceive.ClearPicker();
            
            _pickerBundleReceive._onClicked += () =>
            {
                if (ToolKit.Instance.GetActiveManipulatorInstance() is not QuestPicker pickerManipulator) return;
                pickerManipulator.activeData = currentData;
                pickerManipulator.OnBundlePicked = (pickedGuid, position) =>
                {
                    currentData.bundleReceiveType.guid = pickedGuid;
                    _pickerBundleReceive.SetTarget(currentData.bundleReceiveType.guid);
                };
            };
            
            _pickerBundleReceive.SetTarget(currentData.bundleReceiveType.guid);

            #endregion
        }
    }

}