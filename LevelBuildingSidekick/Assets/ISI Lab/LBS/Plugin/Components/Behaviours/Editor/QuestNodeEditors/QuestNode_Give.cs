using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using ISILab.LBS.Manipulators;
using LBS.VisualElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    
    public class QuestNode_Give : NodeEditor
    {
        private VeQuestPickerBundle _pickerBundleGiveTarget;
        private VeQuestPickerBundle _pickerBundleGiveReceiver;
        public QuestNode_Give()
        {
            Clear();
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestNode_Give");
            visualTree.CloneTree(this);
            
            _pickerBundleGiveTarget = this.Q<VeQuestPickerBundle>("GiveTarget");
            _pickerBundleGiveTarget.SetInfo(
                "Object to give", 
                    "The bundle type the player must give at the location.",
                false);
            
            _pickerBundleGiveReceiver = this.Q<VeQuestPickerBundle>("GiveReceiver");
            _pickerBundleGiveReceiver.SetInfo(
                "Target receiver", 
                "The object in the graph that will receive the object.",
                true);
            

        }

        public override void SetNodeData(BaseQuestNodeData data)
        {
            if(data is not DataGive currentData) return;
            
            #region Give
            _pickerBundleGiveTarget.ClearPicker();
            
            _pickerBundleGiveTarget._onClicked += () =>
            {
                if (ToolKit.Instance.GetActiveManipulatorInstance() is not QuestPicker pickerManipulator) return;
                pickerManipulator.activeData = currentData;
                pickerManipulator.OnBundlePicked = (pickedGuid, position) =>
                {
                    currentData.bundleGive.guid = pickedGuid;
                    _pickerBundleGiveTarget.SetTarget(currentData.bundleGive.guid);
                };
            };
            
            _pickerBundleGiveTarget.SetTarget(currentData.bundleGive.guid);
            
            #endregion
            
            
            #region Receive
            
            _pickerBundleGiveReceiver.ClearPicker();
            
            _pickerBundleGiveReceiver._onClicked += () =>
            {
                if (ToolKit.Instance.GetActiveManipulatorInstance() is not QuestPicker pickerManipulator) return;
                pickerManipulator.activeData = currentData;
                pickerManipulator.OnBundlePicked = (pickedGuid, position) =>
                {
                    currentData.bundleGiveTo.guid = pickedGuid;
                    currentData.bundleGiveTo.position = position;
                    
                    _pickerBundleGiveReceiver.SetTarget(currentData.bundleGiveTo.guid, position);
                };
            };
            
            _pickerBundleGiveReceiver.SetTarget(currentData.bundleGiveTo.guid, currentData.bundleGiveTo.position);

            #endregion
        }
    }

}