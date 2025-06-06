using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    
    public class QuestNode_Capture : NodeEditor
    {
        private FloatField requiredCaptureTime;
        private Toggle resetOnExit;
        public QuestNode_Capture()
        {
            Clear();
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestNode_Capture");
            visualTree.CloneTree(this);
            
            requiredCaptureTime = this.Q<FloatField>("CaptureTime");
            resetOnExit = this.Q<Toggle>("CaptureResetOnExit");
            
        }

        public override void SetMyData(BaseQuestNodeData data)
        {
           
        }
    }

}