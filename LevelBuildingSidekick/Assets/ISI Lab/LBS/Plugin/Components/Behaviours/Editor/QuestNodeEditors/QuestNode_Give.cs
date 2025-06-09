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
            pickerGiveTarget.SetInfo(
                "Object to give", 
                    "The bundle type the player must give at the location.",
                false);
            
            pickerGiveReceiver = this.Q<VeQuestTilePicker>("GiveReceiver");
            pickerGiveReceiver.SetInfo(
                "Target receiver", 
                "The object in the graph that will receive the object.",
                true);
            

        }

        public override void SetMyData(BaseQuestNodeData data)
        {

        }
    }

}