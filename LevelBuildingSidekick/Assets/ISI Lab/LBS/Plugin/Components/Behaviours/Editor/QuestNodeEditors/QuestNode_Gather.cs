using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    
    public class QuestNode_Gather :NodeEditor
    {
        private VeQuestTilePicker pickerGather;
        private IntegerField gatherAmount;
        public QuestNode_Gather()
        {
            Clear();
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestNode_Gather");
            visualTree.CloneTree(this);
            
            pickerGather = this.Q<VeQuestTilePicker>("GatherTarget");
            pickerGather.SetInfo(
                "Object to gather", 
                "The bundle type the player must gather/collect within the trigger area.",
                false); 
            gatherAmount = this.Q<IntegerField>("GatherAmount");
            

        }

        public override void SetMyData(BaseQuestNodeData data)
        {

        }
    }

}