using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    public class QuestNode_Listen : NodeEditor
    {
        private VeQuestTilePicker picker; 
        public QuestNode_Listen()
        {
            Clear();
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestNode_Listen");
            visualTree.CloneTree(this);
            
            picker = this.Q<VeQuestTilePicker>("ListenTarget");
            picker?.SetInfo("Listen target", true);


        }

        public override void SetMyData(BaseQuestNodeData data)
        {

        }
    }

}