using System.Linq;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    public class QuestNodeStealth : NodeEditor<DataStealth>
    {
        private readonly ListView _observerList;
        private readonly PickerVector2Int _requiredPosition;

        public QuestNodeStealth()
        {
            Clear();
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestNode_Stealth");
            visualTree.CloneTree(this);

            _requiredPosition = this.Q<PickerVector2Int>("RequiredPosition");
            _requiredPosition.SetInfo(
                "Stealth Required Position",
                "The position within the graph that the player must reach to complete the action node.");

            _requiredPosition._onClicked = () =>
            {
                var pickerManipulator = AssignPickerData();
                pickerManipulator.OnBundlePicked = (_, _, pos) =>
                {
                    NodeData.objective = pos;
                    _requiredPosition.SetTarget(pos);
                };
            };

            _observerList = this.Q<ListView>("ObserverList");
        }

        protected override void OnDataAssigned()
        {
            _requiredPosition.SetTarget(NodeData.objective);

            if (_observerList == null) return;

            _observerList.itemsSource = NodeData.BundlesObservers;
            _observerList.makeItem = CreateObserverItem;
            _observerList.bindItem = BindObserverItem;

            _observerList.itemsRemoved += removedIndices =>
            {
                foreach (int index in removedIndices.OrderByDescending(x => x))
                {
                    if (index >= 0 && index < NodeData.BundlesObservers.Count)
                        NodeData.BundlesObservers.RemoveAt(index);
                }

                _observerList.Rebuild();
            };

            _observerList.Rebuild();
        }

        private VisualElement CreateObserverItem()
        {
            var tilePicker = new PickerBundle();
            tilePicker.SetInfo("Observer target", "Objects that can detect the player.", true);
            return tilePicker;
        }

        private void BindObserverItem(VisualElement element, int index)
        {
            if (element is not PickerBundle tilePicker) return;
            if (index < 0 || index >= NodeData.BundlesObservers.Count) return;

            var bundle = NodeData.BundlesObservers[index];
            tilePicker.ClearPicker();
            tilePicker.SetTarget(bundle.Layer, bundle.Guid, bundle.Position);

            tilePicker.OnClicked = () =>
            {
                var pickerManipulator = AssignPickerData();
                pickerManipulator.OnBundlePicked = (layer, guid, pos) =>
                {
                    bundle.Layer = layer;
                    bundle.Guid = guid;
                    bundle.Position = pos;
                    tilePicker.SetTarget(layer, guid, pos);
                    NodeData.BundlesObservers[index] = bundle;
                };
            };
        }
    }
}
