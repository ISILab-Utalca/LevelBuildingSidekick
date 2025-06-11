using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using ISILab.LBS.Manipulators;
using LBS.VisualElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    
    public class QuestNode_Gather :NodeEditor
    {
        private VeQuestPickerBundle _pickerBundle;
        private IntegerField gatherAmount;
        private DataGather currentData;
        public QuestNode_Gather()
        {
            Clear();
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestNode_Gather");
            visualTree.CloneTree(this);
            
            _pickerBundle = this.Q<VeQuestPickerBundle>("GatherTarget");
            _pickerBundle.SetInfo(
                "Object to gather", 
                "The bundle type the player must gather/collect within the trigger area.",
                false); 
            gatherAmount = this.Q<IntegerField>("GatherAmount");
            

        }

        public override void SetNodeData(BaseQuestNodeData data)
        { 
            if(data is not DataGather currentData) return;
            
            _pickerBundle.ClearPicker();
            
            _pickerBundle._onClicked += () =>
            {
                if (ToolKit.Instance.GetActiveManipulatorInstance() is not QuestPicker pickerManipulator) return;
                pickerManipulator.activeData = currentData;
                pickerManipulator.OnBundlePicked = (pickedGuid, position) =>
                {
                    currentData.bundleGatherType.guid = pickedGuid;
                    _pickerBundle.SetTarget(currentData.bundleGatherType.guid);
                };
            };
            
            _pickerBundle.SetTarget(currentData.bundleGatherType.guid);


        }
    }

}