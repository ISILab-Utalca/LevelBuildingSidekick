using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    
    public class QuestNode_Report : NodeEditor
    {
        private VeQuestTilePicker picker;

        public QuestNode_Report()
        {
            Clear();
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestNode_Report");
            visualTree.CloneTree(this);
                        
            picker = this.Q<VeQuestTilePicker>("ReportTarget");
            picker.SetInfo(
                "Report target", 
                "The target in the graph, that the player must report to"
                ,true); 
            

        }

        public override void SetMyData(BaseQuestNodeData data)
        {

        }
    }

}