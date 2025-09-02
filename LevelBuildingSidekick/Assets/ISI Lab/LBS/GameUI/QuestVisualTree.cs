using System.Linq;
using ISILab.LBS.Components;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS
{
    /// <summary>
    /// Example UI class that displays an active quest and its steps.
    /// Must attach to a scene UI document to work.
    /// </summary>
    public class QuestVisualTree : MonoBehaviour
    {
        
        #region FIELDS
        private UIDocument _questVisualTree;
        private ListView _questList;
        private QuestObserver _observer;
        
        [SerializeField]
        private GameObject observerGameObject;
        
        #endregion
        
        #region PROPERTIES
        public GameObject Observer 
        {
            get => observerGameObject;
            set => observerGameObject = value;
        }
        
        #endregion
        
        
        #region METHODS
        public void Start()
        {
           _questVisualTree = GetComponentInParent<UIDocument>();
           var root = _questVisualTree.rootVisualElement;
           _questList = root.Q<ListView>("QuestList");
           if (_questList == null) return;
           
           _observer = observerGameObject.GetComponent<QuestObserver>();
           _observer.OnQuestAdvance +=  UpdateQuest;

           UpdateQuest();
           MakeQuestList();

        }

        private void UpdateQuest()
        {
            var quest = _observer.nodeTriggerMap.Keys.ToList();
            _questList.itemsSource = quest;
            _questList.Rebuild();
        }

        private void MakeQuestList()
        {
            _questList.makeItem = () => new VisualElementQuest(); 
            _questList.bindItem = (element, index) =>
            {
                if (element is not VisualElementQuest questEntryVe) return;
      
                var quest = _questList.itemsSource[index];
                
                // Sub-triggers do not have graph use we only display quests node from graph
                if (quest is QuestNode questNode)
                {
                    questEntryVe.SetQuest(questNode);
        
                    // only display main quest node -NO subnodes!
                    if (questNode.Graph is not null)
                    {
                        questEntryVe.style.display = DisplayStyle.Flex;           
                    }
                }
            };
        }
        
        #endregion
    }
}
