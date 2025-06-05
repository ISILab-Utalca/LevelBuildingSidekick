using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    
    public class QuestNode_Gather : VisualElement, INodeEditor
    {
        protected VisualElement CreateVisualElement()
        {
            Clear();
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestNode_Gather");
            visualTree.CloneTree(this);
            
            
            return this;
        }

        public void SetMyData(BaseQuestNodeData data)
        {
            throw new System.NotImplementedException();
        }
    }

}