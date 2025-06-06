using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
        public class QuestNode_Explore : NodeEditor
        {
                private IntegerField subareas;
                private readonly Toggle useRandomPoint;

                public QuestNode_Explore()
                {
                        Clear();
                        var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestNode_Explore");
                        visualTree.CloneTree(this);
                        
                        subareas = this.Q<IntegerField>("ExploreSubareas");
                        
                        useRandomPoint = this.Q<Toggle>("ExploreUseRandomPosition");
                        useRandomPoint.RegisterValueChangedCallback(evt =>
                        {
                                subareas.style.display =  evt.newValue ? DisplayStyle.None : DisplayStyle.Flex;
                        });

                        useRandomPoint.value = false;
                }

                public override void SetMyData(BaseQuestNodeData data)
                {
                        // Set values here based on `data`
                }
        }
}