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
        private FloatField requiredCaptureTime;
        private Toggle resetOnExit;

        public NodeEditorCapture()
        {
            Clear();
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestNode_Capture");
            visualTree.CloneTree(this);
            
            requiredCaptureTime = this.Q<FloatField>("CaptureTime");
            resetOnExit = this.Q<Toggle>("CaptureResetOnExit");
            
        }

        public override void SetNodeData(BaseQuestNodeData data)
        {

        }
    }

}