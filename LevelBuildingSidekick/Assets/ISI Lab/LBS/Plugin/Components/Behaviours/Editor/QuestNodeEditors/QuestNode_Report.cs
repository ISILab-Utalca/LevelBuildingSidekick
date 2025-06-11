using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using ISILab.LBS.Manipulators;
using LBS.VisualElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    
    public class QuestNode_Report : NodeEditor
    {
        private PickerBundle _pickerBundle;

        public QuestNode_Report()
        {
            Clear();
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestNode_Report");
            visualTree.CloneTree(this);
                        
            _pickerBundle = this.Q<PickerBundle>("ReportTarget");
            _pickerBundle.SetInfo(
                "Report target", 
                "The target in the graph, that the player must report to"
                ,true); 
            

        }

        public override void SetNodeData(BaseQuestNodeData data)
        {
            if(data is not DataReport currentData) return;
            
            _pickerBundle.ClearPicker();
            
            _pickerBundle._onClicked += () =>
            {
                if (ToolKit.Instance.GetActiveManipulatorInstance() is not QuestPicker pickerManipulator) return;
                pickerManipulator.activeData = currentData;
                pickerManipulator.OnBundlePicked = (layer, pickedGuid, position) =>
                {
                    currentData.bundleReportTo.layer = layer;   
                    currentData.bundleReportTo.guid = pickedGuid;
                    currentData.bundleReportTo.position = position;
                    _pickerBundle.SetTarget(layer, currentData.bundleReportTo.guid, position);
                };
            };
            
            _pickerBundle.SetTarget(currentData.bundleReportTo.layer,currentData.bundleReportTo.guid, currentData.bundleReportTo.position );
        }
    }

}