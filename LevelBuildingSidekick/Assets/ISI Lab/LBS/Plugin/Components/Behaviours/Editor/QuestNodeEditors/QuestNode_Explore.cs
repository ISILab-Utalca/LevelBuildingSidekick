using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
        public class QuestNode_Explore : VisualElement, INodeEditor
        {
                public QuestNode_Explore()
                {
                        var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestNode_Explore");
                        visualTree.CloneTree(this);
                }

                public void SetMyData(BaseQuestNodeData data)
                {
                        // Set values here based on `data`
                }
        }
}