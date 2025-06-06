using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    
    public class QuestNode_Give : NodeEditor
    {
        private VeQuestTilePicker pickerGiveTarget;
        private VeQuestTilePicker pickerGiveReceiver;
        public QuestNode_Give()
        {
            Clear();
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestNode_Give");
            visualTree.CloneTree(this);
            
            pickerGiveTarget = this.Q<VeQuestTilePicker>("GiveTarget");
            pickerGiveTarget.SetInfo("Object to give", false);
            
            pickerGiveReceiver = this.Q<VeQuestTilePicker>("GiveReceiver");
            pickerGiveReceiver.SetInfo("Target receiver", true);
            

        }

        public override void SetMyData(BaseQuestNodeData data)
        {

        }
    }

}