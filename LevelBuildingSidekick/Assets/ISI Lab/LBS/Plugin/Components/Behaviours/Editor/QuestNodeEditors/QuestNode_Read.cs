using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    
    public class QuestNode_Read : NodeEditor
    {
        private VeQuestTilePicker picker; 
        public QuestNode_Read()
        {
            Clear();
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestNode_Read");
            visualTree.CloneTree(this);
            
            picker = this.Q<VeQuestTilePicker>("ReadTarget");
            picker.SetInfo(
                "Read target", 
                "The object in the graph that the player must read.",
                true); 
            

        }


        public override void SetMyData(BaseQuestNodeData data)
        {

        }
    }

}