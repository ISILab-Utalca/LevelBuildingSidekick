using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
        public class QuestNodeGather : NodeEditor<DataGather>
        {
                private readonly PickerBundle _pickerBundle;
                private readonly IntegerField _gatherAmount;

                public QuestNodeGather()
                {
                        Clear();
                        var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestNode_Gather");
                        visualTree.CloneTree(this);

                        _pickerBundle = this.Q<PickerBundle>("GatherTarget");
                        _pickerBundle.SetInfo(
                                "Object to gather",
                                "The bundle type the player must gather/collect within the trigger area."
                        );

                        _gatherAmount = this.Q<IntegerField>("GatherAmount");
                        _gatherAmount.RegisterValueChangedCallback(evt => NodeData.gatherAmount = evt.newValue);
                }

                protected override void OnDataAssigned()
                {
                        _pickerBundle.ClearPicker();

                        _pickerBundle.OnClicked += () =>
                        {
                                AssignPickerData().OnBundlePicked = (_, pickedGuid, _) =>
                                {
                                        NodeData.bundleGatherType.guid = pickedGuid;
                                        _pickerBundle.SetTarget(null, NodeData.bundleGatherType.guid);
                                };
                        };

                        _pickerBundle.SetTarget(null, NodeData.bundleGatherType.guid);
                        _gatherAmount.SetValueWithoutNotify(NodeData.gatherAmount);
                }
        }
}