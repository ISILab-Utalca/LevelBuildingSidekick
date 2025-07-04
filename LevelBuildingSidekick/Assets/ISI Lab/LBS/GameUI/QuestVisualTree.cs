using System.Collections.Generic;
using System.Linq;
using ISILab.LBS.Components;
using ISILab.LBS.Modules;
using UnityEngine;
using UnityEngine.Serialization;
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
            var quest = _observer.NodeTriggerMap.Keys.ToList();
            _questList.itemsSource = quest;
            _questList.Rebuild();
        }

        private void MakeQuestList()
        {
            _questList.makeItem = () => new VisualElementQuest(); 
            _questList.bindItem = (element, index) =>
            {
                var questEntryVe = element as VisualElementQuest;
                if (questEntryVe == null) return;

                var quest = _questList.itemsSource[index]; 
               
                questEntryVe.SetQuest(quest as QuestNode);
            };
        }
        
        #endregion
    }
}
