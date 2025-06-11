using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
        public class QuestNodeRead : NodeEditor<DataRead>
        {
                private readonly PickerBundle _pickerBundle;

                public QuestNodeRead()
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

                protected override void OnDataAssigned()
                {
                        _pickerBundle.ClearPicker();

                        _pickerBundle.OnClicked += () =>
                        {
                                AssignPickerData().OnBundlePicked = (layer, pickedGuid, position) =>
                                {
                                        NodeData.BundleToRead.Layer = layer;
                                        NodeData.BundleToRead.Guid = pickedGuid;
                                        NodeData.BundleToRead.Position = position;
                                        _pickerBundle.SetTarget(layer, pickedGuid, position);
                                };
                        };

                        _pickerBundle.SetTarget(NodeData.BundleToRead.Layer, NodeData.BundleToRead.Guid, NodeData.BundleToRead.Position);
                }
        }
}