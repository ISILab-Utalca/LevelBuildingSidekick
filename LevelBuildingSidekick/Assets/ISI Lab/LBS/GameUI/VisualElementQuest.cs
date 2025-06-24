using System;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using ISILab.LBS.Settings;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS
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
        
        private Label _questLabel;
        private VisualElement _outerQuestState; // color
        private VisualElement _innerQuestState; // toggle 
        
        private QuestNode _questNode;
        
        public VisualElementQuest()
        {
            CreateVisualElement();
        }
        
        private VisualElement CreateVisualElement()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("VisualElementQuest");
            visualTree.CloneTree(this);
            
            _questLabel = this.Q<Label>("Action");
            _outerQuestState = this.Q<VisualElement>("Outer");
            _innerQuestState = this.Q<VisualElement>("Inner");
            return this;
        }

        /// <summary>
        ///  Assigns the quest and updates the element's display to represent the quest's state
        /// </summary>
        /// <param name="questNode"></param>
        public void SetQuest(QuestNode questNode)
        {
            _questNode = questNode;

            var color = Color.gray;
            bool closed = false;
            bool display = true;
            switch (questNode.QuestState)
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
            
            _outerQuestState.style.borderBottomColor = color;
            _outerQuestState.style.borderTopColor = color;
            _outerQuestState.style.borderLeftColor = color;
            _outerQuestState.style.borderRightColor = color;
            
            _innerQuestState.style.backgroundColor = color;
            
            _questLabel.style.color = new StyleColor(color);
            
            _innerQuestState.style.display = closed ? DisplayStyle.Flex : DisplayStyle.None;
  
            if (closed) _questLabel.text = "<s>" + questNode.QuestAction + "</s>"; // strikethrough 
            else _questLabel.text = questNode.QuestAction;
        }
    }
}
