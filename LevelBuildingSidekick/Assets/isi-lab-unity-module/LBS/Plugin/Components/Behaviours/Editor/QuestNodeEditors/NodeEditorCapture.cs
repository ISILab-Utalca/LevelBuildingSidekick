using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using ISILab.LBS.Manipulators;
using LBS.VisualElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    
    public class NodeEditorCapture : NodeEditor
    {
        private FloatField _requiredCaptureTime;
        private Toggle _resetOnExit;

        public NodeEditorCapture()
        {
            Clear();
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("NodeEditorCapture");
            visualTree.CloneTree(this);
            
            _requiredCaptureTime = this.Q<FloatField>("CaptureTime");
            _resetOnExit = this.Q<Toggle>("CaptureResetOnExit");
            
        }

        public override void SetNodeData(BaseQuestNodeData data)
        {

        }
    }

}