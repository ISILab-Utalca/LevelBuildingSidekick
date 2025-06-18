using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
        public class QuestNodeReport : NodeEditor<DataReport>
        {
                private readonly PickerBundle _pickerBundle;

                public QuestNodeReport()
                {
                        Clear();
                        var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestNode_Report");
                        visualTree.CloneTree(this);

                        _pickerBundle = this.Q<PickerBundle>("ReportTarget");
                        _pickerBundle.SetInfo("Report target", "The target in the graph, that the player must report to", true);
                }

                protected override void OnDataAssigned()
                {
                        SetupUI();
                }

                private void SetupUI()
                {
                        _pickerBundle.ClearPicker();

                        _pickerBundle.OnClicked += () =>
                        {
                                AssignPickerData().OnBundlePicked = (layer, pickedGuid, position) =>
                                {
                                        NodeData.bundleReportTo.layer = layer;
                                        NodeData.bundleReportTo.guid = pickedGuid;
                                        NodeData.bundleReportTo.position = position;
                                        _pickerBundle.SetTarget(layer, pickedGuid, position);
                                };
                        };

                        _pickerBundle.SetTarget(NodeData.bundleReportTo.layer, NodeData.bundleReportTo.guid, NodeData.bundleReportTo.position);
                }
        }
}