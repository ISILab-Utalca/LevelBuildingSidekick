using System.Collections.Generic;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    
    public class QuestNode_Take : NodeEditor
    {
        private VeQuestTilePicker picker;

        public QuestNode_Take()
        {
            Clear();
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestNode_Take");
            visualTree.CloneTree(this);
            
            picker = this.Q<VeQuestTilePicker>("TakeTarget");
            picker.SetInfo(
                "Take target", 
                "the target in the graph that the player must take."
                ,true); 
            

        }

        public override void SetMyData(BaseQuestNodeData data)
        {
 
        }
    }

}