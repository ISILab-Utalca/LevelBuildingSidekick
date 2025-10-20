using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using ISILab.LBS.Editor.Windows;
using ISILab.LBS.VisualElements.Editor;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    public class NodeEditorKill : NodeEditor<DataKill>
    {
        private readonly ListView _killList;

        public NodeEditorKill()
        {
            Clear();
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("NodeEditorKill");
            visualTree.CloneTree(this);

            _killList = this.Q<ListView>("KillList");
            if (_killList == null) return;

            _killList.makeItem = () =>
            {
                var tilePicker = new PickerBundle();
                tilePicker.SetInfo("Kill target", "Targets that the player must kill to complete this action node.", true);
                return tilePicker;
            };

            _killList.bindItem = (element, i) =>
            {
                if (element is not PickerBundle tilePicker || NodeData == null) return;
                if (i < 0 || i >= NodeData.bundlesToKill.Count) return;

                var bundleGraph = NodeData.bundlesToKill[i];
                tilePicker.ClearPicker();
                tilePicker.SetEditorLayerTarget(bundleGraph);

                tilePicker.OnClicked = () =>
                {
                    var pickerManipulator = AssignPickerData();
                    pickerManipulator.OnBundlePicked = (layer,tileBundle) =>
                    {
                        bundleGraph = new BundleGraph(NodeData, layer, tileBundle);
                        tilePicker.SetEditorLayerTarget(bundleGraph);
                        NodeData.bundlesToKill[i] = bundleGraph;
                    };
                };
            };

            _killList.itemsRemoved += (_) =>
            {
                _killList.Rebuild();
                
                // Delay Redraw because Unity reconstructs the list and it gets updated on the next tick
                _killList.schedule.Execute(() =>
                {
                    DrawManager.Instance.RedrawLayer(LBSMainWindow.Instance._selectedLayer);
                }).ExecuteLater(1); 
                
            };
            
            _killList.Rebuild();
        }

        protected override void OnDataAssigned()
        {
            _killList.itemsSource = NodeData.bundlesToKill;
            _killList.Rebuild();
        }
    }
}
