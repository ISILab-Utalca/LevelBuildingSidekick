using System.ComponentModel.Composition.Hosting;
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
                                AssignPickerData().OnBundlePicked = (layer, positions, pickedGuid, position) =>
                                {
                                        NodeData.bundleReportTo = new BundleGraph(
                                                layer,
                                                positions,
                                                pickedGuid,
                                                position);
                                        if(layer!=null) _pickerBundle.SetTarget(layer.ID, pickedGuid, position);
                                };
                        };

                        _pickerBundle.SetTarget(NodeData.bundleReportTo.layerID, NodeData.bundleReportTo.guid, NodeData.bundleReportTo.position);
                }
        }
}