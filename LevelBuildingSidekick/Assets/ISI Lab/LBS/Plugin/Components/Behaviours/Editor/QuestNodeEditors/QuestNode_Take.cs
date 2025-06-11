using System.Collections.Generic;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using ISILab.LBS.Manipulators;
using LBS.VisualElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    
    public class QuestNode_Take : NodeEditor
    {
        private PickerBundle _pickerBundle;

        public QuestNode_Take()
        {
            Clear();
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestNode_Take");
            visualTree.CloneTree(this);
            
            _pickerBundle = this.Q<PickerBundle>("TakeTarget");
            _pickerBundle.SetInfo(
                "Take target", 
                "the target in the graph that the player must take."
                ,true); 
            

        }

        public override void SetNodeData(BaseQuestNodeData data)
        {
            if(data is not DataTake currentData) return;
                        
             _pickerBundle.ClearPicker();
                        
             _pickerBundle._onClicked += () =>
             {
                 if (ToolKit.Instance.GetActiveManipulatorInstance() is not QuestPicker pickerManipulator) return;
                 
                 pickerManipulator.activeData = currentData;
                 pickerManipulator.OnBundlePicked = (pickedGuid, position) =>
                 {
                     currentData.bundleToTake.guid = pickedGuid;
                     _pickerBundle.SetTarget(currentData.bundleToTake.guid, position);
                 };
             };
                        
             _pickerBundle.SetTarget(currentData.bundleToTake.guid, currentData.bundleToTake.position );

        }
    }

}