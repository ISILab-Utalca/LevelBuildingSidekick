using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    
    public class QuestNode_Spy : NodeEditor
    {
        private VeQuestTilePicker picker;
        private FloatField requiredSpyTime;
        private Toggle resetOnExit;

        public QuestNode_Spy()
        {
            Clear();
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestNode_Spy");
            visualTree.CloneTree(this);
            
            picker = this.Q<VeQuestTilePicker>("SpyTarget");
            picker.SetInfo("Spy target", true); 
            
            requiredSpyTime = this.Q<FloatField>("SpyTime");
            resetOnExit = this.Q<Toggle>("SpyResetOnExit");
            

        }

        public override void SetMyData(BaseQuestNodeData data)
        {

        }
    }

}