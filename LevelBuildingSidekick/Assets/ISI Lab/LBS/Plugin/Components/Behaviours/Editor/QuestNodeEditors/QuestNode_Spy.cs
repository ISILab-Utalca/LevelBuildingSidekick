using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using ISILab.LBS.Manipulators;
using LBS.VisualElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    
    public class QuestNode_Spy : NodeEditor
    {
        private VeQuestPickerBundle _pickerBundle;
        private FloatField requiredSpyTime;
        private Toggle resetOnExit;

        public QuestNode_Spy()
        {
            Clear();
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestNode_Spy");
            visualTree.CloneTree(this);
            
            _pickerBundle = this.Q<VeQuestPickerBundle>("SpyTarget");
            _pickerBundle.SetInfo(
                "Spy target",
                "The target in the graph that the player must spy."
                ,true); 
            
            requiredSpyTime = this.Q<FloatField>("SpyTime");
            resetOnExit = this.Q<Toggle>("SpyResetOnExit");
            
        }

        public override void SetNodeData(BaseQuestNodeData data)
        {
            if(data is not DataSpy currentData) return;
            
            _pickerBundle.ClearPicker();
            
            _pickerBundle._onClicked += () =>
            {
                if (ToolKit.Instance.GetActiveManipulatorInstance() is not QuestPicker pickerManipulator) return;
                pickerManipulator.activeData = currentData;
                pickerManipulator.OnBundlePicked = (pickedGuid, position) =>
                {
                    currentData.bundleToSpy.guid = pickedGuid;
                    currentData.bundleToSpy.position = position;
                    _pickerBundle.SetTarget(currentData.bundleToSpy.guid, position);
                };
            };
            
            _pickerBundle.SetTarget(currentData.bundleToSpy.guid, currentData.bundleToSpy.position );
        }
        
    }

}