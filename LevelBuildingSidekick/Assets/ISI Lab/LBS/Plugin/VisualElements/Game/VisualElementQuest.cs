using System;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using ISILab.LBS.Settings;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISI_Lab.LBS.Plugin.VisualElements.Game
{
    /// <summary>
    /// Visual Element used to indicate quest objectives in the QuestVisualTree UI Document.
    /// meant to display the state of each quest step
    /// </summary>
    [UxmlElement]
    public partial class VisualElementQuest : VisualElement
    {
        [UxmlElementAttribute]
        public new class UxmlFactory { }
        
        private Label questLabel;
        private VisualElement outerQuestState; // color
        private VisualElement innerQuestState; // toggle 
        
        private QuestNode questNode;
        
        public VisualElementQuest()
        {
            CreateVisualElement();
        }
        
        private VisualElement CreateVisualElement()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("VisualElementQuest");
            visualTree.CloneTree(this);
            
            questLabel = this.Q<Label>("Action");
            outerQuestState = this.Q<VisualElement>("Outer");
            innerQuestState = this.Q<VisualElement>("Inner");
            return this;
        }

        /// <summary>
        ///  Assigns the quest and updates the element's display to represent the quest's state
        /// </summary>
        /// <param name="quest"></param>
        public void SetQuest(QuestNode quest)
        {
            questNode = quest;

            var color = Color.gray;
            bool closed = false;
            bool display = true;
            switch (quest.QuestState)
            {
                case QuestState.Blocked: 
                    // hide: display = false;
                    break;
                case QuestState.Active:
                    color = Color.white;
                    break;
                case QuestState.Completed:
                    color = LBSSettings.Instance.view.successColor;
                    closed = true;
                    break;
                case QuestState.Failed:
                    color = LBSSettings.Instance.view.errorColor;
                    closed = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            this.style.display = display ? DisplayStyle.Flex : DisplayStyle.None;
            
            outerQuestState.style.borderBottomColor = color;
            outerQuestState.style.borderTopColor = color;
            outerQuestState.style.borderLeftColor = color;
            outerQuestState.style.borderRightColor = color;
            
            innerQuestState.style.backgroundColor = color;
            
            questLabel.style.color = new StyleColor(color);
            
            innerQuestState.style.display = closed ? DisplayStyle.Flex : DisplayStyle.None;
  
            if (closed) questLabel.text = "<s>" + quest.QuestAction + "</s>"; // strikethrough 
            else questLabel.text = quest.QuestAction;
        }
    }
}
