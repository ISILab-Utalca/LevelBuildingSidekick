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
        private VeQuestTilePicker picker;
        private DataListen currentData;
        public QuestNode_Listen()
        {
            Clear();
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestNode_Listen");
            visualTree.CloneTree(this);
            
            picker = this.Q<VeQuestTilePicker>("ListenTarget");
            picker?.SetInfo(
                "Listen target", 
                "The target in the graph that the player must get close to, in order to complete this action node.", 
                true);
            
        }

        public override void SetMyData(BaseQuestNodeData data)
        {
            currentData = data as DataListen;
            if(currentData is null) return;
            
            picker.ClearPicker();
            
            picker._onClicked += () =>
            {
                var pickerManipulator = ToolKit.Instance.GetActiveManipulatorInstance() as QuestPicker;
                if (pickerManipulator != null)
                {
                    pickerManipulator.activeData = currentData;
                    pickerManipulator.OnBundlePicked = (pickedGuid, position) =>
                    {
                        currentData.bundleToListen.guid = pickedGuid;
                        currentData.bundleToListen.position = position;
                        picker.SetTarget(currentData.bundleToListen.guid, position);
                    };
                }
            };
            
            picker.SetTarget(currentData.bundleToListen.guid, currentData.bundleToListen.position );
            
        }
    }

}