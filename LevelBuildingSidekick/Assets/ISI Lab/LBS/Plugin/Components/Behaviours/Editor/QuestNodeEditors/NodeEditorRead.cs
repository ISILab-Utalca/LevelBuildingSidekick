using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    public class NodeEditorRead : NodeEditor<DataRead>
    {
        private readonly PickerBundle _pickerBundle;

        public NodeEditorRead()
        {
            Clear();
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("NodeEditorRead");
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
                AssignPickerData().OnBundlePicked = (layer, tileBundle) =>
                {
                    NodeData.bundleToRead = new BundleGraph(NodeData, layer, tileBundle);
                    _pickerBundle.SetEditorLayerTarget(NodeData.bundleToRead);
                };
            };

            _pickerBundle.SetEditorLayerTarget(NodeData.bundleToRead);
        }
    }
}
