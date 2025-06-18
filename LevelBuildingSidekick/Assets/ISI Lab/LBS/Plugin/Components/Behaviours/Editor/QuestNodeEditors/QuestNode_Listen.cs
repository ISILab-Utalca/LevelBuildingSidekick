using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
        public class QuestNodeListen : NodeEditor<DataListen>
        {
                private readonly PickerBundle _pickerBundle;

                public QuestNodeListen()
                {
                        Clear();
                        var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestNode_Listen");
                        visualTree.CloneTree(this);

                        _pickerBundle = this.Q<PickerBundle>("ListenTarget");
                        _pickerBundle.SetInfo(
                                "Listen target",
                                "The target in the graph that the player must get close to, in order to complete this action node.",
                                true);
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
                                        NodeData.bundleListenTo.layer = layer;
                                        NodeData.bundleListenTo.guid = pickedGuid;
                                        NodeData.bundleListenTo.position = position;
                                        _pickerBundle.SetTarget(layer, pickedGuid, position);
                                };
                        };

                        _pickerBundle.SetTarget(NodeData.bundleListenTo.layer, NodeData.bundleListenTo.guid, NodeData.bundleListenTo.position);
                }
        }
}