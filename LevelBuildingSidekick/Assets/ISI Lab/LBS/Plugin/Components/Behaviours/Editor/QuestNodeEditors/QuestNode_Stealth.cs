using System.Linq;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using ISILab.LBS.Editor.Windows;
using ISILab.LBS.VisualElements.Editor;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    public class NodeEditorStealth : NodeEditor<DataStealth>
    {
        private readonly ListView _observerList;
        private readonly PickerVector2Int _requiredPosition;

        public NodeEditorStealth()
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
                pickerManipulator.OnPositionPicked = (pos) =>
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

            _observerList.itemsSource = NodeData.bundlesObservers;
            _observerList.makeItem = CreateObserverItem;
            _observerList.bindItem = BindObserverItem;

            _observerList.itemsRemoved += (_) =>
            {
                _observerList.Rebuild();
                // Redraw to remove any elements that correspond to the deleted element
                DrawManager.Instance.RedrawLayer(LBSMainWindow.Instance._selectedLayer, MainView.Instance);
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
            if (index < 0 || index >= NodeData.bundlesObservers.Count) return;

            var bundleGraph = NodeData.bundlesObservers[index];
            tilePicker.ClearPicker();
            tilePicker.SetEditorLayerTarget(bundleGraph);

            tilePicker.OnClicked = () =>
            {
                var pickerManipulator = AssignPickerData();
                pickerManipulator.OnBundlePicked = (layer, tileBundle) =>
                {
                    bundleGraph = new BundleGraph(NodeData, layer, tileBundle);
                    if(layer!=null) tilePicker.SetEditorLayerTarget(bundleGraph);
                    NodeData.bundlesObservers[index] = bundleGraph;
                };
            };
        }
    }
}
