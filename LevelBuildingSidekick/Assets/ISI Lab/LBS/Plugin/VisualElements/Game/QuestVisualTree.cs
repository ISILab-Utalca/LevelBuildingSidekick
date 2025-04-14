using System.Collections.Generic;
using ISILab.LBS;
using ISILab.LBS.Components;
using ISILab.LBS.Modules;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISI_Lab.LBS.Plugin.VisualElements.Game
{
    /// <summary>
    /// Example UI class that displays an active quest and its steps.
    /// Must attach to a scene UI document to work.
    /// </summary>
    public class QuestVisualTree : MonoBehaviour
    {
        
        #region PROPERTIES
        private UIDocument questVisualTree;
        private ListView questList;
        #endregion
        
        #region METHODS
        public void Awake()
        {
           questVisualTree = GetComponentInParent<UIDocument>();
           var root = questVisualTree.rootVisualElement;
           questList = root.Q<ListView>("QuestList");
           if (questList == null) return;
           
           List<QuestNode> exampleQuestNodes = new List<QuestNode>();
           var tempGraph = new QuestGraph();
           
           var q1 = new QuestNode("1", Vector2.one, "go to place", tempGraph);
           q1.QuestState = QuestState.completed;
           var q2 = new QuestNode("2", Vector2.one, "go to place", tempGraph);
           q2.QuestState = QuestState.failed;
           var q3 = new QuestNode("3", Vector2.one, "kiss enemy entity", tempGraph);
           q3.QuestState = QuestState.active;
           var q4 = new QuestNode("4", Vector2.one, "collect n shits", tempGraph);
           q4.QuestState = QuestState.blocked;
           
           exampleQuestNodes.Add(q1);
           exampleQuestNodes.Add(q2);
           exampleQuestNodes.Add(q3);
           exampleQuestNodes.Add(q4);
           
           SetQuestList(exampleQuestNodes);
 
        }

        public void UpdateQuestList(List<QuestNode> quests)
        {
            
        }

        private void SetQuestList(List<QuestNode> quests)
        {
            questList.makeItem = () => new VisualElementQuest(); 
            questList.bindItem = (element, index) =>
            {
                var questEntryVe = element as VisualElementQuest;
                if (questEntryVe == null) return;

                var quest = quests[index]; 
               
                questEntryVe.SetQuest(quest);
            };
            
            questList.itemsSource = quests;
        }
        
        #endregion
    }
}
