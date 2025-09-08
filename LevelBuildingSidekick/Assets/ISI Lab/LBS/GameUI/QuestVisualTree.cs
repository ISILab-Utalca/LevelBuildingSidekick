using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace ISILab.LBS
{
    public class QuestVisualTree : MonoBehaviour
    {
        [SerializeField] 
        private GameObject trackerGO;
        [SerializeField] 
        private QuestTracker tracker;
        
        
        private UIDocument _questVisualTree;
        private TreeView _questTree;


        public GameObject GO
        {
            get => trackerGO;
            set => trackerGO = value;
        }

        private void Start()
        {
            _questVisualTree = GetComponentInParent<UIDocument>();
            var root = _questVisualTree.rootVisualElement;

            _questTree = root.Q<TreeView>("QuestTree");
            if (_questTree == null) return;

            tracker = trackerGO.GetComponent<QuestTracker>();
            tracker.OnQuestAdvance += UpdateQuest;

            ConfigureTree();
            UpdateQuest();
        }

        private void ConfigureTree()
        {
            // Create a VisualElement for each quest
            _questTree.makeItem = () => new VisualElementQuest();

            // Bind quest objective data to its visual element
            _questTree.bindItem = (element, index) =>
            {
                if (element is VisualElementQuest questEntryVe)
                {
                    var item = _questTree.GetItemDataForIndex<QuestObjective>(index);
                    questEntryVe.SetQuest(item);
                }
                
            };
            
        }

        private void UpdateQuest()
        {
            if (tracker == null) return;

            var objectives = tracker.Objectives;
            if (objectives == null) return;

            // Build TreeViewItemData hierarchy
            var rootItems = new List<TreeViewItemData<QuestObjective>>();
            foreach (var rootObjective in objectives)
            {
                rootItems.Add(BuildTreeRecursive(rootObjective));
            }

            // Assign tree root
            _questTree.SetRootItems(rootItems);

            // Refresh the tree
            _questTree.Rebuild();
            
            _questTree.ExpandAll();
            
        }

        private TreeViewItemData<QuestObjective> BuildTreeRecursive(QuestObjective objective)
        {
            var children = new List<TreeViewItemData<QuestObjective>>();

            foreach (var branch in objective.GetBranches())
            {
                var subs = objective.GetSubObjectives(branch);
                if (subs == null) continue;

                foreach (var subTrigger in subs)
                {
                    var subObjective = new QuestObjective(subTrigger);
                    children.Add(BuildTreeRecursive(subObjective));
                }
            }

            return new TreeViewItemData<QuestObjective>(
                objective.Trigger.GetInstanceID(), // unique ID
                objective,
                children
            );
        }
    }
}
