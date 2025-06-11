using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using ISILab.LBS.Manipulators;
using LBS.VisualElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    
    public class QuestNode_Read : NodeEditor
    {
        private PickerBundle _pickerBundle; 
        public QuestNode_Read()
        {
            Clear();
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestNode_Read");
            visualTree.CloneTree(this);
            
            _pickerBundle = this.Q<PickerBundle>("ReadTarget");
            _pickerBundle.SetInfo(
                "Read target", 
                "The object in the graph that the player must read.",
                true); 
            

        }


        public override void SetNodeData(BaseQuestNodeData data)
        {

            if(data is not DataRead currentData) return;
            
            _pickerBundle.ClearPicker();
            
            _pickerBundle._onClicked += () =>
            {
                if (ToolKit.Instance.GetActiveManipulatorInstance() is not QuestPicker pickerManipulator) return;
                pickerManipulator.activeData = currentData;
                pickerManipulator.OnBundlePicked = (pickedGuid, position) =>
                {
                    currentData.bundleToRead.guid = pickedGuid;
                    currentData.bundleToRead.position = position;
                    _pickerBundle.SetTarget(currentData.bundleToRead.guid, position);
                };
            };
            
            _pickerBundle.SetTarget(currentData.bundleToRead.guid, currentData.bundleToRead.position );
        }
    }

}