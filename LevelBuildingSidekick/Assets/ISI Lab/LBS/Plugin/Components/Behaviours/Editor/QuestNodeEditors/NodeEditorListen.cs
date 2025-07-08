using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    public class NodeEditorListen : NodeEditor<DataListen>
    {
        private readonly PickerBundle _pickerBundle;

        public NodeEditorListen()
        {
            Clear();
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("NodeEditorListen");
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
                AssignPickerData().OnBundlePicked = (layer, tileBundle) =>
                {
                    NodeData.bundleListenTo = new BundleGraph(NodeData, layer, tileBundle);
                    if (layer != null)
                        _pickerBundle.SetEditorLayerTarget(NodeData.bundleListenTo);
                };
            };

            _pickerBundle.SetEditorLayerTarget(NodeData.bundleListenTo);
        }
    }
}
