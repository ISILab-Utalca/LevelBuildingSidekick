using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
        public class QuestNodeTake : NodeEditor<DataTake>
        {
                private readonly PickerBundle _pickerBundle;

                public QuestNodeTake()
                {
                        Clear();
                        var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestNode_Take");
                        visualTree.CloneTree(this);

                        _pickerBundle = this.Q<PickerBundle>("TakeTarget");
                        _pickerBundle.SetInfo(
                                "Take target",
                                "The target in the graph that the player must take.",
                                true);
                }

                protected override void OnDataAssigned()
                {
                        _pickerBundle.ClearPicker();
                        _pickerBundle.SetTarget(NodeData.BundleToTake.Layer, NodeData.BundleToTake.Guid, NodeData.BundleToTake.Position);

                        _pickerBundle.OnClicked = () =>
                        {
                                var pickerManipulator = AssignPickerData();
                                pickerManipulator.OnBundlePicked = (layer, pickedGuid, position) =>
                                {
                                        NodeData.BundleToTake.Layer = layer;
                                        NodeData.BundleToTake.Guid = pickedGuid;
                                        NodeData.BundleToTake.Position = position;
                                        _pickerBundle.SetTarget(layer, pickedGuid, position);
                                };
                        };
                }
        }
}