using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using ISILab.LBS.Manipulators;
using LBS;
using LBS.VisualElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    public class QuestNode_Listen : NodeEditor
    {
        private PickerBundle _pickerBundle;
        
        public QuestNode_Listen()
        {
            Clear();
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestNode_Listen");
            visualTree.CloneTree(this);
            
            _pickerBundle = this.Q<PickerBundle>("ListenTarget");
            _pickerBundle?.SetInfo(
                "Listen target", 
                "The target in the graph that the player must get close to, in order to complete this action node.", 
                true);
            
        }

        public override void SetNodeData(BaseQuestNodeData data)
        {
            if(data is not DataListen currentData) return;
            
            _pickerBundle.ClearPicker();
            
            _pickerBundle._onClicked += () =>
            {
                if (ToolKit.Instance.GetActiveManipulatorInstance() is not QuestPicker pickerManipulator) return;
                pickerManipulator.activeData = currentData;
                pickerManipulator.OnBundlePicked = (pickedGuid, position) =>
                {
                    currentData.bundleListenTo.guid = pickedGuid;
                    currentData.bundleListenTo.position = position;
                    _pickerBundle.SetTarget(currentData.bundleListenTo.guid, position);
                };
            };
            
            _pickerBundle.SetTarget(currentData.bundleListenTo.guid, currentData.bundleListenTo.position );
            
        }
    }

}